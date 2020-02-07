using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public interface ISimObject
    {
        int SimID { get; set; }
        int CreatedFrame { get; set; }

        void SimUpdate();
        void SimLateUpdate();

        URollbackEntityData SaveData();
        void LoadData(URollbackEntityData entityData);
    }
}