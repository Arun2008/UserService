using AuthenticationService.Models.DBModel;
using System.Security.Claims;


namespace AuthenticationService.Security
{
    public interface ITokenService
    {
        public Task<string> GenerateTokenStringAsync(ApplicationUser user);
        public Task<object> GenerateTokenAsync(ApplicationUser user);
        public Task<bool> IsTokenValid(string token);
        public Task<List<Claim>> GetClaims(ApplicationUser user);
    }
}
