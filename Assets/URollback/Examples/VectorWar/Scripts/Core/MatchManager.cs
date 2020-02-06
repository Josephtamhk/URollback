using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public MatchManager(GameManager gameManager, NetworkManager networkManager)
        {
            this.gameManager = gameManager;
            this.networkManager = networkManager;
            timeStepManager = new TimeStepManager(60.0f, 1.0f, 120.0f, 30.0f);
            simObjectManager = new SimObjectManager(gameManager.rollbackSession);
            timeStepManager.OnUpdate += simObjectManager.Update;
        }

        public void Update()
        {
            timeStepManager.Update(Time.deltaTime);
        }
    }
}