namespace Vedia.API.Models.DTO
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public Profile Profile { get; set; }
    }
}