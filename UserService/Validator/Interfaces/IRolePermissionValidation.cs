using UserService.Validator.CoreValidator;

namespace UserService.Validator.Interfaces
{
    public interface IRolePermissionValidation
    {
        public CreateRoleValidator CreateRoleValidator { get; set; }
        public AssignRoleValidator AssignRoleValidator { get; set; }
        
        public CreatePermissionValidator CreatePermissionValidator { get; set; }
        public CreateRolePermissionValidator CreateRolePermissionValidator { get; set; }
        public ChangeRoleStatusValidator ChangeRoleStatusValidator { get; set; }
        


    }
}
