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

        private List<List<PlayerInputDefinition>> playersInputs = new List<List<PlayerInputDefinition>>();

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

        [Server]
        public void ServerSpawnPlayers(int spawnIndex)
        {
            RpcSpawnPlayers(spawnIndex);
        }

        [ClientRpc]
        public void RpcSpawnPlayers(int spawnIndex)
        {
            Debug.Log($"Client spawned player for {connectionID}");
            GameObject player = Instantiate(playerPrefab, gameManager.spawnPositions[spawnIndex], Quaternion.identity);
            gameManager.MatchManager.simObjectManager.RegisterObject(player.GetComponent<ISimObject>());
        }
    }
}