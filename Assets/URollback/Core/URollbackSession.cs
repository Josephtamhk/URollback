using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Core
{
    [System.Serializable]
    public class URollbackSession
    {
        public bool SessionActive { get { return sessionActive; } }

        private Dictionary<int, URollbackClient> clients = new Dictionary<int, URollbackClient>();
        private bool sessionActive;
        private int frameDelay;

        /// <summary>
        /// Activates a session.
        /// This should be called before gameplay starts, such as while in a lobby.
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public URollbackErrorCode ActivateSession()
        {
            if (sessionActive)
            {
                return URollbackErrorCode.INVALID_SESSION;
            }
            sessionActive = true;
            return URollbackErrorCode.OK;
        }

        /// <summary>
        /// Starts a rollback session.
        /// This should be called when gameplay actually starts, ie. the match begins.
        /// </summary>
        /// <returns></returns>
        public URollbackErrorCode StartSession()
        {
            return URollbackErrorCode.OK;
        }

        /// <summary>
        /// Ends a rollback session.
        /// This should be called when gameplay ends.
        /// </summary>
        /// <returns></returns>
        public URollbackErrorCode EndSession()
        {
            return URollbackErrorCode.OK;
        }

        /// <summary>
        /// Deactivates the session.
        /// This should be called when the client disconnects from the server, or when the host stops the server.
        /// </summary>
        public void DeactivateSession()
        {
            clients.Clear();
            sessionActive = false;
        }

        /// <summary>
        /// Adds a client to the session. 
        /// Returns null if the identifier is being used.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public URollbackClient AddClient(int identifier)
        {
            return AddClient(identifier, new URollbackClient(identifier));
        }

        public URollbackClient AddClient(int identifier, URollbackClient client)
        {
            if (clients.ContainsKey(identifier))
            {
                return null;
            }
            clients.Add(identifier, client);
            return clients[identifier];
        }

        /// <summary>
        /// Removes a client from the session.
        /// If the 
        /// </summary>
        /// <param name="identifier"></param>
        public void RemoveClient(int identifier)
        {
            clients.Remove(identifier);
        }

        /// <summary>
        /// Gets a client that is in the session. 
        /// Returns null if no client exist for the identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public URollbackClient GetClient(int identifier)
        {
            if (!clients.ContainsKey(identifier))
            {
                return null;
            }
            return clients[identifier];
        }

        public bool HasClient(int identifier)
        {
            return clients.ContainsKey(identifier);
        }
    }
}