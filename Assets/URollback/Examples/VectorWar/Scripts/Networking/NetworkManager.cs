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
        public delegate void ClientDisconnectServerAction();
        public event ClientDisconnectServerAction OnClientDisconnectServer;

        [Header("References")]
        [SerializeField] private ClientManager clientManagerPrefab;
        [SerializeField] private GameManager gameManager;

        [Header("UI")]
        [SerializeField] private InputField ipField;
        [SerializeField] private GameObject connectionWindow;
        [SerializeField] private GameObject disconnectWindow;

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

        public void Disconnect()
        {
            if (NetworkServer.active)
            {
                StopServer();
            }
            else
            {
                StopClient();
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetupServerNetworkMessages();
            OnServerStarted?.Invoke();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            connectionWindow.SetActive(false);
            disconnectWindow.SetActive(true);
            SetupClientNetworkMessages();
            OnClientStarted?.Invoke();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            connectionWindow.SetActive(true);
            disconnectWindow.SetActive(false);
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
            gameManager.rollbackSession.AddClient(conn.connectionId);
            
            // Tell clients to add this client to the list.
            URollbackSessionClientsMsg clientsMsg = new URollbackSessionClientsMsg(URollbackSessionClientsMsgType.Add, 
                gameManager.rollbackSession.Clients.Values.ToArray());
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

            gameManager.rollbackSession.RemoveClient(conn.connectionId);
            NetworkServer.SendToAll(new URollbackSessionClientsMsg(URollbackSessionClientsMsgType.Remove, new URollbackClient[] { new URollbackClient(conn.connectionId) }));
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Debug.Log("Connected to server.");
            OnClientJoinedServer?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            OnClientDisconnectServer?.Invoke();
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
                        if (gameManager.rollbackSession.HasClient(rsMsg.clients[i].Identifier))
                        {
                            continue;
                        }
                        gameManager.rollbackSession.AddClient(rsMsg.clients[i].Identifier, rsMsg.clients[i]);
                        break;
                    case URollbackSessionClientsMsgType.Remove:
                        gameManager.rollbackSession.RemoveClient(rsMsg.clients[i].Identifier);
                        break;
                }
            }
        }
    }
}