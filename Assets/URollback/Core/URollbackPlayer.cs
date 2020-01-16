using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Core
{
    public class URollbackClient
    {
        public delegate void RTTChangedAction();
        public static event RTTChangedAction OnRTTChanged;

        public int Identifier { get { return identifier; } }
        public float RTT { get { return rtt; } set { rtt = value; OnRTTChanged?.Invoke(); } }

        private int identifier;
        private float rtt;

        public URollbackClient(int identifier)
        {
            this.identifier = identifier;
        }

        
    }
}