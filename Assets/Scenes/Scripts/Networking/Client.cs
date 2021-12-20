using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Networking;
using TMPro;
using UnityEngine;
using Utils;

public class Client : Player
{
    public static Client Instance;

    public GameObject server;
    public Hands hands;

    private Thread udpClientThread;

    new void Awake()
    {
        base.Awake();
        Instance = this;
        AwakeUdp();
    }

    void Start()
    {
        udpClientThread = new Thread(StartUdp);
        udpClientThread.Start();
        Debug.Log("Client started");
    }

    void FixedUpdate()
    {
        FixedUpdateUdp();
    }

    private void Update()
    {
        UpdateUdp();
    }

    private void OnApplicationQuit()
    {
        CloseUdp();
        udpClientThread.Abort();
    }

    #region UDP

    public static string nick;
    private IPEndPoint _remoteAddr;

    void AwakeUdp()
    {
        string prefIp = PlayerPrefs.GetString("ip", "127.0.0.1");

        if (PlayerPrefs.GetInt("isServer", 1) == 1)
        {
            server.SetActive(true);
            _remoteAddr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), NewServer.Port);
            Debug.Log("UDP Client-Server mode");
        }
        else
        {
            server.SetActive(false);
            _remoteAddr = new IPEndPoint(IPAddress.Parse(prefIp), NewServer.Port);
            Debug.Log("UDP Client only mode");
        }

        nick = PlayerPrefs.GetString("nick", Environment.UserName);

        bufferPlayer = new NetPlayer(nick);
    }

    public EndPoint epServer;
    public Socket clientSocket; //The main client socket
    byte[] byteData = new byte[Utils.Network.BUFFER_SIZE];

    private void OnReceive(IAsyncResult ar)
    {
        try
        {
            int bytes = clientSocket.EndReceive(ar);

            //Convert the bytes received into an object of type Data
            var packet = Packer.UnPack(byteData, bytes);

            //Accordingly process the message received
            switch (packet.chanelID)
            {
                case ChanelID.Login:

                    Login login = Data.ByteArrayToObject(packet.data) as Login;
                    Debug.LogWarning("Client: new player: " + login.nick);

                    break;

                case ChanelID.Logout:
                    Logout logout = Data.ByteArrayToObject(packet.data) as Logout;
                    MainThreadBridge.DoInMainThread(() =>
                    {
                        lock (playerTable)
                        {
                            Destroy((GameObject) initedPlayers[logout.nick]);
                            playerTable.Remove(logout.nick);
                            initedPlayers.Remove(logout.nick);
                        }
                    });
                    break;

                case ChanelID.PlayerPosition:

                    NetPlayer[] players = Data.ByteArrayToObject(packet.data) as NetPlayer[];

                    lock (playerTable)
                    {
                        foreach (var np in players)
                        {
                            if (np is null)
                                continue;
                            playerTable[np.nick] = np;
                        }
                    }

                    break;
                case ChanelID.ChangeWeapon:

                    ChangeWeapon changeWeapon = (ChangeWeapon) Data.ByteArrayToObject(packet.data);
                    if (changeWeapon.nick != Client.nick)
                    {
                        Debug.Log("Смена оружки на " + changeWeapon.weapon + " у " + changeWeapon.nick);
                        ((NetPlayerData) dataPlayers[changeWeapon.nick]).botHands.ApplyWeapon(changeWeapon.weapon);
                    }

                    break;
                case ChanelID.SpawnDecal:

                    SpawnDecal spawnDecal = (SpawnDecal) Data.ByteArrayToObject(packet.data);
                    hands.SpawnDecal(spawnDecal);

                    break;
                case ChanelID.Damage:

                    //Хто надамажил меня? и насколько?
                    SendDamage sendDamage = (SendDamage) Data.ByteArrayToObject(packet.data);
                    if (sendDamage.analDamager == Client.nick)
                    {
                        Debug.Log("Ты попал мужик...");
                        (dataPlayers[sendDamage.anal] as NetPlayerData).botState.ApplyDamage(sendDamage.damage);
                        AddMoney();
                        Debug.Log("На" + sendDamage.damage + " дамага");
                    }

                    if (sendDamage.anal == Client.nick)
                    {
                        Debug.Log("Ты маслину поймал мужик...");
                        ApplyDamage(sendDamage.damage);
                        Debug.Log("На" + sendDamage.damage + " дамага");
                    }

                    Debug.Log($"anal {sendDamage.anal}, nick {Client.nick} , analDamager {sendDamage.analDamager}");

                    break;

                case ChanelID.Respawn:
                    Respawn respawn = (Respawn) Data.ByteArrayToObject(packet.data);
                    if (respawn.nick == Client.nick)
                        eventsToRaise.Enqueue(() =>
                        {
                           targetPlayer.transform.position = new Vector3(0, 4, -2);
                        });
                    break;

                default:
                    string message = Encoding.Unicode.GetString(packet.data);
                    Debug.LogWarning("Server says: " + message);
                    Debug.LogWarningFormat("Это был пакет {0} канала", packet.chanelID);
                    break;
            }

            byteData = new byte[Utils.Network.BUFFER_SIZE];
            //Start listening to receive more data from the user
            clientSocket.BeginReceiveFrom(byteData, 0, Utils.Network.BUFFER_SIZE, SocketFlags.None, ref epServer,
                OnReceive, null);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"Server: {ex.Message}, {ex.Source}, {ex.StackTrace}, {ex.Data}");
        }
    }

    private void StartUdp()
    {
        try
        {
            //Using UDP sockets
            clientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            //IP address of the server machine
            IPAddress ipAddress = _remoteAddr.Address;
            //Server is listening on port 1000
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, NewServer.Port);

            epServer = (EndPoint) ipEndPoint;


            Login login = new Login(nick);
            // content bytes
            byte[] tempBytes = Data.ObjectToByteArray(login);
            // bytes to send or bytes from server
            byteData = Packer.CombinePacket(ChanelID.Login, tempBytes);
            //Login to the server
            clientSocket.BeginSendTo(byteData, 0, byteData.Length,
                SocketFlags.None, epServer, OnSend, null);


            byteData = new byte[Utils.Network.BUFFER_SIZE];
            //Start listening to the data asynchronously
            clientSocket.BeginReceiveFrom(byteData,
                0, Utils.Network.BUFFER_SIZE,
                SocketFlags.None,
                ref epServer,
                OnReceive,
                null);
        }
        catch (Exception ex)
        {
            Debug.Log($"Server: {ex.Message}");
        }
    }

    private void OnSend(IAsyncResult ar)
    {
        try
        {
            clientSocket.EndSend(ar);
        }
        catch (Exception ex)
        {
            Debug.Log($"Server: {ex.Message}");
        }
    }

    public void SendUdpData(byte[] data)
    {
        try
        {
            clientSocket.BeginSendTo(data, 0, data.Length,
                SocketFlags.None, epServer, OnSend, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Client: {ex.Message}, {ex.Source}, {ex.StackTrace}, {ex.Data}");
        }
    }

    public GameObject targetPlayer;
    public Transform XRot;
    public Transform YRot;

    private NetPlayer bufferPlayer;

    private void FixedUpdateUdp()
    {
        // TODO implement client logick (send position, etc)
        bufferPlayer.Position = targetPlayer.transform.position;
        bufferPlayer.Rotation = new Vector3(XRot.rotation.eulerAngles.x, YRot.rotation.eulerAngles.y);

        byte[] outputBytes = Data.ObjectToByteArray(bufferPlayer); // the most important bytes)0
        byte[] outputValidBytes = Packer.CombinePacket(ChanelID.PlayerPosition, outputBytes);
        SendUdpData(outputValidBytes);
    }

    private readonly Hashtable playerTable = new Hashtable();
    private readonly Hashtable initedPlayers = new Hashtable();
    private readonly Hashtable dataPlayers = new Hashtable();

    private void UpdateUdp()
    {
        lock (playerTable)
        {
            if (playerTable.Count == 0)
                return;

            foreach (DictionaryEntry o in playerTable)
            {
                NetPlayer netPlayer = o.Value as NetPlayer;
                if (netPlayer.nick != nick)
                {
                    if (initedPlayers.ContainsKey(netPlayer.nick))
                    {
                        ((GameObject) initedPlayers[netPlayer.nick]).transform.position = netPlayer.Position;
                        ((GameObject) initedPlayers[netPlayer.nick]).transform.rotation =
                            Quaternion.Euler(netPlayer.Rotation);
                    }
                    else
                    {
                        SpawnPlayer(netPlayer);
                    }
                }
            }
        }
    }

    public GameObject netPlayerPrefab;

    private void SpawnPlayer(NetPlayer player)
    {
        var newPlayer = Instantiate(netPlayerPrefab, player.Position, Quaternion.identity);
        newPlayer.GetComponentInChildren<TextMeshPro>().text = player.nick;
        initedPlayers[player.nick] = newPlayer;
        var settings = newPlayer.GetComponent<NetPlayerData>();
        settings.nick = player.nick;
        dataPlayers[player.nick] = settings;
    }

    public void CloseUdp()
    {
        // Send Logout command
        Logout login = new Logout(nick);
        byte[] tempBytes = Data.ObjectToByteArray(login);
        byteData = Packer.CombinePacket(ChanelID.Logout, tempBytes);
        clientSocket.BeginSendTo(byteData, 0, byteData.Length,
            SocketFlags.None, epServer, OnSend, null);

        try
        {
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        // You must close the udp listener
        try
        {
            clientSocket.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        Debug.Log("Client stopped");
    }

    #endregion
}