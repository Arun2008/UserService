using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
using AuthenticationService.Security;
using AuthenticationService.Models.DBModel;
using AuthenticationService.Models.Request;
using AuthenticationService.Filters;
using AuthenticationService.Repository.Interfaces;
using AuthenticationService.Validator.Interfaces;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationUserRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IApplicationUserValidation _validation;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationUserRole> roleManager,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IUserRepository userRepository,
            IApplicationUserValidation validation)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _validation = validation;

        }

        #region Login Authenticate
        private async Task LoginAuthenticate(ApplicationUser user)
        {
            #region Login Authenticate
            if (user != null)
            {
                var jwtAuth = _configuration.GetSection("Authentication:JWT");
                var claims = await _tokenService.GetClaims(user);
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(jwtAuth["ExpireIn"])),
                });
            }

            #endregion
        }
        #endregion

        #region Register
        [HttpPost]
        [Route("register")]
        [Authorize(Policy = "PublicSecure")]
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseResult<DBNull>.Failure("User already exists."));

            ApplicationUser user = new ApplicationUser()
            {
                Name = model.Name,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseResult<DBNull>.Failure("User creation failed! Please check user details and try again."));

            if (model.RoleId != Guid.Empty)
            {
                var role = await _userRepository.GetRoleById(model.RoleId);
                if (role == null)
                    return Ok(ResponseResult<DBNull>.Failure("Role not exist!"));
                await _userManager.AddToRoleAsync(user, role.Name);
            }

            return Ok(ResponseResult<DBNull>.Success("User created successfully!!"));
        }

        #endregion

        #region Login

        [HttpPost]
        [Route("login")]
        [Authorize(Policy = "PublicSecure")]
        public async Task<IActionResult> Login(UserLogin model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await LoginAuthenticate(user);
                return Ok(await _tokenService.GenerateTokenAsync(user));
            }
            return Unauthorized();
        }


        #endregion

        #region External login With Return URL

        [HttpPost]
        [Route("external-login-url")]
        [Authorize(Policy = "PublicSecure")]
        public async Task<IActionResult> ExternalLogins(string? returnUrl = null)
        {
            returnUrl = returnUrl == null ? Request.Scheme + "://" + Request.Host : returnUrl;
            IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            List<ExternalUserLogin> logins = new List<ExternalUserLogin>();
            foreach (var scheme in schemes)
            {
                logins.Add(new ExternalUserLogin
                {
                    Name = scheme.Name,
                    Url = Url.ActionLink("ExternalLogin", "Account",
                    new
                    {
                        provider = scheme.Name,
                        returnUrl
                    })
                });
            }
            return Ok(logins);
        }

        [HttpPost]
        [Route("external-login")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public IActionResult ExternalLogin(string provider, string? error = null, string? returnUrl = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpPost]
        [Route("external-login-callback")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("Error loading external login information.");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser? user = null;
            if (email != null)
            {
                user = await _userManager.FindByNameAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    return Ok("Email not confirmed yet");
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded && user != null)
            {

                //await LoginAuthenticate(user);
                //return Ok(await _tokenService.GenerateTokenAsync(user));



                // Add the cookie to the response cookie collection
                Response.Cookies.Append("token", await _tokenService.GenerateTokenStringAsync(user), new CookieOptions { Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddMinutes(5) });
                return Redirect(returnUrl);

            }
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            Name = info.Principal.FindFirstValue(ClaimTypes.Name),
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            EmailConfirmed = true
                        };
                        await _userManager.CreateAsync(user);
                        #region Token for email confirmation
                        //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme); 
                        #endregion
                    }
                    IdentityResult result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        //await LoginAuthenticate(user);
                        //return Ok(await _tokenService.GenerateTokenAsync(user));

                        // Add the cookie to the response cookie collection
                        Response.Cookies.Append("token", await _tokenService.GenerateTokenStringAsync(user), new CookieOptions { Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddMinutes(5) });
                        return Redirect(returnUrl);


                    }
                    return Ok("External user registration failed.");

                }
                return Ok("External loading failed.");
            }
        }
        #endregion

        #region External login

        //[HttpGet]
        //[Route("external-login-url")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLogins()
        //{
        //    IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        //    List<ExternalUserLogin> logins = new List<ExternalUserLogin>();
        //    foreach (var scheme in schemes)
        //    {
        //        logins.Add(new ExternalUserLogin
        //        {
        //            Name = scheme.Name,
        //            Url = Url.ActionLink("ExternalLogin", "Account", new { provider = scheme.Name, })
        //        });
        //    }
        //    return Ok(logins);
        //}

        //[HttpGet]
        //[Route("external-login")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLogin(string provider, string? error = null)
        //{
        //    if (error != null)
        //    {
        //        return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
        //    }
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        IEnumerable<AuthenticationScheme> schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        //        if (schemes.Any(x => x.Name == provider))
        //        {
        //            var redirectUrl = Url.Action(nameof(ExternalLogin), "Account", new { provider });
        //            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //            return Challenge(properties, provider);
        //        }
        //        return Ok("Invalid provider");
        //    }
        //    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //    ApplicationUser? user = null;
        //    if (email != null)
        //    {
        //        user = await _userManager.FindByNameAsync(email);
        //        if (user != null && !user.EmailConfirmed)
        //        {
        //            return Ok("Email not confirmed yet");
        //        }
        //    }

        //    var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        //    if (signInResult.Succeeded && user != null)
        //    {

        //        //await LoginAuthenticate(user);
        //        //return Redirect(returnUrl);
        //        return Ok(await _tokenService.GenerateTokenAsync(user));
        //    }
        //    else
        //    {
        //        if (email != null)
        //        {
        //            if (user == null)
        //            {
        //                user = new ApplicationUser
        //                {
        //                    Name = info.Principal.FindFirstValue(ClaimTypes.Name),
        //                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
        //                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
        //                    EmailConfirmed = true
        //                };
        //                await _userManager.CreateAsync(user);
        //                #region Token for email confirmation
        //                //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //                //var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme); 
        //                #endregion
        //            }
        //            IdentityResult result = await _userManager.AddLoginAsync(user, info);
        //            if (result.Succeeded)
        //            {
        //                //await LoginAuthenticate(user);
        //                //return Redirect(returnUrl);
        //                return Ok(await _tokenService.GenerateTokenAsync(user));
        //            }
        //            return Ok("External user registration failed.");

        //        }
        //        return Ok("External loading failed.");
        //    }
        //}
        #endregion

        #region Logout

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [Route("logout")]
        //[Authorize(Policy = "PublicSecure")]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            returnUrl = returnUrl == null ? Request.Scheme + "://" + Request.Host : returnUrl;
            if (User?.Identity?.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync();
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
            return Redirect(returnUrl);
        }
        #endregion

        #region Confirm Email
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return LocalRedirect("~/api/account/login");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest($"The User ID {userId} is invalid");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest("Email cannot be confirmed");
        }
        #endregion

    }

}
