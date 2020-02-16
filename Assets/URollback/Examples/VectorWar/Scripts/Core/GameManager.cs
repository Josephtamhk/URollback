using System.Collections.Generic;
using UnityEngine;
using URollback.Core;
using Mirror;
using System;
using System.Threading.Tasks;

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
            networkManager.OnClientStarted += ClientHandlers;
            // Whenever a client joins the server/the server starts up,
            // a rollback session should be started.
            networkManager.OnServerStarted += () => { rollbackSession.StartSession(); };
            networkManager.OnClientJoinedServer += () => { rollbackSession.StartSession(); };
            // Whenver we leave the server, the rollback session should be ended.
            networkManager.OnClientDisconnectServer += () => { rollbackSession.EndSession(); };
        }

        private void ClientHandlers()
        {
            NetworkClient.RegisterHandler<InitMatchMsg>(InitializeMatch);
            NetworkClient.RegisterHandler<StartMatchMsg>(StartMatch);
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
            // Tell all clients to initialize match with the parameters passed.
            NetworkServer.SendToAll(new InitMatchMsg());

            // Spawn the client players and get the longest delayed client time.
            int spawnPosCounter = 0;
            double longestDelay = 0;
            foreach (NetworkConnectionToClient client in NetworkServer.connections.Values)
            {
                Debug.Log($"Spawning for {client.connectionId}");
                ClientManager clientManager = client.identity.GetComponent<ClientManager>();
                clientManager.ServerSpawnPlayers(spawnPosCounter);
                spawnPosCounter++;

                if (clientManager.ClientRTT > longestDelay)
                {
                    longestDelay = clientManager.ClientRTT;
                }
            }

            // Start the match. Each client needs to be delayed
            // depending on the longest delayed client.
            foreach(NetworkConnectionToClient client in NetworkServer.connections.Values)
            {
                ClientManager clientManager = client.identity.GetComponent<ClientManager>();
                client.Send(new StartMatchMsg((longestDelay - clientManager.ClientRTT) / 2.0));
            }
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

        /// <summary>
        /// Called from the server to all clients to start the match.
        /// We delay before we start the game to make sure all clients are
        /// relatively in sync.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private async void StartMatch(NetworkConnection conn, StartMatchMsg msg)
        {
            await Task.Delay(TimeSpan.FromSeconds(msg.delay));
            matchManager.StartMatch();
            Debug.Log("Match started.");
        }

        private void Update()
        {
            if (matchManager != null && matchManager.MatchStarted)
            {
                matchManager.Update();
            }
        }
    }
}