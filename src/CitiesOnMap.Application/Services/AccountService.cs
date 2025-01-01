using CitiesOnMap.Application.Commands.Users.CreateUser;
using CitiesOnMap.Application.Commands.Users.GenerateTokens;
using CitiesOnMap.Application.Common;
using CitiesOnMap.Application.Interfaces.Helpers;
using CitiesOnMap.Application.Interfaces.Identity;
using CitiesOnMap.Application.Interfaces.Services;
using CitiesOnMap.Application.Models.Login;
using CitiesOnMap.Application.Models.Login.External;
using CitiesOnMap.Application.Queries.Users.GetExternalToken;
using CitiesOnMap.Application.Queries.Users.GetExternalUserInfo;
using CitiesOnMap.Application.Queries.Users.GetOAuthProviderConfiguration;
using CitiesOnMap.Application.Queries.Users.GetUser;
using CitiesOnMap.Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace CitiesOnMap.Application.Services;

public class AccountService(
    IMediator mediator,
    IUserManager userManager,
    IHashingHelper hasher)
    : IAccountService
{
    public async Task<OperationResult<User>> RegisterUserAsync(RegistrationRequestModel model,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(model.UserName, model.Email, model.Password);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<OperationResult<TokensModel>> LoginAsync(LoginRequestModel model,
        CancellationToken cancellationToken)
    {
        var request = new GetUserRequest(null, model.UserName, model.Email, null, null);
        User? user = await mediator.Send(request, cancellationToken);
        if (user == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.UserNotExist);
        }

        TokensModel tokens = await mediator.Send(new GenerateTokensCommand(user), cancellationToken);

        return new OperationResult<TokensModel>(true, tokens);
    }

    public async Task<OperationResult<TokensModel>> LoginExternalUserAsync(string provider, CodeExchangeModel model,
        CancellationToken cancellationToken)
    {
        ExternalProviderConfiguration config =
            await mediator.Send(new GetOAuthProviderConfigurationRequest(provider), cancellationToken);

        OperationResult<ExternalTokenResponse> codeExchangeResult = await mediator.Send(
            new GetExternalTokenRequest(config, model), cancellationToken);
        if (!codeExchangeResult.Succeeded || codeExchangeResult.Payload?.AccessToken == null)
        {
            return new OperationResult<TokensModel>(false, codeExchangeResult.Details.Type);
        }

        ExternalTokenResponse externalTokens = codeExchangeResult.Payload;

        ExternalUserInfo? userInfo = await mediator.Send(new GetExternalUserInfoRequest(provider,
            config.UserInfoEndpoint, externalTokens.AccessToken), cancellationToken);
        if (userInfo == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.FetchingExternalUserInfoFailed);
        }

        User? user = await userManager.FindByLoginAsync(provider, userInfo.ProviderKey);
        if (user == null)
        {
            OperationResult<User> result = await mediator.Send(
                new CreateUserCommand(userInfo.UserName, userInfo.Email, null, provider, userInfo.ProviderKey),
                cancellationToken);
            if (!result.Succeeded || result.Payload == null)
            {
                return new OperationResult<TokensModel>(false, ResultType.UserCreationFailed);
            }

            user = result.Payload;
        }

        TokensModel tokens = await mediator.Send(new GenerateTokensCommand(user), cancellationToken);

        return new OperationResult<TokensModel>(true, tokens);
    }

    public async Task<OperationResult<TokensModel>> RefreshTokensAsync(string email, RefreshTokensModel model,
        CancellationToken cancellationToken)
    {
        if (model.AccessToken == null || model.RefreshToken == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.InvalidToken);
        }

        User? user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.UserNotExist);
        }

        string? token = await userManager.GetAuthenticationTokenAsync(user, "Local", "Refresh");
        if (token == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.InvalidToken);
        }

        string[] parts = token.Split("::");
        if (!long.TryParse(parts[0], out long utcTicks)
            || DateTimeOffset.UtcNow.UtcTicks > utcTicks)
        {
            return new OperationResult<TokensModel>(false, ResultType.TokenExpired);
        }

        string tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        if (tokenHash != model.RefreshToken)
        {
            return new OperationResult<TokensModel>(false, ResultType.InvalidToken);
        }

        TokensModel tokens = await mediator.Send(new GenerateTokensCommand(user), cancellationToken);

        return new OperationResult<TokensModel>(true, tokens);
    }

    public async Task<OperationResult> RevokeTokenAsync(string email, RefreshTokensModel model,
        CancellationToken cancellationToken)
    {
        if (model.AccessToken == null || model.RefreshToken == null)
        {
            return new OperationResult(false, ResultType.InvalidToken);
        }

        User? user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new OperationResult(false, ResultType.UserNotExist);
        }

        string? token = await userManager.GetAuthenticationTokenAsync(user, "Local", "Refresh");
        if (token == null || token != hasher.GetSha256Hash(model.RefreshToken))
        {
            return new OperationResult(false, ResultType.InvalidToken);
        }

        await userManager.SetAuthenticationTokenAsync(user, "Local", "Refresh", null);
        return new OperationResult(true);
    }
}