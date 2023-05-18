using AuthenticationService.Validator.CoreValidator;

namespace AuthenticationService.Validator.Interfaces
{
    public interface IApplicationUserValidation
    {
        public RegisterUserValidator RegisterUserValidator { get; set; }
         
    }
}
