using CitiesOnMap.Application.Common;
using CitiesOnMap.Application.Models;
using CitiesOnMap.Domain.Entities;

namespace CitiesOnMap.Application.Interfaces;

public interface IAccountService
{
    Task<OperationResult<User>> RegisterUserAsync(RegistrationRequestModel model, CancellationToken cancellationToken);

    Task<OperationResult<TokensModel>> LoginExternalUserAsync(string provider, CodeExchangeModel model,
        CancellationToken cancellationToken);

    Task<OperationResult<TokensModel>> LoginAsync(LoginRequestModel model, CancellationToken cancellationToken);

    Task<OperationResult<TokensModel>> RefreshTokensAsync(string email, RefreshTokensModel model,
        CancellationToken cancellationToken);

    Task<OperationResult> RevokeTokenAsync(string email, RefreshTokensModel model, CancellationToken cancellationToken);
}