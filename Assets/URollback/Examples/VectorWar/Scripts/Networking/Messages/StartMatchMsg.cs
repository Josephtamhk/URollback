using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace URollback.Examples.VectorWar
{
    public class StartMatchMsg : MessageBase
    {
        public double delay;

        public StartMatchMsg()
        {
            delay = 0;
        }

        public StartMatchMsg(double delay)
        {
            this.delay = delay;
        }
    }
}