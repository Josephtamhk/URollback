﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    public class MatchManager
    {
        private GameManager gameManager;
        private NetworkManager networkManager;
        private TimeStepManager timeStepManager;
        private List<ISimObject> simObjects = new List<ISimObject>();

        public MatchManager(GameManager gameManager, NetworkManager networkManager)
        {
            this.gameManager = gameManager;
            this.networkManager = networkManager;
            timeStepManager = new TimeStepManager(60.0f, 1.0f, 120.0f, 30.0f);
            timeStepManager.OnUpdate += SimUpdate;
        }

        public void Update()
        {
            timeStepManager.Update(Time.deltaTime);
        }

        private void SimUpdate(float dt)
        {
            for(int i = 0; i < simObjects.Count; i++)
            {
                simObjects[i].SimUpdate();
            }

            for(int i = 0; i < simObjects.Count; i++)
            {
                simObjects[i].SimLateUpdate();
            }
        }
    }
}