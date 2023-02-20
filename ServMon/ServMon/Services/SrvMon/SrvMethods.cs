using ServMon.Models;
using System.Net.NetworkInformation;

namespace ServMon.Services.SrvMon
{
    public static class SrvMethods
    {
        public static bool AddressIsAvailable(string name = "", string ip = "", int _timeout = 1000)
        {

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(ip)) return false;

            try
            {
                string address = !string.IsNullOrEmpty(name) ? name : ip;
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(address, _timeout);

                if (reply.Status == IPStatus.Success) return true;
                else
                {
                    if (String.Compare(address, name) == 0 && !string.IsNullOrEmpty(ip))
                    {
                        reply = pingSender.Send(ip, _timeout);
                        if (reply.Status == IPStatus.Success) return true;
                        else return false;
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
