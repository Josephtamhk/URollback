using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Core
{
    public class URollbackSession
    {
        public bool SessionStarted { get { return sessionStarted; } }

        private Dictionary<int, URollbackClient> clients = new Dictionary<int, URollbackClient>();
        private bool sessionStarted;
        private int frameDelay;

        /// <summary>
        /// Initializes a rollback session.  
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public URollbackErrorCode InitSession(int[] clients)
        {
            if (sessionStarted)
            {
                return URollbackErrorCode.INVALID_SESSION;
            }
            for(int i = 0; i < clients.Length; i++)
            {
                this.clients.Add(clients[i], new URollbackClient(clients[i]));
            }
            sessionStarted = true;
            return URollbackErrorCode.OK;
        }

        public void EndSession()
        {
            clients.Clear();
            sessionStarted = false;
        }

        /// <summary>
        /// Adds a client to the session. 
        /// Returns null if the identifier is being used.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public URollbackClient AddClient(int identifier)
        {
            if (clients.ContainsKey(identifier))
            {
                return null;
            }
            clients.Add(identifier, new URollbackClient(identifier));
            return clients[identifier];
        }

        /// <summary>
        /// Gets a client that is in the session. 
        /// Returns null if no client exist for the identifier.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public URollbackClient GetClient(int identifier)
        {
            if (!clients.ContainsKey(identifier))
            {
                return null;
            }
            return clients[identifier];
        }
    }
}