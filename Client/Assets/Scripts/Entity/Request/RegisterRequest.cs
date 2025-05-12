namespace Entity.Request
{
    public class RegisterRequest : Request
    {
        public string ClientId { get; set; }
        
        public RegisterRequest(string clientId)
        {
            ClientId = clientId;
        }
        
        public override string Serialize()
        {
            return $"register\n{ClientId}";
        }
    }
}