using Dextor.API.Models.Core;
using Dextor.API.Models.ViewModel;

namespace Dextor.API.Data.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        List<User> GetUserDetails(int nUserId);
        SessionClass GetSessionData(string sUserName);
        UserViewModel GetTechnicianByUserName(string userName);
        int GetUserIdByTechnicianId(int technicianId);
        UserViewModel GetTsoTsmUser(string userName);
        bool InsertUser(User user);
        bool EditUser(User user, string isPasswordReset);

        bool InsertFirebaseTokenByUser(FirebaseTokenByUser model);
    }
}
