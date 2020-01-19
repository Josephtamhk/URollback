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
        [SerializeField] protected double localFrameLagAvg;
        [SerializeField] protected double remoteFrameLagAvg;
        [SerializeField] protected List<int> localFrameLagDataSet = new List<int>();
        [SerializeField] protected List<int> remoteFrameLagDataSet = new List<int>();

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
        /// input to their queue. If it's a remote client,
        /// call AdvanceRemoteInputFrame instead.
        /// </summary>
        public void AdvanceLocalInputFrame()
        {
            inputFrame++;
        }

        /// <summary>
        /// Call this whenever we get a input that
        /// belongs to a remote client on that client's URollbackClient.
        /// </summary>
        public void AdvanceRemoteInputFrame(int localClientInputFrame)
        {
            inputFrame++;
            remoteFrameLagDataSet.Add(inputFrame - localClientInputFrame);
        }

        /// <summary>
        /// Call this right after you call AdvanceInputFrame
        /// on all clients except the local one.
        /// </summary>
        /// <param name="localClientInputFrame"></param>
        public void AddLocalFrameLag(int localClientInputFrame)
        {
            localFrameLagDataSet.Add(localClientInputFrame - inputFrame);
        }
    }
}