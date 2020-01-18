using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Core
{
    [System.Serializable]
    public class URollbackClient
    {
        public delegate void RTTChangedAction();
        public static event RTTChangedAction OnRTTChanged;

        public int Identifier { get { return identifier; } }
        public double RTT { get { return rtt; } set { rtt = value; OnRTTChanged?.Invoke(); } }

        [SerializeField] private int identifier;
        [SerializeField] private double rtt;

        public URollbackClient(int identifier)
        {
            this.identifier = identifier;
        }

        public URollbackClient(int identifier, double rtt)
        {
            this.identifier = identifier;
            this.rtt = rtt;
        }
    }
}