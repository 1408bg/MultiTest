using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class ThreadManager : MonoBehaviour
    {
        private static ThreadManager _instance;
        private readonly Queue<Action> _executionQueue = new();
        private readonly object _lock = new();

        public static ThreadManager Instance
        {
            get
            {
                if (_instance == null) Debug.LogError("[ThreadManager] Scene does not have this script");
                return _instance;
            }
        }

        private ThreadManager()
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

        private void Update()
        {
            lock (_lock)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue()?.Invoke();
                }
            }
        }

        public void ExecuteOnMainThread(Action action)
        {
            if (action == null) return;
            
            lock (_lock)
            {
                _executionQueue.Enqueue(action);
            }
        }
    }
}