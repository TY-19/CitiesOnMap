namespace CitiesOnMap.Application.Common.Results;

public enum ResultType
{
    Undefined = 0,
    Succeeded = 1,
    UserNotExist = 11,
    UserCreationFailed = 12,
    InvalidPassword = 13,
    InvalidToken = 16,
    TokenExpired = 17,
    ExternalCodeExchangeFailed = 21,
    FetchingExternalUserInfoFailed = 22,
    GameNotExist = 101,
    NoCityInQuestion
}