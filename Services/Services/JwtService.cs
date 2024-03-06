using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Common;
using Entities.Framework;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Services.Services;

namespace Services.Services
{
    public class JwtService : IJwtService, IScopedDependency
    {
        private readonly SiteSettings _siteSetting;
        //private readonly SignInManager<User> signInManager;

        public JwtService(IOptionsSnapshot<SiteSettings> settings/*, SignInManager<User> signInManager*/)
        {
            _siteSetting = settings.Value;
            //this.signInManager = signInManager;
        }

        public async Task<AccessToken> GenerateAsync(User user, string clientId)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.EncryptKey);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptKey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            //var certificate = new X509Certificate2("d:\\aaaa2.cer"/*, "P@ssw0rd"*/);
            //var encryptingCredentials = new X509EncryptingCredentials(certificate);

            var claims = await _getClaimsAsync(user, clientId);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            return new AccessToken(securityToken);
        }

        private static Task<IEnumerable<Claim>> _getClaimsAsync(User user, string clientId)
        {
            if (user == null)
                return Task.FromResult<IEnumerable<Claim>>(null);

            //var result = await signInManager.ClaimsFactory.CreateAsync(user);
            var list = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.UserData, clientId),
            };

            return Task.FromResult<IEnumerable<Claim>>(list);
        }
    }
}
