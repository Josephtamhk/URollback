using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    public struct PlayerInputDefinition
    {
        public sbyte thrustMagnitude; // quantized thrust magnitude. [-127,127] -> [-1,1]
        public sbyte turnMagnitude; // quantized turn magnitude. [-127,127] -> [-1,1]
        public bool firing;

        public PlayerInputDefinition(float thrustMag, float turnMag, bool isFiring)
        {
            thrustMag = Mathf.Clamp(thrustMag, -1, 1);
            thrustMagnitude = (sbyte)(thrustMag * 127.0f);
            turnMag = Mathf.Clamp(turnMag, -1, 1);
            turnMagnitude = (sbyte)(turnMag * 127.0f);
            firing = isFiring;
        }
    }
}