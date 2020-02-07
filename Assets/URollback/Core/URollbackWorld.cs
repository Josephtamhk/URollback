using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A URollbackWorld keeps track of the data of the sim entities currently
/// in the rollback session, and handles restoring their data and saving it.
/// </summary>
namespace URollback.Core
{
    public class URollbackWorld
    {
        protected int idCounter = 0;
        protected Dictionary<int, URollbackEntity> entities = new Dictionary<int, URollbackEntity>();

        /// <summary>
        /// Adds an entity to the world.
        /// </summary>
        /// <returns>The ID of the entity.</returns>
        public int AddEntity(URollbackEntityData entityData)
        {
            int entityID = idCounter;
            entities.Add(entityID, new URollbackEntity(entityID, entityData));
            idCounter++;
            return entityID;
        }

        /// <summary>
        /// Removes the entity with the given ID, if it exist.
        /// </summary>
        /// <param name="entityID"></param>
        public void RemoveEntity(int entityID)
        {
            entities.Remove(entityID);
        }

        /// <summary>
        /// Returns the entity with the given ID.
        /// Returns null if it does not exist.
        /// </summary>
        /// <param name="entityID"></param>
        public URollbackEntity GetEntity(int entityID)
        {
            if (!entities.ContainsKey(entityID))
            {
                return null;
            }
            return entities[entityID];
        }

        /// <summary>
        /// If an entity exist at that ID.
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public bool EntityExist(int entityID)
        {
            return entities.ContainsKey(entityID);
        }
    }
}