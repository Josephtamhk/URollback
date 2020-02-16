using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URollback.Core.Input;

namespace URollback.Examples.VectorWar
{
    /// <summary>
    /// Contains the client's player(s) input on a frame.
    /// </summary>
    public class ClientInputHolder : ClientInputDefinition
    {
        // A list containing each player's inputs on a frame.
        // Index = player number.
        public List<PlayerInputDefinition> playerInputs;

        public ClientInputHolder(List<PlayerInputDefinition> playerInputs)
        {
            this.playerInputs = playerInputs; 
        }
    }
}