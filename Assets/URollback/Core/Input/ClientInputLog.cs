using System.Collections.Generic;

namespace URollback.Core.Input
{
    /// <summary>
    /// Keeps track of a client's player inputs.
    /// </summary>
    public class ClientInputLog
    {
        protected List<ClientInputDefinition> inputLog
            = new List<ClientInputDefinition>();

        public void AddInput(ClientInputDefinition input)
        {
            inputLog.Add(input);
        }

        public int InputFrame()
        {
            return inputLog.Count;
        }
    }
}
