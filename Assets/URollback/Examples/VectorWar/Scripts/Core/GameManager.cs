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

        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private MatchManager matchManager;

        public List<Vector2> spawnPositions = new List<Vector2>();
        public bool autoStartMatch;

        private void Awake()
        {
            instance = this;
            networkManager.rollbackSession.OnClientAdded += AutoStartMatch;
            networkManager.OnClientStarted += () => { NetworkClient.RegisterHandler<InitMatchMsg>(InitializeMatch); };
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
            if(networkManager.rollbackSession.Clients.Count >= networkManager.maxConnections)
            {
                ServerStartMatch();
            }
        }

        /// <summary>
        /// Called on the server when we want the match to start
        /// with the current players.
        /// </summary>
        public void ServerStartMatch()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            NetworkServer.SendToAll(new InitMatchMsg());
            int spawnPosCounter = 0;
            foreach(NetworkConnectionToClient client in NetworkServer.connections.Values)
            {
                Debug.Log($"Spawning for {client.connectionId}");
                ClientManager clientManager = client.identity.GetComponent<ClientManager>();
                clientManager.ServerSpawnPlayers(spawnPosCounter);
                spawnPosCounter++;
            }
        }

        private void InitializeMatch(NetworkConnection conn, InitMatchMsg msg)
        {
            matchManager = new MatchManager(this, networkManager);
        }
    }
}