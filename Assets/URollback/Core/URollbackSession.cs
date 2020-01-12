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

        public URollbackErrorCode StartSession(int[] players)
        {
            if (sessionStarted)
            {
                return URollbackErrorCode.INVALID_SESSION;
            }
            for(int i = 0; i < players.Length; i++)
            {
                this.players.Add(players[i], new URollbackPlayer(players[i]));
            }
            sessionStarted = true;
            return URollbackErrorCode.OK;
        }

        public void EndSession()
        {
            players.Clear();
            sessionStarted = false;
        }

        public URollbackPlayer AddPlayer(int identifier)
        {
            if (players.ContainsKey(identifier))
            {
                return null;
            }
            players.Add(identifier, new URollbackPlayer(identifier));
            return players[identifier];
        }

        public URollbackPlayer GetPlayer(int connectionId)
        {
            if (!players.ContainsKey(connectionId))
            {
                return null;
            }
            return players[connectionId];
        }
    }
}