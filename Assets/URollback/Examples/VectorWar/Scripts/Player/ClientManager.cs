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
            networkManager.rollbackSession.GetClient(NetworkClient.connection.connectionId).RTT = rtt;
            CmdUpdateRTT(rtt);
        }

        [Command]
        public void CmdUpdateRTT(double rtt)
        {
            networkManager.rollbackSession.GetClient(NetworkClient.connection.connectionId).RTT = rtt;
            this.rtt = rtt;
            RpcUpdateRTT(rtt);
        }

        [ClientRpc]
        public void RpcUpdateRTT(double rtt)
        {
            networkManager.rollbackSession.GetClient(NetworkClient.connection.connectionId).RTT = rtt;
            this.rtt = rtt;
        }
    }
}