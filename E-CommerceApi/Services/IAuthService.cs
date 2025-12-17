using E_CommerceApi.DTOs;

namespace E_CommerceApi.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterDto model);
        Task<AuthModel> GetTokenAsync(LoginDto model);
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}
