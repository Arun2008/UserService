
using UserService.Repository.Services;
using Req = UserService.Models.Request;
using Res = UserService.Models.Response;
namespace UserService.Repository.Interfaces
{
    public interface IModuleVerificationRepository
    {
        public Task<Res.VerifyPermission> VerifyPermission(Req.VerifyPermission verifyPermission);
    }
}
