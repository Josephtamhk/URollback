using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Core
{
    public class URollbackSession
    {
        public bool SessionStarted { get { return sessionStarted; } }

        private Dictionary<int, URollbackPlayer> players = new Dictionary<int, URollbackPlayer>();
        private bool sessionStarted;
        private int frameDelay;

        public URollbackErrorCode StartSession(NetworkConnection[] players)
        {
            if (sessionStarted)
            {
                return URollbackErrorCode.INVALID_SESSION;
            }
            for(int i = 0; i < players.Length; i++)
            {
                this.players.Add(players[i].connectionId, new URollbackPlayer(players[i]));
            }
            sessionStarted = true;
            return URollbackErrorCode.OK;
        }

        public void EndSession()
        {
            players.Clear();
            sessionStarted = false;
        }

        public int AddPlayer(NetworkConnection networkConnection)
        {
            players.Add(networkConnection.connectionId, new URollbackPlayer(networkConnection));
            return players.Count-1;
        }

        public URollbackPlayer GetPlayer(int connectionId)
        {
            if (!players.ContainsKey(connectionId))
            {
                return null;
            }
            return players[connectionId];
        }

        public void AdvanceFrame()
        {

        }
    }
}