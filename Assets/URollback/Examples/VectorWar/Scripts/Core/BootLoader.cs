using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    public class BootLoader : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            ReadCommandLine();
        }

        void ReadCommandLine()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            bool startHosting = false;
            int playerCount = 2;

            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log("ARG " + i + ": " + args[i]);
                if(args[i].ToLower() == "-startHost")
                {
                    startHosting = true;
                }
                if (args[i].ToLower() == "-autostart")
                {
                    (NetworkManager.singleton as NetworkManager).autoStartGame = true;
                }
                if (args[i].ToLower().Contains("-playercount"))
                {
                    string[] split = args[i].ToLower().Split('=');
                    if(split.Length >= 2)
                    {
                        if(int.TryParse(split[1], out int r))
                        {
                            playerCount = r;
                        }
                    }
                }
            }

            if (startHosting)
            {
                (NetworkManager.singleton as NetworkManager).StartHosting(playerCount);
            }
        }
    }
}
