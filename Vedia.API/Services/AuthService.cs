using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Vedia.API.Services
{
    public class AuthService
    {
        private readonly ProfileService _profileService;
        private readonly SymmetricSecurityKey _key;

        public AuthService(ProfileService profileService, IConfiguration configuration)
        {
            _profileService = profileService;
            _key = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(configuration.GetSection("Secrets")["JwtSecret"]));
        }
    }
}