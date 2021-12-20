using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Networking;
using UnityEngine;
using Utils;

public class Server : MonoBehaviour
{
    public static NewServer newServer;
    private Thread udpServerThread;

    void Start()
    {
        udpServerThread = new Thread(() => newServer = new NewServer());
        udpServerThread.Start();
    }

    private void OnApplicationQuit()
    {
        newServer.Stop();
        udpServerThread.Abort();
    }
}

public static class BytesExtension
{
    public static byte ChanelID(this byte[] data)
    {
        return data[0];
    }
}


public class NewServer
{
    public const int Port = 2281;

    public NewServer()
    {
        Setup();
    }

    //The ClientInfo structure holds the required information about every
    //client connected to the server
    struct ClientInfo
    {
        public EndPoint endpoint; //Socket of the client
        public string strName; //Name by which the user logged into the chat room
    }

    //The collection of all clients logged into the room (an array of type ClientInfo)
    /// ArrayList clientList = new ArrayList();
    private List<ClientInfo> clientList = new List<ClientInfo>();

    //The main socket on which the server listens to the clients
    Socket serverSocket;

    byte[] byteData = new byte[1500];


    // Game
    private Hashtable existPlayers = new Hashtable();
    private NetPlayer[] _netPlayers = new NetPlayer[0];

    private void Setup()
    {
        try
        {
            //We are using UDP sockets
            serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            //Assign the any IP of the machine and listen on port number 1000
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, Port);

            //Bind this address to the server
            serverSocket.Bind(ipEndPoint);

            IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
            //The epSender identifies the incoming clients
            EndPoint epSender = (EndPoint) ipeSender;

            //Start receiving data
            serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
                SocketFlags.None, ref epSender, OnReceive, epSender);
            Debug.Log("UDP server started");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Server: {ex.Message}");
        }
    }

    private void OnReceive(IAsyncResult ar)
    {
        try
        {
            IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint) ipeSender;

            int bytes = serverSocket.EndReceiveFrom(ar, ref epSender);

            var packet = Packer.UnPack(byteData, bytes);

            switch (packet.chanelID)
            {
                case ChanelID.Login:
                    
                    //When a user logs in to the server then we add her to our
                    //list of clients

                    Login login = Data.ByteArrayToObject(packet.data) as Login;
                    ClientInfo newClientInfo = new ClientInfo
                    {
                        endpoint = epSender,
                        strName = login.nick
                    };
                    clientList.Add(newClientInfo);
                    Debug.Log("Server: LOGIN new player: " + newClientInfo.strName);

                    foreach (ClientInfo clientInfo in clientList)
                    {
                        //Send the message to all users
                        serverSocket.BeginSendTo(byteData, 0, bytes, SocketFlags.None,
                            clientInfo.endpoint,
                            OnSend, clientInfo.endpoint);
                    }

                    break;

                case ChanelID.Logout:
                    
                    //When a user wants to log out of the server then we search for her 
                    //in the list of clients and close the corresponding connection

                    Logout logout = Data.ByteArrayToObject(packet.data) as Logout;
                    existPlayers.Remove(logout.nick);
                    foreach (ClientInfo clientInfo in clientList)
                    {
                        //Send the message to all users
                        serverSocket.BeginSendTo(byteData, 0, bytes, SocketFlags.None,
                            clientInfo.endpoint,
                            OnSend, clientInfo.endpoint);
                    }

                    int nIndex = 0;
                    foreach (ClientInfo client in clientList)
                    {
                        if (client.endpoint == epSender)
                        {
                            Debug.Log("Server: LOGOUT player: " + clientList[nIndex].strName);
                            clientList.RemoveAt(nIndex);
                            break;
                        }
                        ++nIndex;
                    }
                    
                    break;
                case ChanelID.PlayerPosition:
                    
                    var netPlayer = Data.ByteArrayToObject(packet.data) as NetPlayer;

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
                    byteData = Data.ObjectToByteArray(_netPlayers);
                    byteData = Packer.CombinePacket(ChanelID.PlayerPosition, byteData);

                    foreach (ClientInfo clientInfo in clientList)
                    {
                        if (clientInfo.endpoint != epSender ||
                            packet.chanelID != ChanelID.Login)
                        {
                            //Send the message to all users
                            serverSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None,
                                clientInfo.endpoint,
                                OnSend, clientInfo.endpoint);
                        }
                    }

                    break;
                case ChanelID.ChangeWeapon:
                    
                    Debug.Log("смена на сервере");
                    
                    foreach (ClientInfo clientInfo in clientList)
                    {
                        serverSocket.BeginSendTo(byteData, 0, bytes, SocketFlags.None,
                            clientInfo.endpoint,
                            OnSend, clientInfo.endpoint);
                    }

                    break;
                case ChanelID.SpawnDecal:
                    
                    Debug.Log("спавн декали");
                    
                    foreach (ClientInfo clientInfo in clientList)
                    {
                        serverSocket.BeginSendTo(byteData, 0, bytes, SocketFlags.None,
                            clientInfo.endpoint,
                            OnSend, clientInfo.endpoint);
                    }

                    break;
                case ChanelID.Damage:
                    
                    foreach (ClientInfo clientInfo in clientList)
                    {
                        serverSocket.BeginSendTo(byteData, 0, bytes, SocketFlags.None,
                            clientInfo.endpoint,
                            OnSend, clientInfo.endpoint);
                    }

                    break;
                
                case ChanelID.Respawn:
                    foreach (ClientInfo clientInfo in clientList)
                    {
                        serverSocket.BeginSendTo(byteData, 0, bytes, SocketFlags.None,
                        clientInfo.endpoint,
                        OnSend, clientInfo.endpoint);
                    }
                    
                        
                    break;
/*
                    case Command.List:

                        //Send the names of all users in the chat room to the new user
                        msgToSend.cmdCommand = Command.List;
                        msgToSend.strName = null;
                        msgToSend.strMessage = null;

                        //Collect the names of the user in the chat room
                        foreach (ClientInfo client in clientList)
                        {
                            //To keep things simple we use asterisk as the marker to separate the user names
                            msgToSend.strMessage += client.strName + "*";
                        }

                        message = msgToSend.ToByte();

                        //Send the name of the users in the chat room
                        serverSocket.BeginSendTo(message, 0, message.Length, SocketFlags.None, epSender,
                            OnSend, epSender);
                        break;
*/
            }

            /*if (packet.chanelID != ChanelID.PlayerPosition)
            {
                foreach (ClientInfo clientInfo in clientList)
                {
                    if (clientInfo.endpoint != epSender ||
                        packet.chanelID != ChanelID.Login)
                    {
                        //Send the message to all users
                        serverSocket.BeginSendTo(byteData, 0, bytes, SocketFlags.None, clientInfo.endpoint,
                            OnSend, clientInfo.endpoint);
                    }
                }
            }
*/
            //If the user is logging out then we need not listen from her
            if (packet.chanelID != ChanelID.Logout)
            {
                byteData = new byte[Utils.Network.BUFFER_SIZE];
                //Start listening to the message send by the user
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender,
                    OnReceive, epSender);
            }
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"Server: {ex.Message}, {ex.Source}, {ex.StackTrace}, {ex.Data}");
        }
    }

    private void OnSend(IAsyncResult ar)
    {
        try
        {
            serverSocket.EndSend(ar);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Server: {ex.Message}");
        }
    }

    public void Stop()
    {
        try
        {
            serverSocket.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        // You must close the udp listener
        try
        {
            serverSocket.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        Debug.Log("UDP server stopped");
    }
}