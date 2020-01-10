using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace URollback.Core
{
    public class URollbackPlayer
    {
        public NetworkConnection networkConnection;

        public URollbackPlayer(NetworkConnection networkConnection)
        {
            this.networkConnection = networkConnection;
        }
    }
}