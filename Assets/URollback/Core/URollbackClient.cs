using System.Collections.Generic;
using URollback.Core.Input;

namespace URollback.Core
{
    [System.Serializable]
    public class URollbackClient
    {
        public delegate void RTTChangedAction();
        public event RTTChangedAction OnRTTChanged;

        public int Identifier { get { return identifier; } }
        public double RTT { get { return rtt; } set { rtt = value; OnRTTChanged?.Invoke(); } }
        public int InputFrame { get { return inputLog.InputFrame(); } }
        public int InputDelay { get { return inputDelay; } }

        protected ClientInputLog inputLog = new ClientInputLog();
        protected int identifier;
        protected double rtt;
        protected int inputFrame;
        protected double localFrameLagAvg;
        protected double remoteFrameLagAvg;
        protected List<int> localFrameLagDataSet = new List<int>();
        protected List<int> remoteFrameLagDataSet = new List<int>();
        protected int inputDelay;

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
        public void AddInput(ClientInputDefinition input)
        {
            inputLog.AddInput(input);
        }

        /// <summary>
        /// Remote Frame Lag is how many frames the remote client
        /// is predicting for this local client.
        /// Call this whenever we get a input that
        /// belongs to a remote client on that client's URollbackClient.
        /// </summary>
        public void AddRemoteFrameLag(int localClientInputFrame)
        {
            remoteFrameLagDataSet.Add(inputFrame - localClientInputFrame);
        }

        /// <summary>
        /// Local Frame Lag is how many frames we're predicting
        /// for this remote client.
        /// </summary>
        /// <param name="localClientInputFrame"></param>
        public void AddLocalFrameLag(int localClientInputFrame)
        {
            localFrameLagDataSet.Add(localClientInputFrame - inputFrame);
        }

        public void CalculateFrameLag()
        {
            if (localFrameLagDataSet.Count != 0)
            {
                localFrameLagAvg = 0;
                for(int i = 0; i < localFrameLagDataSet.Count; i++)
                {
                    localFrameLagAvg += localFrameLagDataSet[i];
                }
                localFrameLagAvg /= localFrameLagDataSet.Count;
                localFrameLagDataSet.Clear();
            }

            if(remoteFrameLagDataSet.Count != 0)
            {
                remoteFrameLagAvg = 0;
                for(int i = 0; i < remoteFrameLagDataSet.Count; i++)
                {
                    remoteFrameLagAvg += remoteFrameLagDataSet[i];
                }
                remoteFrameLagAvg /= remoteFrameLagDataSet.Count;
                remoteFrameLagDataSet.Clear();
            }
        }

        public void SetInputDelay(int value)
        {
            inputDelay = value;
        }
    }
}