
using Req = UserService.Models.Request;
using Res = UserService.Models.Response;

namespace UserService.Repository.Interfaces
{
    public interface IRolePermissionRepository
    {
        public  Task<bool> CreateRole(Req.CreateRole role);
        public Task<bool> AssignRole(Req.AssignRole assign);
        public Task<bool> CreatePermission(Req.CreatePermission permission);
        public Task<bool> CreateRolePermission(Req.CreateRolePermission rolePermission);
        public Task<List<Res.ApplicationRole>?> GetAllRoles();
        public Task<List<Res.Permission>?> GetAllPermissions();
        public Task<List<Res.RolePermission>?> GetAllRolePermissions();
        public Task<bool> ChangeStatus(Req.ChangeRoleStatus status);
    }
}
