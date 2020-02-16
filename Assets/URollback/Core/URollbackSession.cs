using System.Collections.Generic;
using URollback.Core.Input;

namespace URollback.Core
{
    [System.Serializable]
    public class URollbackSession
    {
        public delegate void ClientAddedAction(int identifier);
        public event ClientAddedAction OnClientAdded;
        public delegate void ClientRemovedAction(int identifier);
        public event ClientRemovedAction OnClientRemoved;

        public delegate void AdvanceLocalInputAction(int identifier);
        public event AdvanceLocalInputAction OnAdvanceLocalInput;

        public bool SessionActive { get { return sessionActive; } }
        public IReadOnlyDictionary<int, URollbackClient> Clients { get { return clients; } }
        public URollbackWorld URollbackWorld { get { return uRollbackWorld; } }
        /// <summary>
        /// Returns the current frame that is being simulation. If we're rolling back,
        /// this returns the frame that the rollback is currently on.
        /// </summary>
        public int CurrentSimulationFrame { get { return inRollback ? currentRollbackFrame : currentFrame; } }
        /// <summary>
        /// Returns the current frame we're on, ignoring what rollback frame
        /// we're on.
        /// </summary>
        public int CurrentFrame { get { return currentFrame; } }

        protected Dictionary<int, URollbackClient> clients = new Dictionary<int, URollbackClient>();
        protected bool sessionActive;
        protected bool inRollback;
        protected int frameDelay;
        protected int currentFrame;
        protected int currentRollbackFrame;

        protected URollbackWorld uRollbackWorld;

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
            ResetSession();
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
            uRollbackWorld = null;
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
            uRollbackWorld = new URollbackWorld();
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
        /// Adds an input for the local client, and adds a 
        /// localFrameLag value for remote clients.
        /// </summary>
        /// <param name="identifier"></param>
        public virtual URollbackErrorCode AddLocalInput(int identifier, ClientInputDefinition clientInput)
        {
            if (!clients.ContainsKey(identifier))
            {
                return URollbackErrorCode.INVALID_CLIENT;
            }

            OnAdvanceLocalInput?.Invoke(identifier);
            URollbackClient localClient = GetClient(identifier);
            localClient.AddInput(clientInput);

            // Calculate how many frames we're predicting for remote client(s). 
            foreach(URollbackClient remoteClient in clients.Values)
            {
                if(remoteClient != localClient)
                {
                    remoteClient.AddLocalFrameLag(localClient.InputFrame);
                }
            }
            return URollbackErrorCode.OK;
        }

        /// <summary>
        /// Advances the input frame coutner for a remote client,
        /// and calculates a remoteFrameLag value for the client.
        /// Call this right after you get the input from a remote client
        /// and have added it to their list of inputs.
        /// </summary>
        /// <param name="localIdentifier"></param>
        /// <param name="identifier"></param>
        public virtual void AddRemoteInput(int localIdentifier, int remoteIdentifier, ClientInputDefinition clientInput)
        {
            URollbackClient localClient = GetClient(localIdentifier);
            URollbackClient remoteClient = GetClient(remoteIdentifier);
            remoteClient.AddInput(clientInput);
            remoteClient.AddRemoteFrameLag(localClient.InputFrame);
        }

        /// <summary>
        /// Get the client with the highest RTT value.
        /// </summary>
        /// <returns></returns>
        public virtual URollbackClient GetHighestRTTClient()
        {
            URollbackClient highestRTTClient = new URollbackClient(-1);
            foreach(URollbackClient client in clients.Values)
            {
                if(client.RTT >= highestRTTClient.RTT)
                {
                    highestRTTClient = client;
                }
            }
            return highestRTTClient;
        }

        /// <summary>
        /// Advances the rollback state forward. 
        /// </summary>
        public virtual void AdvanceState()
        {

        }
    }
}