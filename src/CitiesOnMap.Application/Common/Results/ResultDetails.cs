namespace CitiesOnMap.Application.Common.Results;

public class ResultDetails(ResultType type)
{
    private static readonly Dictionary<ResultType, string> Messages = new()
    {
        [ResultType.Undefined] = "Unknown result.",
        [ResultType.Succeeded] = "Succeeded.",
        [ResultType.GameNotExist] = "The game does not exist.",
        [ResultType.InvalidPlayerForGame] = "The game does not belong to the player.",
        [ResultType.NoCityInQuestion] = "There are no city to point out to.",
        [ResultType.UserNotExist] = "The user does not exist.",
        [ResultType.UserCreationFailed] = "The user has not been created.",
        [ResultType.InvalidPassword] = "The password is invalid.",
        [ResultType.InvalidToken] = "The token is invalid.",
        [ResultType.TokenExpired] = "The token has expired.",
        [ResultType.ExternalCodeExchangeFailed] = "Code exchanging with the external provider has failed.",
        [ResultType.FetchingExternalUserInfoFailed] = "Fetching user info from the external provider has failed."
    };

    public ResultDetails() : this(ResultType.Undefined)
    {
    }

    public ResultType Type { get; } = type;
    public string? Message => Messages[Type];
}
