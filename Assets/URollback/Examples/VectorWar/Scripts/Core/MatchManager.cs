using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    /// <summary>
    /// 
    /// </summary>
    public class MatchManager
    {
        private GameManager gameManager;
        private NetworkManager networkManager;
        private TimeStepManager timeStepManager;
        public SimObjectManager simObjectManager;

        public bool MatchStarted { get { return matchStarted; } }

        private bool matchStarted;

        public MatchManager(GameManager gameManager, NetworkManager networkManager)
        {
            this.gameManager = gameManager;
            this.networkManager = networkManager;
            timeStepManager = new TimeStepManager(60.0f, 1.0f, 120.0f, 30.0f);
            simObjectManager = new SimObjectManager(gameManager.rollbackSession);
            timeStepManager.OnUpdate += Tick;
        }

        public void StartMatch()
        {
            matchStarted = true;
        }

        public void Update()
        {
            timeStepManager.Update(Time.deltaTime);
        }

        protected virtual void Tick(float dt)
        {
            /*
            // Add the player's local input to the simulation.
            URollbackErrorCode result = gameManager.rollbackSession.AdvanceLocalInput(NetworkClient.connection.connectionId);

            if(result == URollbackErrorCode.OK)
            {
                simObjectManager.Update(dt);
            }*/
        }
    }
}