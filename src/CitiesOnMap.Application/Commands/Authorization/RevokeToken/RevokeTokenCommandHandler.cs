using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Interfaces.Identity;
using CitiesOnMap.Domain.Constants;
using MediatR;

namespace CitiesOnMap.Application.Commands.Authorization.RevokeToken;

public class RevokeTokenCommandHandler(
    IUserManager userManager)
    : IRequestHandler<RevokeTokenCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        await userManager.SetAuthenticationTokenAsync(
            request.User, Defaults.DefaultProvider, Defaults.RefreshTokenType, null);
        return new OperationResult(true);
    }
}