using Microsoft.EntityFrameworkCore;
using UserService.Repository.Base;
using UserService.Repository.Interfaces;
using Req = UserService.Models.Request;
using Res = UserService.Models.Response;
namespace UserService.Repository.Services
{
    public class ModuleVerificationRepository : IModuleVerificationRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ModuleVerificationRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// verify user's module permision 
        /// </summary>
        /// <param name="verifyPermission"></param>
        /// <returns></returns>
        public async Task<Res.VerifyPermission> VerifyPermission(Req.VerifyPermission verifyPermission)
        {
            Res.VerifyPermission result = new();
            if (verifyPermission.Action != string.Empty && verifyPermission.Controller != string.Empty && verifyPermission.RoleId != Guid.Empty)
            {
                bool flage = await (from p in _unitOfWork.GetContext().Permissions 
                                    join rp in _unitOfWork.GetContext().RolePermissions on p.Id equals rp.PermissionId 
                                    where rp.RoleId == verifyPermission.RoleId && p.ControllerName == verifyPermission.Controller && p.ActionName == verifyPermission.Action select p).AnyAsync();
                if (flage)
                {
                    result.AuthFlage = true;
                }
                    return result;
            }
            result.AuthFlage = false;
            return result;
        }
    }
}
