using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    /// <summary>
    /// Handles executing command line arguments.
    /// </summary>
    public class ServerCommandLineHandler
    {
        private GameManager gameManager;
        private NetworkManager networkManager;
        private List<string> commandLine;

        private bool startHosting;
        private bool startServer;
        private string ip;
        private int playerCount;

        public ServerCommandLineHandler(GameManager gameManager, NetworkManager networkManager)
        {
            this.gameManager = gameManager;
            this.networkManager = networkManager;
            commandLine = new List<string>(System.Environment.GetCommandLineArgs());
        }

        /// <summary>
        /// Reads the command line arguments and executes them.
        /// </summary>
        public void ReadCommandLineArgs()
        {
            List<string> commandLine = new List<string>(System.Environment.GetCommandLineArgs());
            foreach(string argument in commandLine)
            {
                ReadArgument(argument.ToLower());
            }

            // Handle the results of the commands.
            if (startHosting)
            {
                networkManager.StartHosting(playerCount);
            }
            else if (startServer)
            {
                networkManager.StartServer(playerCount);
            }
            else if (!string.IsNullOrEmpty(ip))
            {
                networkManager.ConnectToServer(ip);
            }
        }

        private void ReadArgument(string argument)
        {
            switch (argument)
            {
                case string a when a.Contains("-starthost"):
                    startHosting = true;
                    break;
                case string b when b.Contains("-startserver"):
                    startServer = true;
                    break;
                case string c when c.Contains("-ip"):
                    string[] cSplit = c.Split('=');
                    if (cSplit.Length >= 2)
                    {
                        ip = cSplit[1];
                    }
                    break;
                case string d when d.Contains("-playercount"):
                    string[] dSplit = d.Split('=');
                    if (dSplit.Length >= 2)
                    {
                        if (int.TryParse(dSplit[1], out int r))
                        {
                            playerCount = r;
                        }
                    }
                    break;
                case string e when e.Contains("-autostart"):
                    gameManager.autoStartMatch = true;
                    break;
            }
        }
    }
}