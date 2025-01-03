using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Models.Authorization.External;
using MediatR;

namespace CitiesOnMap.Application.Queries.Authorization.GetExternalToken;

public record GetExternalTokenRequest(ExternalProviderConfiguration Configuration, CodeExchangeModel Model)
    : IRequest<OperationResult<ExternalTokenResponse>>;