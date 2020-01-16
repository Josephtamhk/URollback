using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace URollback.Examples.VectorWar
{
    public class ClientManager : NetworkBehaviour
    {
        private GameManager gameManager;

        int rtt;
        float rttUpdateTimer;

        private void Start()
        {
            gameManager = GameManager.instance;
        }

        private void Update()
        {
            rttUpdateTimer += Time.deltaTime;
            if(rttUpdateTimer > 2.5f)
            {
                rttUpdateTimer = 0;
                CmdUpdateRTT(rtt);
            }
        }

        [Command]
        public void CmdUpdateRTT(int rtt)
        {
            this.rtt = rtt;
        }
    }
}