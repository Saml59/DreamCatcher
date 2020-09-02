using System;
using Android.Net;

namespace DC.Droid.Data
{
    public class NCB : ConnectivityManager.NetworkCallback
    {
        public bool connected { get; set; }

        public override void OnAvailable(Network network)
        {
            base.OnAvailable(network);
            connected = true;
        }
        public override void OnLost(Network network)
        {
            connected = false;
        }
    }
}
