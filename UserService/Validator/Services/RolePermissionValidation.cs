using UserService.Validator.CoreValidator;
using UserService.Validator.Interfaces;

namespace UserService.Validator.Services
{
    public class RolePermissionValidation: IRolePermissionValidation
    {
        public CreateRoleValidator CreateRoleValidator { get; set; } = new();
        public AssignRoleValidator AssignRoleValidator { get; set; }=new();
        public CreatePermissionValidator CreatePermissionValidator { get; set; } = new();
        public CreateRolePermissionValidator CreateRolePermissionValidator { get; set; }=new();
        public ChangeRoleStatusValidator ChangeRoleStatusValidator { get; set; }= new();
    }
}
