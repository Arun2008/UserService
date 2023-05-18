using AuthenticationService.Validator.CoreValidator;
using AuthenticationService.Validator.Interfaces;

namespace AuthenticationService.Validator.Services
{
    public class ApplicationUserValidation: IApplicationUserValidation
    {
        public RegisterUserValidator RegisterUserValidator { get; set; } = new();
    }
}
