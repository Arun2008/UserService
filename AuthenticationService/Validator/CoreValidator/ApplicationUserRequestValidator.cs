using FluentValidation;
using Req = AuthenticationService.Models.Request;

namespace AuthenticationService.Validator.CoreValidator
{
    public class RegisterUserValidator : AbstractValidator<Req.RegisterUser>
    {
        public RegisterUserValidator()
        {
            RuleFor(prop => prop.Name).NotEmpty().NotNull().WithMessage("Name is required!").MaximumLength(100).WithMessage("Name must be less than 100 characters!").Matches("^([a-zA-Z]+\\s)*[a-zA-Z]+$").WithMessage("Name is invalid! it will allow alphabets only!");
            RuleFor(prop => prop.Email).NotEmpty().NotNull().EmailAddress().WithMessage("Email is required!");
            RuleFor(prop => prop.RoleId).NotEmpty().NotNull().WithMessage("RoleId is required!");
        }
    }
    
}
