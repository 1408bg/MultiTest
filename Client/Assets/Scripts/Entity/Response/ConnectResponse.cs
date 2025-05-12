using System.Collections.Generic;

namespace Entity.Response
{
    public class ConnectResponse : UdpResponse
    {
        public string ClientId { get; set; }
        public Dictionary<string, Position2D> Users { get; set; }
    }
}