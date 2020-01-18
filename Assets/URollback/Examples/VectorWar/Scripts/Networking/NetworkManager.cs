using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public static NetworkManager instance;

        public URollbackSession rollbackSession;

        public bool autoStartGame;
        public ClientManager clientManagerPrefab;
        public GameManager gameManager;

        public override void Awake()
        {
            base.Awake();
            instance = this;
        }

        #region Connecting
        public void StartHosting(int playerCount)
        {
            Debug.Log($"Starting host for {playerCount} players.");
            maxConnections = playerCount - 1;
            StartHost();
        }

        public void StartServer(int playerCount)
        {
            Debug.Log($"Starting server for {playerCount} players.");
            maxConnections = playerCount;
            autoStartGame = true;
            StartServer();
        }

        public void ConnectToServer(string ip)
        {
            Debug.Log("Trying server connection.");
            networkAddress = ip;
            StartClient();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetupServerNetworkMessages();
            rollbackSession = new URollbackSession();
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
            URollbackClient rClient = rollbackSession.AddClient(conn.connectionId);
            NetworkServer.SendToAll(new URollbackSessionClientsMsg(URollbackSessionClientsMsgType.Add, new URollbackClient[] { rClient }));
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Debug.Log($"Connected to server.");
            rollbackSession.ActivateSession();
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
            for (int i = 0; i < rsMsg.clients.Count; i++)
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