using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Interfaces.Helpers;
using CitiesOnMap.Application.Interfaces.Identity;
using CitiesOnMap.Domain.Constants;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Requests.ValidateRefreshToken;

public class ValidateRefreshTokenRequestHandler(
    IUserManager userManager,
    IHashingHelper hasher)
    : IRequestHandler<ValidateRefreshTokenRequest, OperationResult>
{
    public async Task<OperationResult> Handle(ValidateRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        string[] parts = request.RefreshToken.Split("::");
        if (!long.TryParse(parts[^1], out long utcTicks)
            || DateTimeOffset.UtcNow.UtcTicks > utcTicks)
        {
            return new OperationResult(false, ResultType.TokenExpired);
        }

        string? tokenHash = await userManager.GetAuthenticationTokenAsync(
            request.User, Defaults.DefaultProvider, Defaults.RefreshTokenType);
        if (tokenHash == null)
        {
            return new OperationResult(false, ResultType.InvalidToken);
        }

        return tokenHash == hasher.GetSha256Hash(request.RefreshToken)
            ? new OperationResult(true)
            : new OperationResult(false, ResultType.InvalidToken);
    }
}
