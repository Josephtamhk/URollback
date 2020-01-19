using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField] private NetworkManager networkManager;


        private void Awake()
        {
            instance = this;
        }

    }
}