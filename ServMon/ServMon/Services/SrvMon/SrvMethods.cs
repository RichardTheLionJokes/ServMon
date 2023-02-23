using ServMon.Models;
using System.Net.NetworkInformation;

namespace ServMon.Services.SrvMon
{
    public static class SrvMethods
    {
        public static bool AddressIsAvailable(string address, int _timeout = 1000)
        {

            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(address, _timeout);

                if (reply.Status == IPStatus.Success) return true;
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
