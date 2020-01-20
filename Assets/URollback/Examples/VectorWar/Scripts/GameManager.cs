using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URollback.Core;
using Mirror;

namespace URollback.Examples.VectorWar
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField] private NetworkManager networkManager;
        public List<Vector2> spawnPositions = new List<Vector2>();

        public bool autoStartGame;

        private void Awake()
        {
            instance = this;
            networkManager.rollbackSession.OnClientAdded += AutoStartGame;
        }

        private void AutoStartGame(int identifier)
        {
            if (!autoStartGame || !NetworkServer.active)
            {
                return;
            }
            if(networkManager.rollbackSession.Clients.Count >= networkManager.maxConnections)
            {
                Debug.Log($"Starting game with {networkManager.rollbackSession.Clients.Count} players.");
                ServerStartGame();
            }
        }

        /// <summary>
        /// Called on the server when we want the game to start
        /// with the current players.
        /// </summary>
        public void ServerStartGame()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            int spawnPosCounter = 0;
            foreach(NetworkConnectionToClient client in NetworkServer.connections.Values)
            {
                Debug.Log($"Spawning for {client.connectionId}");
                ClientManager clientManager = client.identity.GetComponent<ClientManager>();
                clientManager.ServerSpawnPlayers(spawnPosCounter);
                spawnPosCounter++;
            }
        }
    }
}