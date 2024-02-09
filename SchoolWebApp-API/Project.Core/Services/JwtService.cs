using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolSoftWebApi.Models.Identity;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.Core.Services
{
    public class JwtService
    {
        private const int EXPIRATION_MINUTES = 600;
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthenticationResponse CreateToken(AppUser user, IList<string> userRoles)
        {
            var expiration = DateTime.UtcNow.AddMinutes(EXPIRATION_MINUTES);
            var userClaims = CreateClaims(user);

            foreach (var userRole in userRoles)
            {
                userClaims.Add(new Claim("roles", userRole));
            }

            var token = CreateJwtToken(userClaims, CreateSigningCredentials(), expiration);

            var tokenHandler = new JwtSecurityTokenHandler();

            return new AuthenticationResponse
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = expiration
            };
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
            new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private List<Claim> CreateClaims(AppUser user)
        {
            var authClaims = new List<Claim>  {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim("userid", user.Id),
                new Claim("username", user.UserName),
                new Claim("email", user.Email)
            };
            return authClaims;
        }
        private SigningCredentials CreateSigningCredentials() =>
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
                ),
                SecurityAlgorithms.HmacSha256
            );
    }
}