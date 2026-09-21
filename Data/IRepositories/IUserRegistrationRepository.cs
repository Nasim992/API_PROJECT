using Dextor.API.Models.Core;

namespace Dextor.API.Data.IRepositories
{
    // FIX: Only one interface declaration, and it inherits IRepository
    public interface IUserRegistrationRepository : IRepository<UserRegistration>
    {
        // Your custom methods for this specific repository
        bool UpdateUserRegistration(UserRegistration oUserRegistration);
    }
}