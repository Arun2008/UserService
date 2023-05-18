

using AuthenticationService.Models.DBModel;

namespace AuthenticationService.Repository.Interfaces
{
    public interface IUserRepository
    {
        public Task<ApplicationUserRole?> GetRoleById(Guid roleId);
    }
}
