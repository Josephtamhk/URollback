﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using URollback.Core;
using URollback.Core.Input;

namespace URollback.Examples.VectorWar
{
    public class ClientManager : NetworkBehaviour
    {
        private GameManager gameManager;
        private NetworkManager networkManager;
        [SerializeField] private GameObject playerPrefab;

        public double ClientRTT { get { return rtt; } }

        private float rttUpdateTimer;
        private double rtt;

        [SyncVar] public int connectionID;
        private int players = 1;

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

        public ClientInputHolder PollLocalInputs()
        {
            List<PlayerInputDefinition> inputs = new List<PlayerInputDefinition>();
            for (int i = 0; i < players; i++)
            {
                float thrustMag = Input.GetAxis("Vertical");
                float turnMag = Input.GetAxis("Horizontal");
                bool isFiring = Input.GetKey(KeyCode.Space);
                inputs.Add(new PlayerInputDefinition(thrustMag, turnMag, isFiring));
            }
            ClientInputHolder clientInputHolder = new ClientInputHolder(inputs);
            return clientInputHolder;
        }

        // Sends the client's inputs to the server.
        // The server then sends the inputs to other clients.
        [Command]
        public void CmdSendInputs(ClientInputHolder inputs)
        {
            //gameManager.rollbackSession.add
            foreach (NetworkConnectionToClient nc in NetworkServer.connections.Values)
            {
                // Ignore the client that sent the message.
                if (nc.connectionId != connectionToClient.connectionId)
                {
                    TargetRecieveInputs(nc, inputs);
                }
            }
        }

        [TargetRpc]
        public void TargetRecieveInputs(NetworkConnection target, ClientInputHolder inputs)
        {
            gameManager.rollbackSession
                .AddRemoteInput(NetworkClient.connection.connectionId, target.connectionId, inputs);
        }
    }
}