using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    public class PlayerInputLog
    {
        protected List<PlayerInputDefinition> playerInputs = new List<PlayerInputDefinition>();

        public PlayerInputDefinition GetInput(int frame)
        {
            if(frame >= playerInputs.Count)
            {
                return playerInputs[playerInputs.Count-1];
            }
            return playerInputs[frame];
        }

        public void AddInput(PlayerInputDefinition playerInput)
        {
            playerInputs.Add(playerInput);
        }

        public int InputCount()
        {
            return playerInputs.Count;
        }
    }
}