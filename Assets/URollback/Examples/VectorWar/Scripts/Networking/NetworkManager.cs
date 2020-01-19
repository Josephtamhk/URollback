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

        public URollbackSession rollbackSession  = new URollbackSession();

        public bool autoStartGame;
        public ClientManager clientManagerPrefab;
        public GameManager gameManager;

        [Header("UI")]
        [SerializeField] private InputField ipField;

        public override void Awake()
        {
            base.Awake();
            instance = this;
        }

        #region Connecting
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
            autoStartGame = true;
            StartServer();
        }

        public void ConnectToServer()
        {
            ConnectToServer(ipField.text);
        }

        public void ConnectToServer(string ip)
        {
            Debug.Log($"Trying server connection to {ip}.");
            networkAddress = ip;
            StartClient();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetupServerNetworkMessages();
            rollbackSession.ActivateSession();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            SetupClientNetworkMessages();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            Debug.Log($"Client {conn.connectionId} connected to server.");

            GameObject clientManager = GameObject.Instantiate(clientManagerPrefab.gameObject, Vector3.zero, Quaternion.identity);
            clientManager.GetComponent<ClientManager>().connectionID = conn.connectionId;
            NetworkServer.AddPlayerForConnection(conn, clientManager);
            rollbackSession.AddClient(conn.connectionId);
            // Send the client information on all connected clients.
            URollbackSessionClientsMsg clientsMsg = new URollbackSessionClientsMsg(URollbackSessionClientsMsgType.Add, rollbackSession.Clients.Values.ToArray());
            NetworkServer.SendToAll(clientsMsg);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log($"Client {conn.connectionId} disconnected from server.");

            rollbackSession.RemoveClient(conn.connectionId);
            NetworkServer.SendToAll(new URollbackSessionClientsMsg(URollbackSessionClientsMsgType.Remove, new URollbackClient[] { new URollbackClient(conn.connectionId) }));
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            rollbackSession.ActivateSession();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            rollbackSession.DeactivateSession();
        }
        #endregion

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
            // Ignore the host, since he is the server.
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