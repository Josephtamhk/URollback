using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    /// <summary>
    /// Manages spawning and registering gameobjects to the simulation,
    /// and linking them with their data representation in the rollback session.
    /// </summary>
    public class SimObjectManager
    {
        private URollbackSession uRollbackSession;

        private List<ISimObject> simObjects = new List<ISimObject>();

        public SimObjectManager(URollbackSession uRollbackSession)
        {
            this.uRollbackSession = uRollbackSession;
        }

        public void Update(float dt)
        {
            for(int i = 0; i < simObjects.Count; i++)
            {
                simObjects[i].SimUpdate();
            }

            for(int i = 0; i < simObjects.Count; i++)
            {
                simObjects[i].SimLateUpdate();
            }
        }

        /// <summary>
        /// Registers an object to the simulation.
        /// This method should only be called during initilization of the match
        /// to register players and scene objects, anything else should use the
        /// SpawnObject method.
        /// </summary>
        /// <param name="simObject"></param>
        public void RegisterObject(ISimObject simObject)
        {
            int spawnID = uRollbackSession.URollbackWorld.AddEntity(simObject.SaveData());
            simObject.SimID = spawnID;
            simObjects.Add(simObject);
        }

        /// <summary>
        /// Spawns a gameobject and adds it to the simulation.
        /// This should be called in place of instantiate.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SpawnObject(GameObject gameObject, Vector3 position, Quaternion rotation)
        {

        }
    }
}