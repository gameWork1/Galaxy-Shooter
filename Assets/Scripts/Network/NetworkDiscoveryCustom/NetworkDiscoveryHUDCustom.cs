using System.Collections.Generic;
using Mirror;
using Mirror.Discovery;

namespace Network.NetworkDiscoveryCustom
{
    public class NetworkDiscoveryHUDCustom : NetworkDiscoveryHUD
    {
        public Dictionary<long, ServerResponse> DiscoveredServers
        {
            get
            {
                return discoveredServers;
            }
        }

        public void ClearDiscoveryServers()
        {
            discoveredServers.Clear();
        }


        void OnGUI()
        {
            return;
        }
    }
}