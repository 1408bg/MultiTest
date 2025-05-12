namespace Entity.Request
{
    public class MoveRequest : Request
    {
        public string ClientId { get; set; }
        public float Dx { get; set; }
        public float Dy { get; set; }
        
        public MoveRequest(string clientId, float dx, float dy)
        {
            ClientId = clientId;
            Dx = dx;
            Dy = dy;
        }

        public override string Serialize()
        {
            return $"move\n{ClientId}|{Dx},{Dy}";
        }
    }
}