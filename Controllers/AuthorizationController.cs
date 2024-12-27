using Authentication_Authorization.Attributes;
using Authentication_Authorization.Filters;
using Authentication_Authorization.Services;
using Authentication_Authorization.Utilities;
using Authentication_Authorization.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationServices;

        public AuthorizationController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
        }

        [HttpPost]
        [Route("Register-User")]
        public async Task<IActionResult> Register(RegisteruserVM registeruserVM)
        {
            if (!ModelState.IsValid)
            {
                ApiReturnObj<object> apiReturnObj = new ApiReturnObj<object>();
                apiReturnObj.Message = "validation error";
                apiReturnObj.IsSuccess = false;
                return BadRequest(apiReturnObj);
            }

            var response = await _authenticationServices.RegisterUsers(registeruserVM);
            return Ok(response);
        }

        [HttpGet]
        [Route("Get-Users")]
        [CustomAuthorization(nameof(ConstantValues.Roles.SuperAdmin), nameof(ConstantValues.Roles.Admin))]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _authenticationServices.GetUsers();
            return Ok(response);
        }

        [HttpPost]
        [Route("User-login")]
        public async Task<IActionResult> UserLogin(LogInRequestVM logInRequestVM)
        {
            var response = await _authenticationServices.LogIn(logInRequestVM);
            return Ok(response);
        }
    }
}
