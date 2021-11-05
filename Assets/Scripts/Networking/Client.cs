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

public class Client : MonoBehaviour
{
    public static Client Instance;

    public GameObject server;
    public Hands hands;

    void Awake()
    {
        Instance = this;

        AwakeUdp();
    }

    void Start()
    {
        StartUdp();
        StartTcp();
        Debug.Log("Client started");
    }

    void FixedUpdate()
    {
        FixedUpdateUdp();
    }

    private void Update()
    {
        UpdateUdp();
        UpdateTcp();
    }

    private void OnApplicationQuit()
    {
        CloseUdp();
        CloseTcp();
    }

    #region UDP

    private static Thread _listeningThread;
    public static string nick;
    private IPEndPoint _remoteAddr;
    private UdpClient _sender;

    void AwakeUdp()
    {
        string prefIp = PlayerPrefs.GetString("ip", "127.0.0.1");

        if (PlayerPrefs.GetInt("isServer", 1) == 1)
        {
            server.SetActive(true);
            _remoteAddr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), UdpServer.Port);
            Debug.Log("UDP Client-Server mode");
        }
        else
        {
            server.SetActive(false);
            _remoteAddr = new IPEndPoint(IPAddress.Parse(prefIp), UdpServer.Port);
            Debug.Log("UDP Client only mode");
        }

        nick = PlayerPrefs.GetString("nick", Environment.UserName);

        bufferPlayer = new NetPlayer(nick);
    }

    private void StartUdp()
    {
        _sender = new UdpClient();
        _sender.Connect(_remoteAddr);

        _listeningThread = new Thread(new ThreadStart(async delegate
        {
            while (true)
            {
                //byte[] data = _sender.Receive(ref _remoteAddr);
                UdpReceiveResult hona = await _sender.ReceiveAsync().ConfigureAwait(false);
                byte[] data = hona.Buffer;

                // TODO implement game logick
                NetPlayer[] players = Data.ByteArrayToObject(data) as NetPlayer[];

                lock (playerTable)
                {
                    foreach (var np in players)
                    {
                        if (np is null)
                            continue;

                        playerTable[np.nick] = np;
                        /*
                         * Debug.Log("From server: Nick:" + np.nick + " Pos: " + np.Position + " Rot: " + np.Rotation);
                         */
                    }
                }
            }
        }));

        _listeningThread.Start();
    }

    public GameObject targetPlayer;
    public Transform XRot;
    public Transform YRot;

    private NetPlayer bufferPlayer;

    public void SendUdpData(byte[] data)
    {
        //_sender.Send(data, data.Length);
        _sender.SendAsync(data, data.Length);
    }

    private void FixedUpdateUdp()
    {
        // TODO implement client logick (send position, etc)
        bufferPlayer.Position = targetPlayer.transform.position;
        bufferPlayer.Rotation = new Vector3(XRot.rotation.eulerAngles.x, YRot.rotation.eulerAngles.y);

        byte[] outputBytes = Data.ObjectToByteArray(bufferPlayer); // the most important bytes)0
        _sender.Send(outputBytes, outputBytes.Length);
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

    private void CloseUdp()
    {
        try
        {
            _sender.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        // You must close the udp listener
        try
        {
            _sender.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        _listeningThread.Abort();

        Debug.Log("Client stopped");
    }

    #endregion

    #region TCP

    private Socket clientSocket;
    private byte[] buffer;

    void StartTcp()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Connect to the specified host.
        string prefIp = PlayerPrefs.GetString("ip", "127.0.0.1");
        IPEndPoint endPoint;
        if (PlayerPrefs.GetInt("isServer", 1) == 1)
        {
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), UdpServer.Port);
            Debug.Log("TCP Client-Server mode");
        }
        else
        {
            endPoint = new IPEndPoint(IPAddress.Parse(prefIp), UdpServer.Port);
            Debug.Log("TCP Client only mode");
        }

        clientSocket.BeginConnect(endPoint, ConnectCallback, null);
    }

    private void UpdateTcp()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var data = Encoding.Unicode.GetBytes("get time");
            clientSocket.Send(data);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            var data = Encoding.Unicode.GetBytes("GG");
            clientSocket.Send(data);
        }
    }

    private void ConnectCallback(IAsyncResult AR)
    {
        try
        {
            clientSocket.EndConnect(AR);
            buffer = new byte[clientSocket.ReceiveBufferSize];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (SocketException ex)
        {
            Debug.LogError(ex.Message);
        }
        catch (ObjectDisposedException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        try
        {
            int received = clientSocket.EndReceive(AR);

            if (received == 0)
            {
                return;
            }

            try
            {
                var packet = Packer.UnPack(buffer, received);

                //#
                switch (packet.chanelID)
                {
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
                        }

                        if (sendDamage.anal == Client.nick)
                        {
                            Debug.Log("Ты маслину поймал мужик...");
                            hands.selfState.hp -= sendDamage.damage;
                        }

                        Debug.Log($"anal {sendDamage.anal}, nick {Client.nick} , analDamager {sendDamage.analDamager}");

                        break;
                    default:
                        string message = Encoding.Unicode.GetString(buffer);
                        Debug.LogWarning("Server says: " + message);
                        Debug.LogWarningFormat("Это был пакет {0} канала", packet.chanelID);
                        break;
                }

                //#
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }

            // Start receiving data again.
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        // Avoid Pokemon exception handling in cases like these.
        catch (SocketException ex)
        {
            Debug.LogError(ex.Message);
        }
        catch (ObjectDisposedException ex)
        {
            Debug.Log("Client disconnect");
            //Debug.LogError(ex.Message);
        }
    }

    public void SendTcpData(byte[] data)
    {
        clientSocket.Send(data);
    }

    void CloseTcp()
    {
        try
        {
            clientSocket.Shutdown(SocketShutdown.Both);
        }
        finally
        {
            clientSocket.Close();
        }
    }

    #endregion
}