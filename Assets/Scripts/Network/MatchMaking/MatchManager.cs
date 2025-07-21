using Mirror;
using Network;
using Network.MatchMaking.Structs;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    [SyncVar] public SyncList<Match> _matches;

    [Server]
    public void CreateMatch(NetworkPlayer sender)
    {
        _matches.Add(new Match(GetMatchID(), sender));
    }

    private string GetMatchID(int lenght = 5)
    {
        string matchID = "";
        
        while (true)
        {
            matchID = "";

            for (int i = 0; i < lenght; i++)
            {
                int k = Random.Range(0, 35);
            
                if (k <= 9)
                {
                    matchID += (char) (30 + k);
                }
                else
                {
                    matchID += (char) (41 + (k - 10));
                }
            }

            bool success = true;
            foreach (Match _match in _matches)
            {
                if (_match.matchID == matchID)
                {
                    success = false;
                    break;
                }
            }
            
            if(success) break;
        }

        return matchID;
    }
}
