using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    [System.Serializable] 
    public class SyncListURollbackClient : SyncList<URollbackClient> { }
    public enum URollbackSessionClientsMsgType
    {
        Add = 0,
        Remove = 1
    }

    public class URollbackSessionClientsMsg : MessageBase
    {
        public URollbackSessionClientsMsgType msgType;
        public SyncListURollbackClient clients = new SyncListURollbackClient();

        public URollbackSessionClientsMsg()
        {

        }

        public URollbackSessionClientsMsg(URollbackSessionClientsMsgType msgType, URollbackClient[] clients)
        {
            this.msgType = msgType;
            foreach(URollbackClient client in clients)
            {
                this.clients.Add(client);
            }
        }
    }
}