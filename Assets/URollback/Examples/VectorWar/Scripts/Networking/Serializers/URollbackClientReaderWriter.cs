using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public static class URollbackClientReaderWriter
    {
        public static void WriteURollbackClient(this NetworkWriter writer, URollbackClient client)
        {
            writer.WriteInt32(client.Identifier);
            writer.WriteDouble(client.RTT);
        }

        public static URollbackClient ReadURollbackClient(this NetworkReader reader)
        {
            return new URollbackClient(reader.ReadInt32(), reader.ReadDouble());
        }
    }
}