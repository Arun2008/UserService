using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using UserService.Models.DBModel;

namespace UserService.Security
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<ApplicationUserRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenService(IConfiguration configuration, RoleManager<ApplicationUserRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<string> GenerateTokenStringAsync(ApplicationUser user)
        {

            var jwtAuth = _configuration.GetSection("Authentication:JWT");
            var claims = await GetClaims(user);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuth["Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                jwtAuth["ValidIssuer"],
                jwtAuth["ValidAudience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(jwtAuth["ExpireIn"])),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        public async Task<object> GenerateTokenAsync(ApplicationUser user)
        {

            var jwtAuth = _configuration.GetSection("Authentication:JWT");
            var claims = await GetClaims(user);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuth["Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                jwtAuth["ValidIssuer"],
                jwtAuth["ValidAudience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(jwtAuth["ExpireIn"])),
                signingCredentials: credentials);
            return new { access_token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor) };
        }
        public async Task<bool> IsTokenValid(string token)
        {
            var jwtAuth = _configuration.GetSection("Authentication:JWT");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuth["Secret"]));
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtAuth["ValidIssuer"],
                    ValidAudience = jwtAuth["ValidAudience"],
                    IssuerSigningKey = securityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            List<Claim> claims = new();
            claims.Add(new Claim(type: "employeeid", value: user.Id));
            claims.Add(new Claim(type: "userid", value: user.Id));
            claims.Add(new Claim(type: "companyid", value: user.Id));
            claims.Add(new Claim(type: "orgid", value: user.Id));
            claims.Add(new Claim(type: "displayname", value: user.Name));
            claims.Add(new Claim(ClaimTypes.Name, user.Email));
            foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            #region RND
            //claims.Add(new Claim(type: "name", value: user.Name));
            //claims.Add(new Claim(type: "email", value: user.Email));
            //foreach (var role in roles) claims.Add(new Claim("role", role.ToString()));
            //string jsonToken = JsonSerializer.Serialize(claims);
            //var protectionProvider = DataProtectionProvider.Create("IdentityServer:keyring");
            //var dataProtector = protectionProvider.CreateProtector(jsonToken);
            //var ticketFormat = new TicketDataFormat(dataProtector);
            //var payload = protectionProvider.CreateProtector("<your purpose string here>").Protect(plainText); 
            #endregion


            return claims;
        }
    }
}
