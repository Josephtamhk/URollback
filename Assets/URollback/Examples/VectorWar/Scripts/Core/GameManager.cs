using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URollback.Core;
using Mirror;
using System;

namespace URollback.Examples.VectorWar
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public MatchManager MatchManager { get { return matchManager; } }

        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private MatchManager matchManager;

        public URollbackSession rollbackSession = new URollbackSession();

        public List<Vector2> spawnPositions = new List<Vector2>();
        public bool autoStartMatch;

        private void Awake()
        {
            instance = this;
            rollbackSession.OnClientAdded += AutoStartMatch;
            networkManager.OnClientStarted += () => { NetworkClient.RegisterHandler<InitMatchMsg>(InitializeMatch); };
            // Whenever a client joins the server/the server starts up,
            // a rollback session should be started.
            networkManager.OnServerStarted += () => { rollbackSession.StartSession(); };
            networkManager.OnClientJoinedServer += () => { rollbackSession.StartSession(); };
            // Whenver we leave the server, the rollback session should be ended.
            networkManager.OnClientDisconnectServer += () => { rollbackSession.EndSession(); };
        }

        /// <summary>
        /// Whenever a client joins, we check if we
        /// should automatically start the match.
        /// </summary>
        /// <param name="identifier"></param>
        private void AutoStartMatch(int identifier)
        {
            if (!autoStartMatch || !NetworkServer.active)
            {
                return;
            }
            if(rollbackSession.Clients.Count >= networkManager.maxConnections)
            {
                ServerStartMatch();
            }
        }

        /// <summary>
        /// Called on the server when we want the match to start with the current players.
        /// </summary>
        public void ServerStartMatch()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            // Tell all clients to start a match with the parameters passed.
            NetworkServer.SendToAll(new InitMatchMsg());
            // Spawn the client players.
            int spawnPosCounter = 0;
            foreach(NetworkConnectionToClient client in NetworkServer.connections.Values)
            {
                Debug.Log($"Spawning for {client.connectionId}");
                ClientManager clientManager = client.identity.GetComponent<ClientManager>();
                clientManager.ServerSpawnPlayers(spawnPosCounter);
                spawnPosCounter++;
            }
            // Start the match.
        }

        /// <summary>
        /// Called from the server to all clients during the match starting process.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void InitializeMatch(NetworkConnection conn, InitMatchMsg msg)
        {
            matchManager = new MatchManager(this, networkManager);
        }
    }
}