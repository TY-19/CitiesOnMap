namespace CitiesOnMap.Application.Common;

public class ResultDetails(ResultType type)
{
    private static readonly Dictionary<ResultType, string> Messages = new()
    {
        [ResultType.Undefined] = "Unknown result.",
        [ResultType.Succeeded] = "Succeeded.",
        [ResultType.GameNotExist] = "The game does not exist.",
        [ResultType.NoCityInQuestion] = "There are no city to point out to.",
        [ResultType.UserNotExist] = "The user does not exist.",
        [ResultType.InvalidToken] = "A token is invalid.",
        [ResultType.TokenExpired] = "The token has expired."
    };

    public ResultDetails() : this(ResultType.Undefined)
    {
    }

    public ResultType Type { get; } = type;
    public string? Message => Messages[Type];
}