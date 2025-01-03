using CitiesOnMap.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CitiesOnMap.Application.Interfaces.Identity;

public interface IUserManager
{
    Task<User?> FindByIdAsync(string userId);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByNameAsync(string userName);
    Task<User?> FindByLoginAsync(string loginProvider, string providerKey);
    Task<IdentityResult> CreateAsync(User user);
    Task<IdentityResult> AddPasswordAsync(User user, string password);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<IdentityResult> AddToRoleAsync(User user, string role);
    Task<IdentityResult> AddLoginAsync(User user, UserLoginInfo login);
    Task<IList<string>> GetRolesAsync(User user);
    Task<string?> GetAuthenticationTokenAsync(User user, string loginProvider, string tokenName);

    Task<IdentityResult> SetAuthenticationTokenAsync(User user, string loginProvider, string tokenName,
        string? tokenValue);

    Task<bool> VerifyUserTokenAsync(User user, string tokenProvider, string purpose, string token);
}