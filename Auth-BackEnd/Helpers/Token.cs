using Auth_BackEnd.DTOs;
using Auth_BackEnd.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Auth_BackEnd.Helpers
{
    public class Token
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;

        public Token(IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;

        }
        internal async Task<ActionResult<UserToken>> BuildTokenAsync(UserInfo userInfo)
        {

            var claims = new List<Claim>
            {
                new Claim("email", userInfo.Email),
                new Claim("TokenUserValue", "a6sASASdl545fkFDSGawobdw6pefnNPBDFNABPKDF12kjgdfyujnbvcxNBS6954LASNFBALSN55SDF5B4DF"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var usuario = await userManager.FindByEmailAsync(userInfo.Email);
            var roles = await userManager.GetClaimsAsync(usuario);

            foreach (var rol in roles)
            {
                claims.Add(rol);
            }
        //    claims.AddRange(roles);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMonths(1);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }


    }
}
