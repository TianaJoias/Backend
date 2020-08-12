using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Security
{
    public interface IPublicTokenBuilder
    {
        IPublicTokenBuilder AddSubject(string subject);
        IPublicTokenBuilder AddRole(string role);
        IPublicTokenBuilder AddCustomRole(string roleName, string roleValue);
        string Build();
    }

    public class TokenBuilder: IPublicTokenBuilder
    {
        private string _issuer;
        private string _audience;
        private DateTime _expires;
        private SigningCredentials _credentials;
        private SymmetricSecurityKey _key;
        private List<Claim> _claims = new List<Claim>();
        private EncryptingCredentials _encryptingCredentials;

        public TokenBuilder()
        {

        }
        public static TokenBuilder Given() => new TokenBuilder();

        public TokenBuilder AddClaims(List<Claim> claims)
        {
            if (_claims == null)
                _claims = claims;
            else
                _claims.AddRange(claims);
            return this;
        }

        public TokenBuilder AddClaim(Claim claim)
        {
            if (_claims == null)
                _claims = new List<Claim>() { claim };
            else
                _claims.Add(claim);
            return this;
        }

        public TokenBuilder AddIssuer(string issuer)
        {
            _issuer = issuer;
            return this;
        }

        public TokenBuilder AddAudience(string audience)
        {
            _audience = audience;
            return this;
        }

        public TokenBuilder AddExpiry(int minutes)
        {
            _expires = DateTime.Now.AddMinutes(minutes);
            return this;
        }

        public TokenBuilder AddKey(string key)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            _credentials = new SigningCredentials(_key,
            SecurityAlgorithms.HmacSha256);
            return this;
        }

        public TokenBuilder AddEncryptingKey(string key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            _encryptingCredentials = new EncryptingCredentials(securityKey, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);
            return this;
        }

        public IPublicTokenBuilder AddSubject(string subject)
        {
            AddClaim(new Claim(JwtRegisteredClaimNames.Sub, subject));
            return this;
        }

        public IPublicTokenBuilder AddRole(string role)
        {
            AddClaim(new Claim(ClaimTypes.Role, role));
            return this;
        }

        public string Build()
        {
            //https://www.scottbrady91.com/C-Sharp/JSON-Web-Encryption-JWE-in-dotnet-Core
            //https://leastprivilege.com/2016/08/21/why-does-my-authorize-attribute-not-work/
            var id = new ClaimsIdentity(_claims, "authenticationType", "name", ClaimTypes.Role);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _audience,
                Issuer = _issuer,
                Expires= _expires,
                NotBefore = DateTime.Now,
                SigningCredentials= _credentials,
                Subject = new ClaimsIdentity(id),
                EncryptingCredentials = _encryptingCredentials,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenstring = tokenHandler.WriteToken(token);
            return tokenstring;
        }

        public IPublicTokenBuilder AddCustomRole(string roleName, string roleValue)
        {
            AddClaim(new Claim(roleName, roleValue));
            return this;
        }
    }
}

