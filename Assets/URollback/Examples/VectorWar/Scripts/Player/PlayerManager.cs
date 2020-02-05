using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    public class PlayerManager : MonoBehaviour, ISimObject
    {
        public int SimID { get; set; } = -1;
        public int CreatedFrame { get; set; } = -1;

        public void SimUpdate()
        {

        }

        public void SimLateUpdate()
        {

        }
    }
}