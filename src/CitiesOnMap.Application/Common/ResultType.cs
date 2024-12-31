namespace CitiesOnMap.Application.Common;

public enum ResultType
{
    Undefined = 0,
    Succeeded = 1,
    UserNotExist = 11,
    InvalidToken = 16,
    TokenExpired = 17,
    GameNotExist = 101,
    NoCityInQuestion
}