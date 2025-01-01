using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Models.Authorization;
using CitiesOnMap.Application.Models.Authorization.External;

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