using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Vedia.API.Models;
using Vedia.API.Models.DTO;

namespace Vedia.API.Services
{
    public class ProfileService
    {
        public IMongoCollection<Profile> Profiles { get; }
        private readonly SymmetricSecurityKey _key;

        public ProfileService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("MongoDB")["ConnectionString"]);
            var database = client.GetDatabase(configuration.GetSection("MongoDB")["DatabaseName"]);
            Profiles = database.GetCollection<Profile>("Profiles");
            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("Secrets")["JwtSecret"]));
        }

        public async Task<AuthenticationResponse> Authenticate(string username, string password)
        {
            var profile = await Profiles.Find(p => p.Username == username).FirstOrDefaultAsync();
            if (profile is not null && VerifyPassword(profile, password))
                return GenerateResponse(profile);
            return null;
        }

        public async Task<Profile> ValidateToken(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var id = ((JwtSecurityToken) validatedToken).Claims.First(c => c.Type == "id").Value;
                return await Profiles.Find(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch
            {
                return null;
            }
        }

        private AuthenticationResponse GenerateResponse(Profile profile)
        {
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", profile.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(descriptor);
            return new AuthenticationResponse
            {
                Profile = profile,
                Token = handler.WriteToken(token)
            };
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        private bool VerifyPassword(Profile profile, string password) 
            => BCrypt.Net.BCrypt.EnhancedVerify(password, profile.Hash);
    }
}