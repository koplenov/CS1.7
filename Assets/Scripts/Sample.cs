using System;
using UnityEngine;

public class Sample : MonoBehaviour
{
    #region ID
    class ID
    {
        public const byte Position = 0;
        public const byte Message = 1;
    }
    #endregion

    void ExampleForLeha()
    {
        NetPlayer netPlayer = new NetPlayer("koplenov");

        // content bytes
        byte[] testByted = Utils.ObjectToByteArray(netPlayer);

        // bytes to send or bytes from server
        byte[] chanalledBytes = Packer.CombinePacket(ID.Position, testByted);

        byte[] clearData = new ArraySegment<byte>(chanalledBytes, 1, chanalledBytes.Length).Array;

        switch (chanalledBytes[0])
        {
            case ID.Message:
                // todo
                break;
            case ID.Position:
                // todo
                break;
        }
    }
}
