using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    /// <summary>
    /// Handles reading commands and sending messages in the console.
    /// </summary>
    public class ServerConsoleHandler
    {
        private GameManager gameManager;
        private Windows.ConsoleWindow cWindow;
        private Windows.ConsoleInput cInput;

        public ServerConsoleHandler(string title, GameManager gameManager)
        {
            this.gameManager = gameManager;
            cWindow = new Windows.ConsoleWindow();
            cInput = new Windows.ConsoleInput();

            cWindow.Initialize();
            cWindow.SetTitle(title);

            cInput.OnInputText += Read;
            Application.logMessageReceived += HandleLog;
        }

        public void Update()
        {
            cInput.Update();
        }

        public void Shutdown()
        {
            cWindow.Shutdown();
        }

        public void Read(string command)
        {
            command = command.ToLower();
        }

        private void HandleLog(string message, string stackTrace, LogType type)
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
    }
}