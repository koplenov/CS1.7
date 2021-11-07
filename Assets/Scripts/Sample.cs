using Networking;
using UnityEngine;
using Utils;
using Network = Utils.Network;

public class Sample : MonoBehaviour
{
    void ExampleForLeha()
    {
        NetPlayer netPlayer = new NetPlayer("koplenov");

        // content bytes
        byte[] testByted = Data.ObjectToByteArray(netPlayer);

        // bytes to send or bytes from server
        byte[] packetBytes = Packer.CombinePacket(ChanelID.PlayerPosition, testByted);
        
        var unpackedBytes = Packer.UnPack(packetBytes);
        int chanelID = unpackedBytes.chanelID;
        byte[] data = unpackedBytes.data;
        
        switch (unpackedBytes.chanelID)
        {
            case ChanelID.PlayerPosition:
                // todo
                break;
            case ChanelID.SpawnDecal:
                // todo
                break;
        }
        
        // simple send bytes to server
        Network.SendUdpData(packetBytes);
    }
}
