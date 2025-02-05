using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Interfaces.Identity;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Requests.CheckUserPassword;

public class CheckUserPasswordRequestHandler(
    IUserManager userManager
) : IRequestHandler<CheckUserPasswordRequest, OperationResult>
{
    public async Task<OperationResult> Handle(CheckUserPasswordRequest request, CancellationToken cancellationToken)
    {
        return await userManager.CheckPasswordAsync(request.User, request.Password)
            ? new OperationResult(true)
            : new OperationResult(false);
    }
}
