using System.Collections.Generic;
using Mirror;

namespace Network.MatchMaking.Structs
{
    [System.Serializable]
    public class Match
    {
        public string matchID;

        public List<NetworkPlayer> players;
        private NetworkPlayer _hostPlayer;

        public NetworkPlayer HostPlayer
        {
            get { return _hostPlayer; }
            private set { _hostPlayer = value; }
        }
        
        public Match(string matchID, NetworkPlayer hostPlayer)
        {
            this.matchID = matchID;
            players.Add(hostPlayer);

            HostPlayer = hostPlayer;
        }
    }
}