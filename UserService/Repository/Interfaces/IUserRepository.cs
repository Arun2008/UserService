using UserService.Models.DBModel;

namespace UserService.Repository.Interfaces
{
    public interface IUserRepository
    {
        public Task<ApplicationUserRole?> GetRoleById(Guid roleId);
    }
}
