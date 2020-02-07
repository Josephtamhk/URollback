using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public class PlayerInput : MonoBehaviour, ISimObject
    {
        public int SimID { get; set; } = -1;
        public int CreatedFrame { get; set; } = 0;

        private ClientManager clientManager;
        [SerializeField] private PlayerManager playerManager;

        public void Init(ClientManager clientManager)
        {
            this.clientManager = clientManager;
        }

        public void SimUpdate()
        {

        }

        public void SimLateUpdate()
        {

        }

        public URollbackEntityData SaveData()
        {
            return new URollbackEntityData();
        }

        public void LoadData(URollbackEntityData entityData)
        {

        }
    }
}