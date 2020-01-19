using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Core
{
    [System.Serializable]
    public class URollbackSession
    {
        public delegate void ClientAddedAction(int identifier);
        public event ClientAddedAction OnClientAdded;
        public delegate void ClientRemovedAction(int identifier);
        public event ClientRemovedAction OnClientRemoved;

        public bool SessionActive { get { return sessionActive; } }
        public IReadOnlyDictionary<int, URollbackClient> Clients { get { return clients; } }

        protected Dictionary<int, URollbackClient> clients = new Dictionary<int, URollbackClient>();
        protected bool sessionActive;
        protected int frameDelay;
        protected int currentFrame;

        /// <summary>
        /// Activates a session.
        /// This should be called by clients whenever they start hosting or join a server.
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public virtual URollbackErrorCode StartSession()
        {
            if (sessionActive)
            {
                return URollbackErrorCode.INVALID_SESSION;
            }
            currentFrame = 0;
            sessionActive = true;
            return URollbackErrorCode.OK;
        }

        /// <summary>
        /// Deactivates the session.
        /// This should be called by clients when they disconnect,
        /// and by the server when it shuts down.
        /// </summary>
        public virtual void EndSession()
        {
            clients.Clear();
            sessionActive = false;
        }


        /// <summary>
        /// Resets the session.
        /// This should be called by clients when a match ends
        /// and you plan on starting a new one, such as returning to lobby.
        /// </summary>
        public virtual void ResetSession()
        {
            currentFrame = 0;
        }

        /// <summary>
        /// Adds a client to the session. 
        /// Returns null if the identifier is being used.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public virtual URollbackClient AddClient(int identifier)
        {
            return AddClient(identifier, new URollbackClient(identifier));
        }

        /// <summary>
        /// Adds a client to the session.
        /// Returns null if the identifier is being used.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public virtual URollbackClient AddClient(int identifier, URollbackClient client)
        {
            if (clients.ContainsKey(identifier))
            {
                return null;
            }
            clients.Add(identifier, client);
            OnClientAdded?.Invoke(identifier);
            return clients[identifier];
        }

        /// <summary>
        /// Removes a client from the session.
        /// This should be called whenever a client disconnects.
        /// </summary>
        /// <param name="identifier"></param>
        public virtual void RemoveClient(int identifier)
        {
            clients.Remove(identifier);
            OnClientRemoved?.Invoke(identifier);
        }

        /// <summary>
        /// Gets a client that is in the session. 
        /// Returns null if no client exist for the identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public virtual URollbackClient GetClient(int identifier)
        {
            if (!clients.ContainsKey(identifier))
            {
                return null;
            }
            return clients[identifier];
        }

        public virtual bool HasClient(int identifier)
        {
            return clients.ContainsKey(identifier);
        }

        /// <summary>
        /// Advanced the currentFrame timer.
        /// This should be called after a frame has been finished
        /// but before you start the next.
        /// </summary>
        /// <returns></returns>
        public virtual URollbackErrorCode AdvanceFrame()
        {
            if (!sessionActive)
            {
                return URollbackErrorCode.INVALID_SESSION;
            }
            currentFrame++;
            return URollbackErrorCode.OK;
        }

        /// <summary>
        /// Advances the input frame counter for the local client,
        /// and adds a localFrameLag value for remote clients.
        /// Call this right after you add a frame's input to 
        /// your character's list of inputs.
        /// </summary>
        /// <param name="identifier"></param>
        public virtual void AdvanceLocalInput(int identifier)
        {
            URollbackClient localClient = GetClient(identifier);
            localClient.AdvanceLocalInputFrame();

            foreach(URollbackClient remoteClient in clients.Values)
            {
                if(remoteClient != localClient)
                {
                    remoteClient.AddLocalFrameLag(localClient.InputFrame - remoteClient.InputFrame);
                }
            }
        }

        public virtual void AdvanceRemoteInput(int localIdentifier, int identifier)
        {
            URollbackClient localClient = GetClient(localIdentifier);
            URollbackClient remoteClient = GetClient(identifier);
            remoteClient.AdvanceRemoteInputFrame(localClient.InputFrame);
        }
    }
}