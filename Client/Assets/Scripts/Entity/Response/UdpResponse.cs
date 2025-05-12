namespace Entity.Response
{
    public class UdpResponse
    {
        public string Type { get; set; }
        public long Timestamp { get; set; }
    }
    
    public class Position2D
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}