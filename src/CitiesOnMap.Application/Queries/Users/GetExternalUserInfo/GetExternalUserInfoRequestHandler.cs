using CitiesOnMap.Application.Models.Authorization.External;
using MediatR;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CitiesOnMap.Application.Queries.Users.GetExternalUserInfo;

public class GetExternalUserInfoRequestHandler(
    IHttpClientFactory factory)
    : IRequestHandler<GetExternalUserInfoRequest, ExternalUserInfo?>
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    public async Task<ExternalUserInfo?> Handle(GetExternalUserInfoRequest request, CancellationToken cancellationToken)
    {
        var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, request.Endpoint);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);

        try
        {
            HttpResponseMessage infoResponse = await _httpClient.SendAsync(requestMessage, cancellationToken);
            var userInfoModel =
                await infoResponse.Content.ReadFromJsonAsync<ExternalUserInfoSerializationModel>(cancellationToken);
            return ExtractUserInfo(request.Provider, userInfoModel);
        }
        catch
        {
            return null;
        }
    }

    private static ExternalUserInfo? ExtractUserInfo(string provider, ExternalUserInfoSerializationModel? model)
    {
        if (model == null)
        {
            return null;
        }

        return provider switch
        {
            "Google" => ExtractUserInfoFromGoogleResponse(provider, model),
            _ => null
        };
    }

    private static ExternalUserInfo? ExtractUserInfoFromGoogleResponse(string provider,
        ExternalUserInfoSerializationModel model)
    {
        if (model.Email == null)
        {
            return null;
        }

        string email = model.Email;
        string userName = email[..email.IndexOf('@')];
        return new ExternalUserInfo(userName, email, provider, email);
    }
}