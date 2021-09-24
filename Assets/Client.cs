using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] private string nick;
    private IPEndPoint _remoteAddr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), GameServer.Port);
    private UdpClient _sender; 

    private void Awake()
    {
        nick = PlayerPrefs.GetString("nick", Environment.UserName);
    }

    private static Thread _listeningThread;
    
    private void Start()
    {
        _sender = new UdpClient();
        _sender.Connect(_remoteAddr);
        
        _listeningThread = new Thread(new ThreadStart(delegate
        {
            byte[] data = new byte[1024];

            while (true)
            {
                data = _sender.Receive(ref _remoteAddr);
                
                // TODO implement game logick
                
                Debug.Log("From server: " + Encoding.Unicode.GetString(data, 0, data.Length));
            }
        }));
        
        _listeningThread.Start();
        
        Debug.Log("Client started");
    }
    
    private void Update()
    {
        string message = "test"; // sending message
        byte[] data = Encoding.Unicode.GetBytes(message);
        _sender.Send(data, data.Length); // send
        
        // TODO implement client logick (send position, etc)
    }

    public void OnApplicationQuit()
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
