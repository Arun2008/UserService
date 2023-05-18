using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Req = UserService.Models.Request;
namespace UserService.Validator.CoreValidator
{
   
    public class CreateRoleValidator : AbstractValidator<Req.CreateRole>
    {
        public CreateRoleValidator()
        {
            RuleFor(prop => prop.RoleName).NotEmpty().NotNull().WithMessage("RoleName is required!").MaximumLength(50).WithMessage("RoleName must be less than 50 characters!").Matches("^([a-zA-Z0-9]+\\s)*[a-zA-Z0-9]+$").WithMessage("RoleName is invalid! it will allow alphanumeric only!");
        }
    }
    public class CreatePermissionValidator : AbstractValidator<Req.CreatePermission>
    {
        public CreatePermissionValidator()
        {
            RuleFor(prop => prop.ModuleName).NotEmpty().NotNull().WithMessage("ModuleName is required!").MaximumLength(50).WithMessage("ModuleName must be less than 50 characters!").Matches("^([a-zA-Z]+\\s)*[a-zA-Z]+$").WithMessage("ModuleName is invalid! it will allow alphabets only!");
            RuleFor(prop => prop.ControllerName).NotEmpty().NotNull().WithMessage("ControllerName is required!").MaximumLength(50).WithMessage("ControllerName must be less than 50 characters!").Matches("^([a-zA-Z]+\\s)*[a-zA-Z]+$").WithMessage("ControllerName is invalid! it will allow alphabets only!");
            RuleFor(prop => prop.ActionName).NotEmpty().NotNull().WithMessage("ActionName is required!").MaximumLength(50).WithMessage("ActionName must be less than 50 characters!").Matches("^([a-zA-Z]+\\s)*[a-zA-Z]+$").WithMessage("ActionName is invalid! it will allow alphabets only!");
        }
    }
    public class AssignRoleValidator : AbstractValidator<Req.AssignRole>
    {
        public AssignRoleValidator()
        {
            RuleFor(prop => prop.RoleId).NotEmpty().NotNull().WithMessage("RoleId is required!");
            RuleFor(prop => prop.UserId).NotEmpty().NotNull().WithMessage("UserId is required!");
        }
    }
    public class PermissionIdValidator : AbstractValidator<Req.Permission>
    {
        public PermissionIdValidator()
        {
            RuleFor(prop => prop.PermissionId).NotEmpty().NotNull().WithMessage("PermissionId is required!");
        }
    }
    public class CreateRolePermissionValidator : AbstractValidator<Req.CreateRolePermission>
    {
        public CreateRolePermissionValidator()
        {
            RuleFor(prop => prop.RoleId).NotEmpty().NotNull().WithMessage("RoleId is required!");
            RuleFor(prop => prop.Permissions).NotEmpty().NotNull().WithMessage("Permission Ids is required!");
            RuleForEach(prop => prop.Permissions).SetValidator(new PermissionIdValidator());
        }
    }
    public class ChangeRoleStatusValidator : AbstractValidator<Req.ChangeRoleStatus>
    {
        public ChangeRoleStatusValidator()
        {
            RuleFor(prop => prop.RoleId).NotEmpty().NotNull().WithMessage("RoleId is required!");
            RuleFor(prop => prop.Status).NotEmpty().NotNull().IsInEnum().WithMessage("Status is required!");
        }
    }
    
}
