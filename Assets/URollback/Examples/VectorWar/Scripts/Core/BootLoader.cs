using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    /// <summary>
    /// This class handles initilizing things as soon as the game starts.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        private ServerCommandLineHandler cLineHandler;
        private ServerConsoleHandler consoleHandler;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private NetworkManager networkManager;

        void Start()
        {
            cLineHandler = new ServerCommandLineHandler(gameManager, networkManager);
            if(IsHeadless())
            {
                consoleHandler = new ServerConsoleHandler("VectorWar Server", gameManager);
            }

            cLineHandler.ReadCommandLineArgs();
        }


        private void Update()
        {
            if (IsHeadless())
            {
                consoleHandler.Update();
            }
        }

        private void OnDestroy()
        {
            if (IsHeadless())
            {
                consoleHandler.Shutdown();
            }
        }

        private bool IsHeadless()
        {
            return Application.isBatchMode;
        }
    }
}
