using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public class ClientManager : NetworkBehaviour
    {
        private GameManager gameManager;
        private NetworkManager networkManager;
        [SerializeField] private GameObject playerPrefab;

        double rtt;
        float rttUpdateTimer;

        [SyncVar] public int connectionID;

        private List<PlayerInputLog> playersInputs = new List<PlayerInputLog>();

        private void Start()
        {
            gameManager = GameManager.instance;
            networkManager = NetworkManager.instance;
        }

        private void Update()
        {
            if (!hasAuthority)
            {
                return;
            }

            rttUpdateTimer += Time.deltaTime;
            if(rttUpdateTimer > 2.5f)
            {
                rttUpdateTimer = 0;
                rtt = NetworkTime.rtt;
                SetRTT();
            }
        }

        private void SetRTT()
        {
            gameManager.rollbackSession.GetClient(NetworkClient.connection.connectionId).RTT = rtt;
            CmdUpdateRTT(rtt);
        }

        [Command]
        public void CmdUpdateRTT(double rtt)
        {
            gameManager.rollbackSession.GetClient(NetworkClient.connection.connectionId).RTT = rtt;
            this.rtt = rtt;
            RpcUpdateRTT(rtt);
        }

        [ClientRpc]
        public void RpcUpdateRTT(double rtt)
        {
            gameManager.rollbackSession.GetClient(NetworkClient.connection.connectionId).RTT = rtt;
            this.rtt = rtt;
        }

        /// <summary>
        /// Tell every client to spawn this client's player.
        /// </summary>
        /// <param name="spawnIndex"></param>
        [Server]
        public void ServerSpawnPlayers(int spawnIndex)
        {
            RpcSpawnPlayers(spawnIndex);
        }

        /// <summary>
        /// Spawn this client's player(s).
        /// </summary>
        /// <param name="spawnIndex"></param>
        [ClientRpc]
        public void RpcSpawnPlayers(int spawnIndex)
        {
            Debug.Log($"Client spawned player for {connectionID}");
            GameObject player = Instantiate(playerPrefab, gameManager.spawnPositions[spawnIndex], Quaternion.identity);
            gameManager.MatchManager.simObjectManager.RegisterObject(player.GetComponent<ISimObject>());
            player.GetComponent<PlayerManager>().Init(this);
        }

        /// <summary>
        /// Gets the input of the given player, with an optional
        /// offset of how far back you want a input for.
        /// </summary>
        /// <param name="player">The player you want the input for.</param>
        /// <param name="framesBack">How many frames before the current one you want. This number
        /// should not be less than 0.</param>
        /// <returns></returns>
        public PlayerInputDefinition GetInput(int player, int framesBack=0)
        {
            return playersInputs[player].GetInput(0-framesBack);
        }
    }
}