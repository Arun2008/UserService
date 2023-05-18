using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using UserService.Models.Common;
using Req=UserService.Models.Request;
using UserService.Repository.Base;
using UserService.Validator;
using System.Text.Json;

namespace UserService.Controllers
{
    public class BaseController : ControllerBase, IActionFilter, IExceptionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        public BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Method intentionally left empty.
        }

        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            ErrorResponse? errorResponse;
            var result = context.RouteData.Values.Select(x => new ActionController { key = x.Key, value = Convert.ToString(x.Value) }).ToList();

            if (result.Any() && result[0].key == "action" && result[1].key == "controller" && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var RoleIds = JsonSerializer.Deserialize<List<Ids>>(User.FindFirstValue("roleid"));
                
                Req.VerifyPermission verifyPermission = new()
                {
                    //RoleId = Guid.Parse(User.FindFirstValue("roleid")),
                    Controller = result[1].value,
                    Action = result[0].value
                };
                bool flage = (from p in _unitOfWork.GetContext().Permissions
                                   join rp in _unitOfWork.GetContext().RolePermissions on p.Id equals rp.PermissionId
                                   
                                   where p.ControllerName == verifyPermission.Controller && p.ActionName == verifyPermission.Action
                                   select p).Any();
                if (flage)
                {
                    // Method intentionally left empty.
                }
                else
                {
                    errorResponse = new() { ErrorCode = 403, Message = "Access Denied!" };
                    context.Result = new ObjectResult(errorResponse);
                }
            }
        }

        [NonAction]
        public void OnException(ExceptionContext context)
        {
            // Method intentionally left empty.
        }
    }
}
