using CitiesOnMap.Application.Interfaces.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace CitiesOnMap.Application.Helpers;

public class HashingHelper : IHashingHelper
{
    public string GetSha256Hash(string input)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
    }
}
