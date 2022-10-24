using Authentication.WebAPI.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.WebAPI.Services
{
    public class TokenFactory : ITokenFactory
    {
        private AppSettings _settings;
        public TokenFactory(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }
        public string GenerateAcessToken(string domainAndLANId, string role)
        {
            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, domainAndLANId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64),
                    new Claim("Role",role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Secret));

            var jwt = new JwtSecurityToken(
                issuer: _settings.Iss,
                audience: _settings.Aud,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(1)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public string GenerateRefreshToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
