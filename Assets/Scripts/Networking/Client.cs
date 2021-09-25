using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    private static Thread _listeningThread;
    private string nick;
    private IPEndPoint _remoteAddr;
    private UdpClient _sender;

    public GameObject server;
    private void Awake()
    {
        string prefIp = PlayerPrefs.GetString("ip");

        if (PlayerPrefs.GetInt("isServer") == 1)
        {
            server.SetActive(true);
            _remoteAddr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), GameServer.Port);
        }
        else
        {
            server.SetActive(false);
            _remoteAddr = new IPEndPoint(IPAddress.Parse(prefIp), GameServer.Port);
            
            Debug.Log("Client only mode");
        }
        
        nick = PlayerPrefs.GetString("nick", Environment.UserName);
        
        bufferPlayer = new NetPlayer(nick);
    }

    private void Start()
    {
        _sender = new UdpClient();
        _sender.Connect(_remoteAddr);

        _listeningThread = new Thread(new ThreadStart(delegate
        {
            while (true)
            {
                byte[] data = _sender.Receive(ref _remoteAddr);

                // TODO implement game logick
                NetPlayer[] players = Utils.ByteArrayToObject(data) as NetPlayer[];
                
                lock (playerTable)
                {
                    foreach (var np in players)
                    {
                        if (np is null)
                            continue;
                        
                        playerTable[np.nick] = np;
                        Debug.Log("From server: Nick:" + np.nick + " Pos: " + np.Position + " Rot: " + np.Rotation);
                    }
                }
            }
        }));

        _listeningThread.Start();

        Debug.Log("Client started");
    }

    public GameObject targetPlayer;
    public Transform XRot;
    public Transform YRot;
    
    private NetPlayer bufferPlayer;
    private void FixedUpdate()
    {
        // TODO implement client logick (send position, etc)
        bufferPlayer.Position = targetPlayer.transform.position;
        bufferPlayer.Rotation = new Vector3(XRot.rotation.eulerAngles.x, YRot.rotation.eulerAngles.y);

        byte[] outputBytes = Utils.ObjectToByteArray(bufferPlayer); // the most important bytes)0
        _sender.Send(outputBytes, outputBytes.Length);
    }

    private readonly Hashtable playerTable = new Hashtable();
    private readonly Hashtable initedPlayers = new Hashtable();

    private void Update()
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
    }
    
    private void OnApplicationQuit()
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
}