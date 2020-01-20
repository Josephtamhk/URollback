using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    public class BootLoader : MonoBehaviour
    {
        Windows.ConsoleWindow cWindow;
        Windows.ConsoleInput cInput;
        ServerCommandReader cReader = new ServerCommandReader();
        List<string> commandLineArgs = new List<string>();

        [SerializeField] private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            GetCommandLineArgs();
            if(IsHeadless())
            {
                cWindow = new Windows.ConsoleWindow();
                cInput = new Windows.ConsoleInput();
                cWindow.Initialize();
                cWindow.SetTitle("VectorWar Server");

                cInput.OnInputText += OnInputText;
                Application.logMessageReceived += HandleLog;
            }
            ReadCommandLine();
        }

        void OnInputText(string obj)
        {
            cReader.Read(obj);
        }

        void HandleLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
                System.Console.ForegroundColor = ConsoleColor.Yellow;
            else if (type == LogType.Error)
                System.Console.ForegroundColor = ConsoleColor.Red;
            else
                System.Console.ForegroundColor = ConsoleColor.White;

            // We're half way through typing something, so clear this line ..
            if (Console.CursorLeft != 0)
                cInput.ClearLine();

            System.Console.WriteLine(message);

            // If we were typing something re-add it.
            cInput.RedrawInputLine();
        }


        private void Update()
        {
            if (IsHeadless())
            {
                cInput.Update();
            }
        }

        private void OnDestroy()
        {
            if (IsHeadless())
            {
                cWindow.Shutdown();
            }
        }

        void ReadCommandLine()
        {
            int playerCount = 2;
            bool startHosting = false;
            bool startServer = false;
            bool connectToServer = false;
            string connectIP = "localhost";

            for (int i = 0; i < commandLineArgs.Count; i++)
            {
                string arg = commandLineArgs[i];
                if(arg == "-starthost")
                {
                    startHosting = true;
                }
                if(arg == "-startserver")
                {
                    startServer = true;
                }
                if (arg == "-autostart")
                {
                    gameManager.autoStartGame = true;
                }
                if (arg.Contains("-playercount"))
                {
                    string[] split = arg.ToLower().Split('=');
                    if(split.Length >= 2)
                    {
                        if(int.TryParse(split[1], out int r))
                        {
                            playerCount = r;
                        }
                    }
                }
                if (arg.Contains("-ip"))
                {
                    string[] split = arg.Split('=');
                    if(split.Length >= 2)
                    {
                        connectIP = split[1];
                        connectToServer = true;
                    }
                }
            }

            if (startHosting)
            {
                (NetworkManager.singleton as NetworkManager).StartHosting(playerCount);
            } else if (startServer) {
                (NetworkManager.singleton as NetworkManager).StartServer(playerCount);
            } else if (connectToServer)
            {
                (NetworkManager.singleton as NetworkManager).ConnectToServer(connectIP);
            }
        }

        void GetCommandLineArgs()
        {
            commandLineArgs = new List<string>(System.Environment.GetCommandLineArgs());
            for(int i = 0; i < commandLineArgs.Count; i++)
            {
                commandLineArgs[i] = commandLineArgs[i].ToLower();
            }
        }

        bool IsHeadless()
        {
            return Application.isBatchMode;
        }
    }
}
