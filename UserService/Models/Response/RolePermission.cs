using UserService.Models.Common;

namespace UserService.Models.Response
{

    public class ApplicationRole
    {
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
    }
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public string? ModuleName { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
    }
    public class RolePermission
    {
        public RolePermission()
        {
            UserList ??= new();
        }
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? ModuleAccess { get; set; }
        public int NoOfUser { get; set; }
        public List<UserData> UserList { get; set; }
    }

    public class UserData
    {
        public string? Name { get; set; }
        public string? RoleName { get; set; }
        public string? EmailId { get; set; }
        public string? Contact { get; set; }
    }

    public class ChangeStatusBand
    {
        public Status Status { get; set; }
    }
    public class PermissionResponse
    {
        public string RoleId { get; set; }
        public IList<RoleClaimss> RoleClaims { get; set; }
    }

    public class RoleClaimss : RoleClaimsList
    {
        public Guid RoleId { get; set; }
    }

    public class RoleClaimsList
    {
        public Guid PermissionId { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public bool Selected { get; set; }
    }
}
