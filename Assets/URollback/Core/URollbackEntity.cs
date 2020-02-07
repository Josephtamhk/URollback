using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A definition of a entity in a URollbackWorld.
/// </summary>
namespace URollback.Core
{
    public class URollbackEntity
    {
        protected int ID;
        protected URollbackEntityData entityData;

        public URollbackEntity(int ID, URollbackEntityData entityData)
        {
            this.ID = ID;
            this.entityData = entityData;
        }

        public void SaveData(URollbackEntityData entityData)
        {
            this.entityData = entityData;
        }

        public URollbackEntityData LoadData()
        {
            return entityData;
        }
    }
}