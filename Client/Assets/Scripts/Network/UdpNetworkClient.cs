using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Manager;

namespace Network
{
    public class UdpNetworkClient
    {
        public event Action<string> OnMessageReceived;
        private readonly UdpClient _client;
        private readonly IPEndPoint _serverEndPoint;
        private readonly Thread _receiveThread;

        public UdpNetworkClient(string address, int port)
        {
            _client = new UdpClient();
            var ip = address.Split("://").Last();
            _serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            _receiveThread = new Thread(ReceiveLoop)
            {
                IsBackground = true
            };
            _receiveThread.Start();
        }
        
        private void ReceiveLoop()
        {
            while (true)
            {
                try
                {
                    var remote = new IPEndPoint(IPAddress.Any, 0);
                    var data = _client.Receive(ref remote);
                    var message = Encoding.UTF8.GetString(data);
                    ThreadManager.Instance.ExecuteOnMainThread(() => OnMessageReceived?.Invoke(message));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[UdpBaseService] Receive error: {e.Message}");
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
        
        public void Send(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            _client.Send(data, data.Length, _serverEndPoint);
        }

        public void Close()
        {
            _receiveThread.Abort();
            _client.Close();
        }
    }
}