
using UserService.Repository.Interfaces;
using Req = UserService.Models.Request;
using Res = UserService.Models.Response;
using DM = UserService.Models.DBModel;
using UserService.Models;
using UserService.Repository.Base;
using Microsoft.AspNetCore.Identity;
using UserService.Models.DBModel;
using UserService.Filters;
using UserService.Models.Common;
using UserService.Models.Request;
using UserService.Models.Response;
using System.Security;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace UserService.Repository.Services
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly UserManager<DM.ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationUserRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CurrentUser CurrentUser;
        public RolePermissionRepository(IUnitOfWork unitOfWork, RoleManager<ApplicationUserRole> roleManager, UserManager<DM.ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            CurrentUser = _unitOfWork.GetCurrentUser();
            _userManager = userManager;
        }
        public async Task<bool> CreateRole(Req.CreateRole role)
        {
            if (!await _roleManager.RoleExistsAsync(role.RoleName))
            {
                await _roleManager.CreateAsync(new ApplicationUserRole { Name = role.RoleName, Status = Status.Active });
                return true;
            }
            return false;
        }

        public async Task<bool> AssignRole(Req.AssignRole assign)
        {
            var role = await _unitOfWork.Repository<DM.ApplicationUserRole>().GetSingle(x => x.Id == assign.RoleId.ToString());
            if (role == null)
                return false;
            var user = await _unitOfWork.Repository<DM.ApplicationUser>().GetSingle(x => x.Id == assign.UserId.ToString());
            if (user == null)
                return false;
            await _userManager.AddToRoleAsync(user, role.Name);
            return true;
        }
        public async Task<bool> CreatePermission(Req.CreatePermission permission)
        {
            if (permission == null)
                return false;
            var result = new DM.Permission
            {
                ModuleName = permission.ModuleName,
                ControllerName = permission.ControllerName,
                ActionName = permission.ActionName,
            };
            return await _unitOfWork.Repository<DM.Permission>().Insert(result) > 0;
        }
        public async Task<bool> CreateRolePermission(CreateRolePermission rolePermission)
        {
            if (rolePermission.Permissions.Count == 0)
                return false;

            List<DM.RolePermission> result = new();
            foreach (var item in rolePermission.Permissions)
            {
                result.Add(new DM.RolePermission
                {
                    RoleId = rolePermission.RoleId,
                    PermissionId = item.PermissionId,

                });

            }
            return await _unitOfWork.Repository<DM.RolePermission>().Insert(result) > 0;
        }


        public async Task<List<ApplicationRole>?> GetAllRoles()
        {
            var result = await _unitOfWork.Repository<ApplicationUserRole>().Get(x => x.Status == Status.Active);
            if (result.Count == 0)
                return null;
            return result.Select(x => new Res.ApplicationRole
            {
                RoleId = Guid.Parse(x.Id),
                RoleName = x.Name,
            }).ToList();
        }

        public async Task<List<Res.Permission>?> GetAllPermissions()
        {
            var result = await _unitOfWork.Repository<DM.Permission>().Get();
            if (result.Count == 0)
                return null;
            return result.Select(x => new Res.Permission
            {
                PermissionId = x.Id,
                ModuleName = x.ModuleName,
                ControllerName = x.ControllerName,
                ActionName = x.ActionName
            }).ToList();
        }

        public async Task<List<Res.RolePermission>?> GetAllRolePermissions()
        {
            var roles = await _unitOfWork.Repository<ApplicationUserRole>().Get();
            if (roles.Count == 0) return null;

            var ctx = _unitOfWork.GetContext();
            List<Res.RolePermission> result = new();
            foreach (var item in roles)
            {
                var module = (from p in ctx.Permissions
                              join r in ctx.RolePermissions on p.Id equals r.PermissionId
                              where r.RoleId == new Guid(item.Id)
                              select new
                              {
                                  p.ModuleName,
                              }).Distinct().ToList();
                var users = await _userManager.GetUsersInRoleAsync(item.Name);
                Res.RolePermission data = new();
                data.RoleId = Guid.Parse(item.Id);
                data.RoleName = item.Name;
                data.ModuleAccess = string.Join(",", module.Select(p => p.ModuleName));
                data.NoOfUser = users.Count;
                data.UserList = users.Select(x => new UserData
                {
                    Name = x.Name,
                    EmailId = x.Email,
                    RoleName = item.Name,
                    Contact = x.PhoneNumber
                }).ToList();
                result.Add(data);
            }
            return result;
        }

        public async Task<bool> ChangeStatus(ChangeRoleStatus status)
        {

            var role = await _unitOfWork.Repository<ApplicationUserRole>().GetSingle(x => x.Id == status.RoleId.ToString());
            if (role == null)
                return false;
            role.Status = status.Status;
            IdentityResult result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
                return true;
            return false;
        }

        
    }
}
