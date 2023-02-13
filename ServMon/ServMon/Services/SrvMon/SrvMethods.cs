using System.Net.NetworkInformation;

namespace ServMon.Services.SrvMon
{
    public static class SrvMethods
    {
        public static bool AddressIsAvailable(string address, int _timeout)
        {
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(address, _timeout);

                if (reply.Status == IPStatus.Success)
                { return true; }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
