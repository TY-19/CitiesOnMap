using CitiesOnMap.Application.Interfaces.Identity;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Queries.Users.GetUser;

public class GetUserRequestHandler(
    IUserManager userManager
) : IRequestHandler<GetUserRequest, User?>
{
    public async Task<User?> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        User? user = null;
        if (request.UserId != null)
        {
            user = await userManager.FindByIdAsync(request.UserId);
        }

        if (user == null && request.UserName != null)
        {
            user = await userManager.FindByNameAsync(request.UserName);
        }

        if (user == null && request.Email != null)
        {
            user = await userManager.FindByNameAsync(request.Email);
        }

        if (user == null && request is { Provider: not null, ProviderKey: not null })
        {
            user = await userManager.FindByLoginAsync(request.Provider, request.ProviderKey);
        }

        return user;
    }
}