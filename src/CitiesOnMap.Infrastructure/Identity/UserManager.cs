using CitiesOnMap.Application.Interfaces.Identity;
using CitiesOnMap.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CitiesOnMap.Infrastructure.Identity;

public class UserManager(
    IUserStore<User> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<User> passwordHasher,
    IEnumerable<IUserValidator<User>> userValidators,
    IEnumerable<IPasswordValidator<User>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<User>> logger)
    : UserManager<User>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
        errors, services, logger), IUserManager
{
}
