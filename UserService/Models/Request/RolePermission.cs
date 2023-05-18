using System.Text.Json.Serialization;
using UserService.Models.Common;

namespace UserService.Models.Request
{

    public class CreateRole
    {
        public string? RoleName { get; set; }
    }
    public class AssignRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
    public class CreatePermission
    {
        public string? ModuleName { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
    }

    public class CreateRolePermission
    {
        public CreateRolePermission()
        {
            Permissions ??= new();
        }
        public Guid RoleId { get; set; }
        public List<Permission> Permissions { get; set; }
    }

    public class Permission
    {
        public Guid PermissionId { get; set; }
    }
    public class ChangeRoleStatus
    {
        public Guid RoleId { get; set; }
        public Status Status { get; set; }
    }
    public class ChangePermissionStatus
    {
        public Guid PermissionId { get; set; }
        public Status Status { get; set; }
    }


}
