using System.Collections.Generic;

namespace Entity.Response
{
    public class SyncResponse : UdpResponse
    {
        public Dictionary<string, Position2D> State { get; set; }
    }
}