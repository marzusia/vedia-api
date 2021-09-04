using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vedia.API.Services;

namespace Vedia.API.Middleware
{
    public class VediaAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ProfileService _profileService;
        
        public VediaAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ProfileService profileService)
            : base(options, logger, encoder, clock)
        {
            _profileService = profileService;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Unauthorized");

            string authHeader = Request.Headers["Authorization"];
            if (!authHeader.StartsWith("Bearer "))
                return AuthenticateResult.Fail("Unauthorized");

            var token = authHeader.Split(" ")[1];
            var profile = await _profileService.ValidateToken(token);
            if (profile is null)
                return AuthenticateResult.Fail("Unauthorized");

            var claims = new List<Claim> {new Claim("id", profile.Id)};
            if (profile.Roles is not null)
                claims.AddRange(profile.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}