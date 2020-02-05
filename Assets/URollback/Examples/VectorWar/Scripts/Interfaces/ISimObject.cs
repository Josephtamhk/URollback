using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URollback.Examples.VectorWar
{
    public interface ISimObject
    {
        int SimID { get; set; }
        int CreatedFrame { get; set; }

        void SimUpdate();
        void SimLateUpdate();
    }
}