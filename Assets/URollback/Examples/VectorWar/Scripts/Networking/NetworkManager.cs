using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace URollback.Examples.VectorWar
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public bool autoStartGame;

        public void StartHosting(int playerCount)
        {
            maxConnections = playerCount - 1;
            StartHost();
        }
    }
}