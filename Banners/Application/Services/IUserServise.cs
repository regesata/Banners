using Application.Models;

namespace Application.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<RegisterResponse> Register(RegisterRequest model);
        Task<RoleResponse> GetUserRole(RoleRequest model);

    }
}
