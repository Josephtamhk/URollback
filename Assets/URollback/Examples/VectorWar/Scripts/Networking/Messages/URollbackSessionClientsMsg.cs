using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public enum URollbackSessionClientsMsgType
    {
        Add = 0,
        Remove = 1
    }

    public class URollbackSessionClientsMsg : MessageBase
    {
        public URollbackSessionClientsMsgType msgType;
        public URollbackClient[] clients;

        public URollbackSessionClientsMsg()
        {

        }

        public URollbackSessionClientsMsg(URollbackSessionClientsMsgType msgType, URollbackClient[] clients)
        {
            this.msgType = msgType;
            this.clients = clients;
        }
    }
}