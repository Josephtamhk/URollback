using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using URollback.Core;
using System.Linq;

namespace URollback.Examples.VectorWar
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public static NetworkManager instance;

        public delegate void ServerStartedAction();
        public event ServerStartedAction OnServerStarted;
        public delegate void ClientStartedAction();
        public event ClientStartedAction OnClientStarted;
        public delegate void ServerClientJoinAction();
        public event ServerClientJoinAction ServerOnClientJoined;
        public delegate void ClientJoinedServerAction();
        public event ClientJoinedServerAction OnClientJoinedServer;

        public URollbackSession rollbackSession  = new URollbackSession();

        [Header("References")]
        [SerializeField] private ClientManager clientManagerPrefab;
        [SerializeField] private GameManager gameManager;

        [Header("UI")]
        [SerializeField] private InputField ipField;

        public override void Awake()
        {
            base.Awake();
            instance = this;
        }

        public void StartHosting(int playerCount)
        {
            Debug.Log($"Starting host for {playerCount} players.");
            maxConnections = playerCount;
            StartHost();
        }

        public void StartServer(int playerCount)
        {
            Debug.Log($"Starting server for {playerCount} players.");
            maxConnections = playerCount;
            StartServer();
        }

        /// <summary>
        /// Called by the "Connect" UI button.
        /// </summary>
        public void ConnectToServer()
        {
            ConnectToServer(ipField.text);
        }

        public void ConnectToServer(string ip)
        {
            Debug.Log($"Trying to connect to {ip}.");
            networkAddress = ip;
            StartClient();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetupServerNetworkMessages();
            rollbackSession.StartSession();
            OnServerStarted?.Invoke();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            SetupClientNetworkMessages();
            OnClientStarted?.Invoke();
        }

        /// <summary>
        /// Whenever a client connects to the server, 
        /// we want to add them to the rollback session and tell all clients to do the same.
        /// </summary>
        /// <param name="conn"></param>
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            Debug.Log($"Client {conn.connectionId} connected to server.");

            GameObject clientManager = GameObject.Instantiate(clientManagerPrefab.gameObject, Vector3.zero, Quaternion.identity);
            clientManager.GetComponent<ClientManager>().connectionID = conn.connectionId;
            NetworkServer.AddPlayerForConnection(conn, clientManager);
            rollbackSession.AddClient(conn.connectionId);
            
            // Tell clients to add this client to the list.
            URollbackSessionClientsMsg clientsMsg = new URollbackSessionClientsMsg(URollbackSessionClientsMsgType.Add, rollbackSession.Clients.Values.ToArray());
            NetworkServer.SendToAll(clientsMsg);
            ServerOnClientJoined?.Invoke();
        }

        /// <summary>
        /// Whenever a client disconnects from the server, 
        /// we want to remove them from the rollback session and tell all clients to do the same.
        /// </summary>
        /// <param name="conn"></param>
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log($"Client {conn.connectionId} disconnected from server.");

            rollbackSession.RemoveClient(conn.connectionId);
            NetworkServer.SendToAll(new URollbackSessionClientsMsg(URollbackSessionClientsMsgType.Remove, new URollbackClient[] { new URollbackClient(conn.connectionId) }));
        }

        /// <summary>
        /// Whenever the client connects to the server,
        /// they need to start the rollback session.
        /// </summary>
        /// <param name="conn"></param>
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Debug.Log("Connected to server.");
            rollbackSession.StartSession();
            OnClientJoinedServer?.Invoke();
        }

        /// <summary>
        /// Whenever the client disconnects from the server,
        /// you should end the rollback session.
        /// </summary>
        /// <param name="conn"></param>
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            rollbackSession.EndSession();
        }

        public void SetupServerNetworkMessages()
        {

        }

        public void SetupClientNetworkMessages()
        {
            NetworkClient.RegisterHandler<URollbackSessionClientsMsg>(OnRollbackSessionClientsMsg);
        }


        /// <summary>
        /// Sent from server->clients whenever clients join or leave. 
        /// This updates each client's list of clients in the session.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="rsMsg"></param>
        public void OnRollbackSessionClientsMsg(NetworkConnection conn, URollbackSessionClientsMsg rsMsg)
        {
            // Don't do this on the server (or host), as they sent the message.
            if (NetworkServer.active)
            {
                return;
            }

            for (int i = 0; i < rsMsg.clients.Length; i++)
            {
                switch (rsMsg.msgType)
                {
                    case URollbackSessionClientsMsgType.Add:
                        if (rollbackSession.HasClient(rsMsg.clients[i].Identifier))
                        {
                            continue;
                        }
                        rollbackSession.AddClient(rsMsg.clients[i].Identifier, rsMsg.clients[i]);
                        break;
                    case URollbackSessionClientsMsgType.Remove:
                        rollbackSession.RemoveClient(rsMsg.clients[i].Identifier);
                        break;
                }
            }
        }
    }
}