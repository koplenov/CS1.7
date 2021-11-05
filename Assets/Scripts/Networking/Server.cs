using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Networking;
using UnityEngine;
using Utils;
using Network = Utils.Network;

public class Server : MonoBehaviour
{
    public static UdpServer udpServer = new UdpServer();
    public static TcpServer tcpServer = new TcpServer();

    void Start()
    {
        udpServer.Start();
        tcpServer.Start();
    }

    private void OnApplicationQuit()
    {
        udpServer.Stop();
        tcpServer.Stop();
    }
}

public class UdpServer
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

            Debug.Log("UDP server waiting for a client..");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                data = _socket.Receive(ref sender);

                // TODO implement server logick
                {
                    var netPlayer = Data.ByteArrayToObject(data) as NetPlayer;
                    /*
                     Debug.Log("From client: Nick:" + netPlayer.nick + " Pos: " + netPlayer.Position + " Rot: " +
                            netPlayer.Rotation);
                    */
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
                    data = Data.ObjectToByteArray(_netPlayers);
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

        Debug.Log("UDP server stopped");
    }
}

public class TcpServer
{
    //private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static Socket serverSocket;

    private static readonly List<Socket> clientSockets = new List<Socket>();
    private const int BUFFER_SIZE = 2048;
    private const int PORT = UdpServer.Port;
    private static readonly byte[] buffer = new byte[BUFFER_SIZE];

    public void Start()
    {
        SetupServer();
    }

    public void Stop()
    {
        CloseAllSockets();
        Debug.Log("TCP server stopped"); //завершение процесса
    }

    private static void SetupServer()
    {
        Debug.Log("Setting up TCP server...");
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
        serverSocket.Listen(0);
        serverSocket.BeginAccept(AcceptCallback, null);
        Debug.Log("TCP server setup complete");
    }

    /// <summary>
    /// Close all connected client (we do not need to shutdown the server socket as its connections
    /// are already closed with the clients).
    /// </summary>
    private static void CloseAllSockets()
    {
        foreach (Socket socket in clientSockets)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                socket.Close();
            }
        }

        try
        {
            serverSocket.Shutdown(SocketShutdown.Both);
        }
        catch (SocketException)
        {
        }
        finally
        {
            serverSocket.Close();
        }
    }

    private static readonly Hashtable connections = new Hashtable();
    private static void AcceptCallback(IAsyncResult AR)
    {
        Socket socket;

        try
        {
            socket = serverSocket.EndAccept(AR);
        }
        catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
        {
            return;
        }
        
        clientSockets.Add(socket);
        socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
        Debug.Log("Client connected, waiting for request...");
        serverSocket.BeginAccept(AcceptCallback, null);
    }

    private static void ReceiveCallback(IAsyncResult AR)
    {
        Socket current = (Socket) AR.AsyncState;
        int received;
        
        try
        {
            received = current.EndReceive(AR);
        }
        catch (SocketException)
        {
            Debug.Log("Client forcefully disconnected");
            // Don't shutdown because the socket may be disposed and its disconnected anyway.
            current.Close();
            clientSockets.Remove(current);
            return;
        }

        byte[] recBuf = new byte[received];
        Array.Copy(buffer, recBuf, received);
        
        //#

        try
        {
            switch (recBuf.ChanelID())
            {
                case ChanelID.ChangeWeapon:
                    Debug.Log("смена на сервере");
                    //current.Send(recBuf);
                    foreach (var connect in clientSockets)
                    {
                        connect.Send(recBuf);
                    }
                    break;
                case ChanelID.SpawnDecal:
                    Debug.Log("спавн декали");
                    //current.Send(recBuf);
                    foreach (var connect in clientSockets)
                    {
                        connect.Send(recBuf);
                    }
                    break;
                case ChanelID.Damage:
                    foreach (var connect in clientSockets)
                    {
                        connect.Send(recBuf);
                    }
                    break;
                default:
                    string text = Encoding.Unicode.GetString(recBuf);
                    Debug.Log("Received Text: " + text);

                    if (text.ToLower() == "get time") // Client requested time
                    {
                        Console.WriteLine("Text is a get time request");
                        byte[] data = Encoding.Unicode.GetBytes(DateTime.Now.ToLongTimeString());
                        current.Send(data);
                        Console.WriteLine("Time sent to client");
                    }
                    else if (text.ToLower() == "exit") // Client wants to exit gracefully
                    {
                        // Always Shutdown before closing
                        current.Shutdown(SocketShutdown.Both);
                        current.Close();
                        clientSockets.Remove(current);
                        Console.WriteLine("Client disconnected");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Text is an invalid request");
                        byte[] data = Encoding.Unicode.GetBytes("Invalid request");
                        current.Send(data);
                        Console.WriteLine("Warning Sent");
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            //throw;
        }

        //#
        current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
    }
}

public static class BytesExtension
{
    public static byte ChanelID(this byte[] data)
    {
        return data[0];
    }
} 