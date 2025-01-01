using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Interfaces.Identity;
using CitiesOnMap.Domain.Constants;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace CitiesOnMap.Application.Commands.Users.CreateUser;

public partial class CreateUserCommandHandler(
    IUserManager userManager
) : IRequestHandler<CreateUserCommand, OperationResult<User>>
{
    public async Task<OperationResult<User>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        List<bool> results = [];
        var user = new User
        {
            UserName = command.UserName,
            Email = GetValidEmail(command)
        };
        results.AddRange([
            (await userManager.CreateAsync(user)).Succeeded,
            await AddRolesAsync(user, command),
            await AddLoginInfoAsync(user, command),
            await AddPasswordAsync(user, command)
        ]);

        bool succeeded = results.All(x => x);
        return new OperationResult<User>(succeeded, user);
    }

    private static string GetValidEmail(CreateUserCommand command)
    {
        if (command.Email != null)
        {
            return command.Email;
        }

        string email = NonAlphaNumeric().Replace(command.UserName, "");
        if (email.Length < 4)
        {
            email += Random.Shared.Next(10000).ToString("D5");
        }

        return email + "@citom.local";
    }

    private async Task<bool> AddRolesAsync(User user, CreateUserCommand command)
    {
        bool succeeded = true;
        if (command.Roles?.Count == 0)
        {
            command.Roles.Add(Defaults.DefaultUserRole);
        }

        foreach (string role in command.Roles ?? [])
        {
            IdentityResult result = await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                succeeded = false;
            }
        }

        return succeeded;
    }

    private async Task<bool> AddLoginInfoAsync(User user, CreateUserCommand command)
    {
        string provider = command.Provider ?? Defaults.DefaultProvider;
        string providerKey = command.ProviderKey ?? command.Email ?? command.UserName;
        var loginInfo = new UserLoginInfo(provider, providerKey, provider);
        return (await userManager.AddLoginAsync(user, loginInfo)).Succeeded;
    }

    private async Task<bool> AddPasswordAsync(User user, CreateUserCommand command)
    {
        return command.Password == null || (await userManager.AddPasswordAsync(user, command.Password)).Succeeded;
    }

    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    private static partial Regex NonAlphaNumeric();
}