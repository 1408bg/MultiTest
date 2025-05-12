using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Network
{
    public class TcpNetworkClient
    {
        public event Action<string, string> OnMessageReceived;
        private readonly HttpClient _client;

        public TcpNetworkClient(string address, int port)
        {
            var handler = new HttpClientHandler { UseCookies = false};
            _client = new HttpClient(handler);
            if (Uri.TryCreate($"{address}:{port}", UriKind.Absolute, out var uri)) _client.BaseAddress = uri;
            else Debug.LogError("invalid url");
        }

        private async Task<bool> Request(string path, [CanBeNull] string data, Func<string, string, Task<HttpResponseMessage>> callback)
        {
            try
            {
                if (callback == null) return false;
                var response = await callback.Invoke(path, data);
                var message = await response.Content.ReadAsStringAsync();
                OnMessageReceived?.Invoke(path, message);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                Debug.Log($"Error Handled: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                
                Debug.LogError($"[TcpNetworkService] {ex}");
                return false;
            }
        }
        
        public async Task<bool> Get(string path)
        {
            return await Request(path, null, (url, _) => _client.GetAsync(url));
        }

        public async Task<bool> Post(string path, string data)
        {
            return await Request(path, data, (url, body) => _client.PostAsync(url, new StringContent(body, Encoding.Unicode)));
        }

        public void Close()
        {
            _client.Dispose();
        }
    }
}