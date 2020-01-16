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

        private void Awake()
        {
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

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            Debug.Log($"Client {conn.connectionId} connected to server.");

            GameObject clientManager = GameObject.Instantiate(clientManagerPrefab.gameObject, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, clientManager);
        }
        #endregion
    }
}