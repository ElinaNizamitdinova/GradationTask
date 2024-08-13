using Microsoft.AspNetCore.Mvc;
using static UserService.Models.UserRepository;

namespace UserService.Interfaces
{
    public interface IUserRepository
    {
        Task<IActionResult> AddAdministratorAsync(UserDto user);
        Task<IActionResult> AddUserAsync(UserDto user);
        Task<IActionResult> GetUserListAsync();
        Task<IActionResult> RemoveUserAsync(Guid id);
        Task<IActionResult> GetMyId();
    }
}
