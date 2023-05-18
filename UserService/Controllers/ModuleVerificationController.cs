using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Req = UserService.Models.Request;
using Res = UserService.Models.Response;
using UserService.Filters;
using UserService.Validator;
using UserService.Repository.Interfaces;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleVerificationController : ControllerBase
    {
        private readonly ILogger<ModuleVerificationController> _logger;
        private readonly IModuleVerificationRepository _moduleVerificationRepository;
        public ModuleVerificationController(ILogger<ModuleVerificationController> logger, IModuleVerificationRepository moduleVerificationRepository)
        {
            _logger = logger;
            _moduleVerificationRepository = moduleVerificationRepository;
        }

        [HttpPost]
        [Route("verifypermission")]
        public async Task<IActionResult> VerifyPermission(Req.VerifyPermission verifyPermission)
        {
            ErrorResponse? errorResponse;
            try
            {
                var result = await _moduleVerificationRepository.VerifyPermission(verifyPermission);
                if (result.AuthFlage)
                {
                    _logger.LogInformation("Authorized user!");
                    return Ok(Result<Res.VerifyPermission>.Success("Authorized user!", result));
                }
                else
                {
                    _logger.LogCritical("Unauthorized user!");
                    return Ok(Result<Res.VerifyPermission>.Failure("Unauthorized user!"));
                }
            }
            catch (DbUpdateException exp)
            {
                var ex = exp.InnerException as SqlException;
                errorResponse = ex != null ? new() { ErrorCode = Convert.ToInt32(ex.ErrorCode), Message = ex.Message } : new();
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "verifypermission", ex);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                errorResponse = new()
                {
                    ErrorCode = 500,
                    Message = "Something went wrong"
                };
                _logger.LogError("LoggingAt:{date} RequestIdentifier:{api} Exception:{ex}", DateTime.Now, "verifypermission", ex);
                return BadRequest(errorResponse);
            }
        }
    }
}
