using CitiesOnMap.Application.Common;
using CitiesOnMap.Application.Models.Login.External;
using MediatR;

namespace CitiesOnMap.Application.Queries.Users.GetExternalToken;

public record GetExternalTokenRequest(ExternalProviderConfiguration Configuration, CodeExchangeModel Model)
    : IRequest<OperationResult<ExternalTokenResponse>>;