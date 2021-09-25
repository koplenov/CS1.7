using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static GameServer gameServer = new GameServer();

    void Start()
    {
        gameServer.Start();
    }

    private void OnApplicationQuit()
    {
        gameServer.Stop();
    }
}

public class GameServer
{
    public const int Port = 2281;
    private UdpClient _socket;

    private static Thread serverThread;

    private const int TickRateValue = 40;
    public readonly int tickRate = TickRateValue / 1000;
    
    private Hashtable existPlayers = new Hashtable();
    private NetPlayer[] _netPlayers = new NetPlayer[0];

    public void Start()
    {
        serverThread = new Thread(new ThreadStart(delegate
        {
            _socket = new UdpClient(Port);

            byte[] data = new byte[1024];

            Debug.Log("Server waiting for a client..");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                data = _socket.Receive(ref sender);
                
                // TODO implement server logick
                {
                    var netPlayer = Utils.ByteArrayToObject(data) as NetPlayer;
                    Debug.Log("From client: Nick:" + netPlayer.nick + " Pos: " + netPlayer.Position + " Rot: " +
                              netPlayer.Rotation);

                    if (existPlayers.ContainsKey(netPlayer.nick))
                    {
                        _netPlayers[(int) existPlayers[netPlayer.nick]] = netPlayer;
                    }
                    else
                    {
                        _netPlayers = new NetPlayer[_netPlayers.Length + 1];
                        existPlayers.Add(netPlayer.nick, _netPlayers.Length - 1);
                    }

                    // Response
                    data = Utils.ObjectToByteArray(_netPlayers);
                    _socket.Send(data, data.Length, sender);
                }
            }
        }));

        serverThread.Start();

        Debug.Log("Server started");
    }

    public void Stop()
    {
        try
        {
            _socket.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        // You must close the udp listener
        try
        {
            _socket.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        serverThread.Abort();

        Debug.Log("Server stopped");
    }
}