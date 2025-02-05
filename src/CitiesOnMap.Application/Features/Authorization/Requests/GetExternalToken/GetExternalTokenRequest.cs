using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Features.Authorization.Models.External;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Requests.GetExternalToken;

public record GetExternalTokenRequest(ExternalProviderConfiguration Configuration, CodeExchangeModel Model)
    : IRequest<OperationResult<ExternalTokenResponse>>;
