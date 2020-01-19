using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Core
{
    [System.Serializable]
    public class URollbackClient
    {
        public delegate void RTTChangedAction();
        public event RTTChangedAction OnRTTChanged;

        public int Identifier { get { return identifier; } }
        public double RTT { get { return rtt; } set { rtt = value; OnRTTChanged?.Invoke(); } }
        public int InputFrame { get { return inputFrame; } }

        [SerializeField] protected int identifier;
        [SerializeField] protected double rtt;
        [SerializeField] protected int inputFrame;

        public URollbackClient(int identifier)
        {
            this.identifier = identifier;
        }

        public URollbackClient(int identifier, double rtt)
        {
            this.identifier = identifier;
            this.rtt = rtt;
        }

        /// <summary>
        /// Call this whenever the client adds another
        /// input to their queue.
        /// </summary>
        public void AdvanceInputFrame()
        {
            inputFrame++;
        }
    }
}