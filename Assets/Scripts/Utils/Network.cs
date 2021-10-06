namespace Utils
{
    public static class Network
    {
        public static void SendTcpData(byte[] data)
        {
            Client.Instance.SendTcpData(data);
        }
    }
}