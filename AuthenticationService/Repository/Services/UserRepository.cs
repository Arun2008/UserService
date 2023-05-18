using AuthenticationService.Models;
using AuthenticationService.Models.DBModel;
using AuthenticationService.Repository.Base;
using AuthenticationService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AuthenticationService.Repository.Services
{
    public class UserRepository: IUserRepository
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly CurrentUser CurrentUser;
        public UserRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            CurrentUser = _unitOfWork.GetCurrentUser();
        }

        /// <summary>
        /// Get role name by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<ApplicationUserRole?> GetRoleById(Guid roleId)
        {
            if (roleId != Guid.Empty)
            {
                return await _unitOfWork.GetContext().Roles.Where(x => x.Id == roleId.ToString()).FirstOrDefaultAsync();
            }

            return null;
        }

    
    }
}
