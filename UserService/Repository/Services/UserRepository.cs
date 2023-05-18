using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Models.DBModel;
using UserService.Repository.Base;
using UserService.Repository.Interfaces;

namespace UserService.Repository.Services
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
            //if (roleId != Guid.Empty)
            //{
            //    return await _unitOfWork.GetContext().Roles.Where(x => x.Id == roleId.ToString()).FirstOrDefaultAsync();
            //}

            return null;
        }

    
    }
}
