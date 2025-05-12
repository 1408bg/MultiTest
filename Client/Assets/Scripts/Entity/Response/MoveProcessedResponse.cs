namespace Entity.Response
{
    public class MoveProcessedResponse : UdpResponse
    {
        public string ClientId { get; set; }
        public Position2D Position { get; set; }
    }
}