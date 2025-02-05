using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Features.Authorization.Models;
using CitiesOnMap.Application.Features.Authorization.Models.External;

namespace CitiesOnMap.Application.Interfaces.Services;

public interface IAccountService
{
    Task<OperationResult<TokensModel>> RegisterUserAsync(RegistrationRequestModel model,
        CancellationToken cancellationToken);

    Task<OperationResult<TokensModel>> LoginExternalUserAsync(string provider, CodeExchangeModel model,
        CancellationToken cancellationToken);

    Task<OperationResult<TokensModel>> LoginAsync(LoginRequestModel model, CancellationToken cancellationToken);

    Task<OperationResult<TokensModel>> RefreshTokensAsync(RefreshTokenModel model, CancellationToken cancellationToken);

    Task<OperationResult> RevokeTokenAsync(RefreshTokenModel model, CancellationToken cancellationToken);
}
