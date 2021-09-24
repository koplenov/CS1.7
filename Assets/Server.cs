using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SendMsg("127.0.0.1");
        }
    }

    //ref
    private void SendMsg(string remoteAddr)
    {
        UdpClient sender = new UdpClient(); // create an UdpClient to send messages
        try
        {
            string message = "test"; // sending message
            byte[] data = Encoding.Unicode.GetBytes(message);
            sender.Send(data, data.Length, remoteAddr, GameServer.Port); // send
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            sender.Close();
        }

        Debug.Log("sended");
    }
}

public class GameServer
{
    public const int Port = 2281;
    private UdpClient _socket;
    
    private static Thread serverThread;

    private const int TickRateValue = 40;
    public readonly int tickRate = TickRateValue / 1000;

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

                Debug.Log("From client: " + Encoding.Unicode.GetString(data, 0, data.Length));

                _socket.Send(data, data.Length, sender);
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