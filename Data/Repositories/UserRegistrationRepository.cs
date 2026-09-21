using Dextor.API.Models.Core;
using Dextor.API.Models.BindingModel;
using Dextor.API.Data.IRepositories;
using System;
using System.Linq;

namespace Dextor.API.Data.Repositories
{
    public class UserRegistrationRepository : GenericRepository<UserRegistration>, IUserRegistrationRepository
    {
        // FIX 1: Pass the database connection to the base GenericRepository!
        // This activates Find(), Update(), and Save() so the controller can use them.
        public UserRegistrationRepository(DatabaseContext context) : base(context)
        {
            // You don't need a private _databaseContext variable anymore.
            // GenericRepository already provides 'DbContext' for you to use.
        }

        public bool UpdateUserRegistration(UserRegistration oUserRegistration)
        {
            // FIX 2: Use the inherited 'DbContext' here
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var userregistration = DbContext.UserRegistration.FirstOrDefault(c => c.Id == oUserRegistration.Id);
                    if (userregistration == null)
                    {
                        return false; // Entity not found
                    }

                    userregistration.Status = oUserRegistration.Status;
                    userregistration.AuthenticateMode = oUserRegistration.AuthenticateMode;
                    userregistration.ActivatedBy = oUserRegistration.ActivatedBy;
                    userregistration.ActivatedDate = oUserRegistration.ActivatedDate;
                    userregistration.VersionNo = oUserRegistration.VersionNo;
                    userregistration.AppId = oUserRegistration.AppId;
                    userregistration.EmployeeId = oUserRegistration.EmployeeId;
                    userregistration.UserID = oUserRegistration.UserID;

                    DbContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error updating product: {ex.Message}");
                    return false;
                }
            }
        }
    }
}