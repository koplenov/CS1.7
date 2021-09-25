public class Packer
{
    private const byte defaultChanelID = 0;
    private const byte defaultValidID = 0xAF; //175

    public static byte[] CombinePacket(byte chanelID, byte[] content, byte validID)
    {
        byte[] outBytes = new byte[content.Length + 2];
        content.CopyTo(outBytes, 1);
        outBytes[0] = chanelID;
        outBytes[outBytes.Length - 1] = validID;
        return outBytes;
    }

    public static byte[] CombinePacket(byte chanelID, byte[] content)
    {
        byte[] outBytes = new byte[content.Length + 2];
        content.CopyTo(outBytes, 1);
        outBytes[0] = chanelID;
        outBytes[outBytes.Length - 1] = defaultValidID;
        return outBytes;
    }

    public static byte[] CombinePacket(byte[] content)
    {
        byte[] outBytes = new byte[content.Length + 2];
        content.CopyTo(outBytes, 1);
        outBytes[0] = defaultChanelID;
        outBytes[outBytes.Length - 1] = defaultValidID;
        return outBytes;
    }
}