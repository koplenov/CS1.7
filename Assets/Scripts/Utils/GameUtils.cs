using Networking;

namespace Utils
{
    public static class GameUtils
    {
        public static void SendDamage(string analDamager, string anal, int damage)
        {
            SendDamage netPlayer = new SendDamage(analDamager, anal, damage);
            
            // content bytes
            byte[] testByted = Data.ObjectToByteArray(netPlayer);

            // bytes to send or bytes from server
            byte[] packetBytes = Packer.CombinePacket(ChanelID.Damage, testByted);
            
            // simple send bytes to server
            Network.SendUdpData(packetBytes);
        }
    }
}