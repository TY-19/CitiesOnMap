namespace CitiesOnMap.Application.Interfaces.Helpers;

public interface IHashingHelper
{
    string GetSha256Hash(string input);
}
