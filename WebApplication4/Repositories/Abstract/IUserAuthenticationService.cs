using WebApplication4.Models.DTO;

namespace WebApplication4.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(LoginModel model);
        Task<Status> RegisttrationAsync(RegistrationModel model);
        Task LogoutAsync();
    }
}
