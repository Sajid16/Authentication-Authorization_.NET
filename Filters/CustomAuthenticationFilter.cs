using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Authentication_Authorization.Filters
{
    public class CustomAuthenticationFilter : IAsyncAuthorizationFilter
    {

        //Constructor to accept a list of roles(or any other parameters)
        public CustomAuthenticationFilter()
        {
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Check if the AllowAnonymous attribute is applied to the action or controller
            bool allowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Any(metadata => metadata is AllowAnonymousAttribute);

            if (allowAnonymous)
            {
                // Skip authorization if AllowAnonymous is present
                return;
            }


            // Get the token from the Authorization header
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (authorizationHeader is not null && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                // Validate token
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);

                    // Check expiration
                    if (jwtToken.ValidTo.AddHours(6) < DateTime.Now)
                    {
                        // Token is expired
                        context.Result = new UnauthorizedResult();
                        return;
                    }

                    // Token is valid; you can add further claims validation here if needed
                    // e.g. if anything that is embedded into claims and can validate through db that can be placed here
                }
                else
                {
                    // Invalid token
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                // No token provided
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        
    }
}
