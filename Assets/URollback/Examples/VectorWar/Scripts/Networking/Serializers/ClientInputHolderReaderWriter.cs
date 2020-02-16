using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public static class ClientInputHolderReaderWriter
    {
        public static void WriteClientInputHolder(this NetworkWriter writer, ClientInputHolder inputHolder)
        {
            writer.WriteInt32(inputHolder.playerInputs.Count);
            for(int i = 0; i < inputHolder.playerInputs.Count; i++)
            {
                writer.WritePlayerInputDefinition(inputHolder.playerInputs[i]);
            }
        }

        public static ClientInputHolder ReadClientInputHolder(this NetworkReader reader)
        {
            int listSize = reader.ReadInt32();
            List<PlayerInputDefinition> inputs = new List<PlayerInputDefinition>(listSize);
            for(int i = 0; i < listSize; i++)
            {
                inputs.Add(reader.ReadPlayerInputDefinition());
            }
            return new ClientInputHolder(inputs);
        }
    }
}