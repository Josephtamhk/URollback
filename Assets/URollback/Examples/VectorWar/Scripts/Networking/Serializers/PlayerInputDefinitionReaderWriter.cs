using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public static class PlayerInputDefinitionReaderWriter
    {
        public static void WritePlayerInputDefinition(this NetworkWriter writer, PlayerInputDefinition playerInputDefinition)
        {
            writer.WriteSByte(playerInputDefinition.thrustMagnitude);
            writer.WriteSByte(playerInputDefinition.turnMagnitude);
            writer.WriteBoolean(playerInputDefinition.firing);
        }

        public static PlayerInputDefinition ReadPlayerInputDefinition(this NetworkReader reader)
        {
            return new PlayerInputDefinition(reader.ReadSByte(), reader.ReadSByte(), reader.ReadBoolean());
        }
    }
}