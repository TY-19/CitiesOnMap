using CitiesOnMap.Application.Commands.Authorization.GenerateTokens;
using CitiesOnMap.Application.Commands.Authorization.RevokeToken;
using CitiesOnMap.Application.Commands.Users.CreateUser;
using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Interfaces.Services;
using CitiesOnMap.Application.Models.Authorization;
using CitiesOnMap.Application.Models.Authorization.External;
using CitiesOnMap.Application.Queries.Authorization.CheckUserPassword;
using CitiesOnMap.Application.Queries.Authorization.GetExternalToken;
using CitiesOnMap.Application.Queries.Authorization.GetExternalUserInfo;
using CitiesOnMap.Application.Queries.Authorization.GetOAuthProviderConfiguration;
using CitiesOnMap.Application.Queries.Authorization.ValidateRefreshToken;
using CitiesOnMap.Application.Queries.Users.GetUser;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Services;

public class AccountService(
    IMediator mediator)
    : IAccountService
{
    public async Task<OperationResult<TokensModel>> RegisterUserAsync(RegistrationRequestModel model,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(model.UserName, model.Email, model.Password);
        OperationResult<User> userResult = await mediator.Send(command, cancellationToken);
        if (userResult is { Succeeded: true, Payload: not null })
        {
            return await LoginAsync(new LoginRequestModel
            {
                UserName = userResult.Payload.UserName,
                Password = model.Password
            }, cancellationToken);
        }

        return new OperationResult<TokensModel>(false, userResult.Details.Type);
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

        OperationResult checkPasswordResult =
            await mediator.Send(new CheckUserPasswordRequest(user, model.Password), cancellationToken);
        if (!checkPasswordResult.Succeeded)
        {
            return new OperationResult<TokensModel>(false, ResultType.InvalidPassword);
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

        ExternalUserInfo? userInfo = await mediator.Send(new GetExternalUserInfoRequest(provider,
            config.UserInfoEndpoint, codeExchangeResult.Payload.AccessToken), cancellationToken);
        if (userInfo == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.FetchingExternalUserInfoFailed);
        }

        User? user = await mediator.Send(
            new GetUserRequest(null, null, null, provider, userInfo.ProviderKey),
            cancellationToken);
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

    public async Task<OperationResult<TokensModel>> RefreshTokensAsync(RefreshTokenModel model,
        CancellationToken cancellationToken)
    {
        User? user = await mediator.Send(new GetUserRequest(null, model.UserName, null, null, null),
            cancellationToken);
        if (user == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.UserNotExist);
        }

        OperationResult validationTokenResult = await mediator.Send(
            new ValidateRefreshTokenRequest(user, model.RefreshToken), cancellationToken);
        if (!validationTokenResult.Succeeded)
        {
            return new OperationResult<TokensModel>(false, validationTokenResult.Details.Type);
        }

        TokensModel tokens = await mediator.Send(new GenerateTokensCommand(user), cancellationToken);

        return new OperationResult<TokensModel>(true, tokens);
    }

    public async Task<OperationResult> RevokeTokenAsync(RefreshTokenModel model,
        CancellationToken cancellationToken)
    {
        User? user = await mediator.Send(new GetUserRequest(null, model.UserName, null, null, null),
            cancellationToken);
        if (user == null)
        {
            return new OperationResult(false, ResultType.UserNotExist);
        }

        OperationResult validationTokenResult = await mediator.Send(
            new ValidateRefreshTokenRequest(user, model.RefreshToken), cancellationToken);

        return validationTokenResult.Succeeded
            ? await mediator.Send(new RevokeTokenCommand(user, model.RefreshToken, null), cancellationToken)
            : new OperationResult(false);
    }
}