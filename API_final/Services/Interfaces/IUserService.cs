using API_final.DTOs;
namespace API_final.Services.Interfaces
{
    public interface IUserService
    {
        Task<String> RegisterAsync(RegisterDto dto);
        Task<String> LoginAsync(LoginDto dto);
    }
}
