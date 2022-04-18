using Tangy_Models;

namespace TangyWeb_Client.Service.IService
{
    public interface IAuthenticationService
    {
        Task<SignUpResponseDTO> RegisterUser(SignUpRequestDTO signUp);
        Task<SignInResponseDTO> Login(SignInRequestDTO signIn);
        Task Logout();
    }
}
