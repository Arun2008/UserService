using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Filters;
using UserService.Models.DBModel;
using UserService.Repository.Interfaces;
using UserService.Validator;
using Req = UserService.Models.Request;
using Res = UserService.Models.Response;
using DM = UserService.Models.DBModel;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using UserService.Models.Common;
using UserService.Models;
using UserService.Validator.Interfaces;
using UserService.Repository.Base;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class RolePermissionController : ControllerBase
    {
        private readonly ILogger<RolePermissionController> _logger;
        private readonly RoleManager<ApplicationUserRole> _roleManager;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IRolePermissionValidation _validation;
        public RolePermissionController(RoleManager<ApplicationUserRole> roleManager, ILogger<RolePermissionController> logger, IRolePermissionRepository rolePermissionRepository,
            IRolePermissionValidation validation)
        {
            _logger = logger;
            _roleManager = roleManager;
            _rolePermissionRepository = rolePermissionRepository;
            _validation = validation;
        }

       
        [HttpPost]
        [Route("createrole")]
        public async Task<IActionResult> CreateRole(Req.CreateRole role)
        {
            

            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateRoleValidator.ValidateAsync(role);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                var result = await _rolePermissionRepository.CreateRole(role);
                if (result)
                {
                    _logger.LogInformation("Role created successfully!");
                    return Ok(Result<DBNull>.Success("Role created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Role not created!");
                    return Ok(Result<DBNull>.Failure("Role not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt {0} RequestIdentifier {1} Exception{2}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
        }

        [HttpPost]
        [Route("assignrole")]
        public async Task<IActionResult> AssignRole(Req.AssignRole assign)
        {


            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.AssignRoleValidator.ValidateAsync(assign);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                var result = await _rolePermissionRepository.AssignRole(assign);
                if (result)
                {
                    _logger.LogInformation("Role assigned successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Role assigned successfully!"));
                }
                else
                {
                    _logger.LogCritical("Role not assigned!");
                    return Ok(ResponseResult<DBNull>.Failure("Role not assigned!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt {0} RequestIdentifier {1} Exception{2}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
        }

        [HttpPost]
        [Route("unassignrole")]
        public async Task<IActionResult> UnssignRole(Req.AssignRole assign)
        {


            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.AssignRoleValidator.ValidateAsync(assign);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                var result = await _rolePermissionRepository.AssignRole(assign);
                if (result)
                {
                    _logger.LogInformation("Role assigned successfully!");
                    return Ok(ResponseResult<DBNull>.Success("Role assigned successfully!"));
                }
                else
                {
                    _logger.LogCritical("Role not assigned!");
                    return Ok(ResponseResult<DBNull>.Failure("Role not assigned!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt {0} RequestIdentifier {1} Exception{2}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
        }

        [HttpPost]
        [Route("createpermission")]
        public async Task<IActionResult> CreatePermission(Req.CreatePermission permission)
        {


            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreatePermissionValidator.ValidateAsync(permission);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                var result = await _rolePermissionRepository.CreatePermission(permission);
                if (result)
                {
                    _logger.LogInformation("Role created successfully!");
                    return Ok(Result<DBNull>.Success("Role created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Role not created!");
                    return Ok(Result<DBNull>.Failure("Role not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt {0} RequestIdentifier {1} Exception{2}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
        }

        [HttpPost]
        [Route("createrolepermission")]
        public async Task<IActionResult> CreateRolePermission(Req.CreateRolePermission rolePermission)
        {


            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.CreateRolePermissionValidator.ValidateAsync(rolePermission);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion

                var result = await _rolePermissionRepository.CreateRolePermission(rolePermission);
                if (result)
                {
                    _logger.LogInformation("Role created successfully!");
                    return Ok(Result<DBNull>.Success("Role created successfully!"));
                }
                else
                {
                    _logger.LogCritical("Role not created!");
                    return Ok(Result<DBNull>.Failure("Role not created!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt {0} RequestIdentifier {1} Exception{2}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "createrole", ex);
                return BadRequest(errorResponse);
            }
        }
        
        [HttpPost]
        [Route("getallroles")]
        public async Task<IActionResult> GetAllRoles()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _rolePermissionRepository.GetAllRoles();
                if (result != null)
                {
                    _logger.LogInformation("Retrive all roles successfully!");
                    return Ok(Result<List<Res.ApplicationRole>>.Success("Retrive all roles successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Roles not found!");
                    return Ok(Result<DBNull>.Failure("Roles not found!" ));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "getallroles", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "getallroles", ex);
                return BadRequest(errorResponse);
            }
        }

        [HttpPost]
        [Route("getallpermissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _rolePermissionRepository.GetAllPermissions();
                if (result != null)
                {
                    _logger.LogInformation("Retrive all permission successfully!");
                    return Ok(Result<List<Res.Permission>>.Success("Retrive all permission successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Roles not found!");
                    return Ok(Result<DBNull>.Failure("Roles not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "getallpermissions", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "getallpermissions", ex);
                return BadRequest(errorResponse);
            }
        }

        [HttpPost]
        [Route("getallrolepermissions")]
        public async Task<IActionResult> GetAllRolePermissions()
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _rolePermissionRepository.GetAllRolePermissions();
                if (result != null)
                {
                    _logger.LogInformation("Retrive all role permission successfully!");
                    return Ok(Result<List<Res.RolePermission>>.Success("Retrive all role permission successfully!", result));
                }
                else
                {
                    _logger.LogCritical("Role permission not found!");
                    return Ok(Result<DBNull>.Failure("Role permission not found!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "getallrolepermissions", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "getallrolepermissions", ex);
                return BadRequest(errorResponse);
            }
        }

        [HttpPost]
        [Route("changestatus")]
        public async Task<IActionResult> ChangeStatus(Req.ChangeRoleStatus status)
        {
            ErrorResponse? errorResponse;
            try
            {
                #region Validate Request Model
                var validation = await _validation.ChangeRoleStatusValidator.ValidateAsync(status);
                errorResponse = CustomResponseValidator.CheckModelValidation(validation);
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }
                #endregion 

                var result = await _rolePermissionRepository.ChangeStatus(status);
                if (result)
                {
                    _logger.LogInformation("Role status update successfully!");
                    return Ok(Result<DBNull>.Success("Role status update successfully!"));
                }
                else
                {
                    _logger.LogCritical("Role does not exist!");
                    return Ok(Result<DBNull>.Failure("Role does not exist!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt {0} RequestIdentifier {1} Exception{2}", DateTime.Now, "changestatus", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"

                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "changestatus", ex);
                return BadRequest(errorResponse);
            }
        }
    }
}
