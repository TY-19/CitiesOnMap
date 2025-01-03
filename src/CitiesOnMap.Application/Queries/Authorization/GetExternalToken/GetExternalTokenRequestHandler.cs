using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Models.Authorization.External;
using MediatR;
using System.Net.Http.Json;

namespace CitiesOnMap.Application.Queries.Authorization.GetExternalToken;

public class GetExternalTokenRequestHandler(
    IHttpClientFactory factory)
    : IRequestHandler<GetExternalTokenRequest, OperationResult<ExternalTokenResponse>>
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    public async Task<OperationResult<ExternalTokenResponse>> Handle(GetExternalTokenRequest request,
        CancellationToken cancellationToken)
    {
        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, request.Configuration.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", request.Configuration.ClientId },
                { "client_secret", request.Configuration.ClientSecret },
                { "code", request.Model.Code },
                { "redirect_uri", request.Configuration.FrontendCallbackUrl },
                { "grant_type", "authorization_code" },
                { "code_verifier", request.Model.CodeVerifier }
            })
        };
        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(tokenRequest, cancellationToken);
            ExternalTokenResponse externalTokens =
                await response.Content.ReadFromJsonAsync<ExternalTokenResponse>(cancellationToken) ??
                throw new Exception("Serialization failed.");
            return new OperationResult<ExternalTokenResponse>(true, externalTokens);
        }
        catch
        {
            return new OperationResult<ExternalTokenResponse>(false, ResultType.ExternalCodeExchangeFailed);
        }
    }
}