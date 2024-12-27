using Authentication_Authorization.Filters;
using Authentication_Authorization.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomAuthenticationFilter))]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public async Task<IActionResult> AnonymousMethod()
        {
            return new ObjectResult(new ApiReturnObj<object> {
                IsSuccess = true,
                Message = "Response from anonymous method."
            }) 
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        [HttpGet]
        [Route("not-anonymous")]
        public async Task<IActionResult> NotAnonymousMethod()
        {
            return new ObjectResult(new ApiReturnObj<object>
            {
                IsSuccess = false,
                Message = "Response from not anonymous method."
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
