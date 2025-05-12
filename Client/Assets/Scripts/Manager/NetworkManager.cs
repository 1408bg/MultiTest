using System;
using System.Threading.Tasks;
using Entity.Request;
using Entity.Response;
using Network;
using UnityEngine;
using Util;

namespace Manager
{
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _instance;
        public static NetworkManager Instance
        {
            get
            {
                if (_instance == null) Debug.LogError("[NetworkManager] Scene does not have this script");
                return _instance;
            }
        }

        private TcpNetworkClient _tcp;
        private UdpNetworkClient _udp;
        private long _lastSyncedTime;

        public event Action<StatusResponse> OnStatusResponded; 
        public event Action<ConnectResponse> OnConnected;
        public event Action<MoveProcessedResponse> OnMoveProcessed;
        public event Action<SyncResponse> OnSynchronized;

        private NetworkManager()
        {
        }


        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy()
        {
            CleanUp();
        }

        public void Initialize(string address)
        {
            _tcp = new TcpNetworkClient(address, 40001);
            _udp = new UdpNetworkClient(address, 40000);
            _lastSyncedTime = 0;

            _tcp.OnMessageReceived += HandleTcpMessage;
            _udp.OnMessageReceived += HandleUdpMessage;
        }

        public void CleanUp()
        {
            if (_tcp != null)
            {
                _tcp.OnMessageReceived -= HandleTcpMessage;
                _tcp.Close();
            }
            if (_udp != null)
            {
                _udp.OnMessageReceived -= HandleUdpMessage;
                _udp.Close();
            }
        }

        public Task<bool> CheckStatus()
        {
            return _tcp.Get("/");
        }

        public void Connect(RegisterRequest request)
        {
            _udp.Send(request.Serialize());
        }

        public void Move(MoveRequest request)
        {
            _udp.Send(request.Serialize());
        }

        private void HandleTcpMessage(string path, string message)
        {
            if (path == "/")
            {
                OnStatusResponded?.Invoke(Parser.ParseJson<StatusResponse>(message));
            }
        }

        private void HandleUdpMessage(string data)
        {
            var baseStruct = Parser.ParseJson<UdpResponse>(data);
            if (baseStruct.Timestamp < _lastSyncedTime) return;
            _lastSyncedTime = baseStruct.Timestamp;
            switch (baseStruct.Type)
            {
                case "connect":
                    OnConnected?.Invoke(Parser.ParseJson<ConnectResponse>(data));
                    return;
                case "move_processed":
                    OnMoveProcessed?.Invoke(Parser.ParseJson<MoveProcessedResponse>(data));
                    return;
                case "sync":
                    OnSynchronized?.Invoke(Parser.ParseJson<SyncResponse>(data));
                    break;
                default:
                    Debug.LogWarning($"[NetworkManager] Unknown Type: {baseStruct.Type}");
                    break;
            }
        }

        public static bool IsValidAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return false;
            if (address.ToLower() == "localhost") return true;

            var isUri = Uri.TryCreate(address, UriKind.Absolute, out _);
            return isUri;
        }
    }
}