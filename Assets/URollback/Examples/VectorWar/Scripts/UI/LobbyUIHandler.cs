using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URollback.Core;

namespace URollback.Examples.VectorWar
{
    public class LobbyUIHandler : MonoBehaviour
    {
        private Dictionary<int, LobbyPlayerItemUI> lobbyPlayers = new Dictionary<int, LobbyPlayerItemUI>();

        [SerializeField] private GameManager gameManager;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private GameObject lobbyPlayerItemPrefab;
        [SerializeField] private Transform lobbyPlayersParent;

        private void Awake()
        {
            networkManager.rollbackSession.OnClientAdded += UpdatePlayerList;
            networkManager.rollbackSession.OnClientRemoved += UpdatePlayerList;
        }

        private void UpdatePlayerList(int identifier)
        {
            lobbyPlayers.Clear();
            foreach(Transform child in lobbyPlayersParent)
            {
                Destroy(child.gameObject);
            }
            foreach(URollbackClient c in networkManager.rollbackSession.Clients.Values)
            {
                GameObject lobbyPlayer = Instantiate(lobbyPlayerItemPrefab, lobbyPlayersParent, false);
                LobbyPlayerItemUI lobbyPlayerUI = lobbyPlayer.GetComponent<LobbyPlayerItemUI>();
                lobbyPlayerUI.clientName.text = $"Player {c.Identifier}";
                lobbyPlayerUI.clientPing.text = (c.RTT/2.0).ToString();
                lobbyPlayers.Add(c.Identifier, lobbyPlayerUI);
            }
        }

        public void ButtonStartMatch()
        {
            gameManager.ServerStartGame();
        }
    }
}