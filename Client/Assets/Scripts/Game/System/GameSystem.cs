using System;
using System.Collections.Generic;
using Entity.Request;
using Entity.Response;
using Game.Controller;
using Manager;
using TMPro;
using UnityEngine;

namespace Game.System
{
    public class GameSystem : MonoBehaviour
    {
        public GameObject playerPrefab;
        private string _clientId;
        private NetworkManager _network;
        private Dictionary<string, GameObject> _players;
        
        private void Start()
        {
            var clientId = PlayerPrefs.GetString("clientId");
            if (string.IsNullOrEmpty(clientId))
            {
                Debug.LogError("[GameManager] clientId is null");
                return;
            }
            Initialize(clientId);
        }

        private void OnDestroy()
        {
            if (_network != null)
            {
                _network.OnConnected -= OnConnected;
                _network.OnMoveProcessed -= OnMoveProcessed;
                _network.OnSynchronized -= OnSynchronized;
            }
            if (_players != null)
            {
                foreach (var (_, player) in _players) Destroy(player);
            }
        }
        
        private void Initialize(string clientId)
        {
            _clientId = clientId;
            _players = new Dictionary<string, GameObject>();
            _network = NetworkManager.Instance;
            _network.OnConnected += OnConnected;
            _network.OnMoveProcessed += OnMoveProcessed;
            _network.OnSynchronized += OnSynchronized;
            _network.Connect(new RegisterRequest(clientId));
        }

        private void OnConnected(ConnectResponse response)
        {
            foreach (var (id, position) in response.Users)
            {
                Debug.Log($"create player: {id}");
                var player = Instantiate(playerPrefab, new Vector3(position.X, position.Y), Quaternion.identity);
                Debug.Log("created");
                player.GetComponentInChildren<TextMeshPro>().text = id;
                _players.Add(id, player);
                if (id == _clientId)
                {
                    player
                        .AddComponent<MovementController>()
                        .Initialize(_clientId, _network.Move);
                }
            }
        }

        private void OnMoveProcessed(MoveProcessedResponse response)
        {
            if (!_players.ContainsKey(response.ClientId)) return;
            _players[response.ClientId].transform.position = new Vector3(response.Position.X, response.Position.Y);
        }

        private void OnSynchronized(SyncResponse response)
        {
            foreach (var (id, position) in response.State)
            {
                if (!_players.ContainsKey(id)) continue;
                _players[id].transform.position = new Vector3(position.X, position.Y);
            }
        }
    }
}