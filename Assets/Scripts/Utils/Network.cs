namespace Utils
{
    public static class Network
    {
        public static void SendUdpData(byte[] data)
        {
            Client.Instance.SendUdpData(data);
        }
        
        public const int BUFFER_SIZE = 2048;
    }
}