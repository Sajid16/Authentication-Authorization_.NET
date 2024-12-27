using Authentication_Authorization.Entities;
using Authentication_Authorization.Utilities;
using Authentication_Authorization.ViewModels;

namespace Authentication_Authorization.Services
{
    public interface IAuthenticationServices
    {
        Task<ApiReturnObj<object>> RegisterUsers(RegisteruserVM registeruserVM);
        Task<ApiReturnObj<List<UserVM>>> GetUsers();
        Task<ApiReturnObj<LogInResponseVM>> LogIn(LogInRequestVM logInRequestVM);
    }
}
