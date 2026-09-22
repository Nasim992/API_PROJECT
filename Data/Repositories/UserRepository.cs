using Dextor.API.Data.IRepositories;
using Dextor.API.Models.BindingModel;
using Dextor.API.Models.Core;
using Dextor.API.Models.ViewModel;
using Microsoft.EntityFrameworkCore; // Required for EF Core extensions like FromSqlRaw and ExecuteSqlRaw

namespace Dextor.API.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly DatabaseContext _databaseContext;

        public UserRepository(DatabaseContext context) : base(context)
        {
            _databaseContext = context;
        }

        public List<User> GetUserDetails(int nUserId)
        {
            const string sGetUserDetails = @"SELECT * FROM t_User a where a.UserIsActive=1";

            // In EF Core, if returning an Entity mapped in a DbSet, use .FromSqlRaw()
            return _databaseContext.Users.FromSqlRaw(sGetUserDetails).ToList();
        }

        public SessionClass? GetSessionData(string sUserName)
        {
            var sGetSessionData = @"select UserID, UserFullName, UserName, a.EmployeeId,  
                                    EmployeeCode, EmployeeName, ShowroomCode,  
                                    ShowroomName, ShowroomType, WarehouseID, CustomerID,b.LocationID,isnull(a.IsReportLogEnabled,0) IsReportLogEnabled 
                                    from t_User a, t_Employee b, t_Showroom c  
                                    Where  a.EmployeeId = b.EmployeeId  
                                    and b.LocationID = c.LocationID and EMPStatus  
                                    IN ({0},{1}) and IsPOSActive ={2} and UserName = '{3}' ";

            sGetSessionData = string.Format(sGetSessionData,
                            (int)Dictionary.HrEmployeeStatus.Confirmed,
                            (int)Dictionary.HrEmployeeStatus.Contractual,
                            (int)Dictionary.YesNo.Yes,
                            sUserName
                            );

            // For custom ViewModels/Classes not in a DbSet, EF Core 8 uses SqlQueryRaw
            return _databaseContext.Database.SqlQueryRaw<SessionClass>(sGetSessionData).FirstOrDefault();
        }

        //public UserViewModel? GetTechnicianByUserName(string userName)
        //{
        //    string query = @"SELECT UserId,b.EmployeeId,EmployeeName,UserName,
        //                    UserFullName,Name TechnicianName,TechnicianId,b.EmployeeCode
        //                    from t_User a,t_Employee b,t_CSDTechnician c
        //                    WHere a.EmployeeId = b.EmployeeId
        //                    AND c.EmployeeCode = b.EmployeeCode
        //                    AND b.EmpStatus in(1,2) and UserName = '{0}' ";
        //    query = string.Format(query, userName.Trim());

        //    return _databaseContext.Database.SqlQueryRaw<UserViewModel>(query).FirstOrDefault();
        //}

        //public int GetUserIdByTechnicianId(int technicianId)
        //{
        //    // FIX: Added 'AS Value' to map the primitive int
        //    string query = @"SELECT UserId AS Value from t_CSDTechnician a,t_Employee b,t_User c
        //            WHERE a.EmployeeCode = b.EmployeeCode 
        //            AND b.EmployeeId = c.EmployeeId and TechnicianID = {0}";

        //    query = string.Format(query, technicianId);

        //    return _databaseContext.Database.SqlQueryRaw<int>(query).FirstOrDefault();
        //}

        //public UserViewModel? GetTsoTsmUser(string userName)
        //{
        //    string query = @"SELECT TOP 1 a.EmployeeId,d.EmployeeCode,d.EmployeeName,
        //                    a.UserId,a.UserName From t_User a,t_MarketGroup b,
        //                    t_Customer c,t_Employee d
        //                    WHERE a.EmployeeId = b.EmployeeId 
        //                    AND c.MarketGroupId = b.MarketGroupId 
        //                    AND d.EmployeeId = a.EmployeeId
        //                    AND b.MarketGroupType = 2
        //                    AND d.EMPStatus in(1,2)
        //                    AND c.IsActive = 1
        //                    AND UserName = '{0}' ";
        //    query = string.Format(query, userName);

        //    return _databaseContext.Database.SqlQueryRaw<UserViewModel>(query).FirstOrDefault();
        //}

        public bool InsertUser(User ouser)
        {
            // REMOVED: using (var db = new DatabaseContext()) -> We use _databaseContext
            using (var transaction = _databaseContext.Database.BeginTransaction())
            {
                try
                {
                    User user = new User
                    {
                        UserName = ouser.UserName,
                        UserFullName = ouser.UserFullName,
                        Password = ouser.Password,
                        Salt = ouser.Salt,
                        UserIsActive = ouser.UserIsActive,
                        CreateUserID = ouser.CreateUserID,
                        CreateDate = ouser.CreateDate,
                        EmployeeId = ouser.EmployeeId,
                        UserGroupID = ouser.UserGroupID,
                        DefaultAppDashboard = ouser.DefaultAppDashboard,
                        IsReportLogEnabled = ouser.IsReportLogEnabled,
                    };

                    _databaseContext.Users.Add(user);
                    _databaseContext.SaveChanges();

                    foreach (var oDetail in ouser.userPermissions)
                    {
                        UserPermission permission = new UserPermission
                        {
                            UserID = user.UserId,
                            PermissionKey = oDetail.PermissionKey,
                        };
                        _databaseContext.UserPermissions.Add(permission);
                    }

                    // Optimization: Moved SaveChanges outside the foreach loop for better performance
                    _databaseContext.SaveChanges();

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool EditUser(User oUser, string isPasswordReset)
        {
            using (var transaction = _databaseContext.Database.BeginTransaction())
            {
                try
                {
                    var user = _databaseContext.Users.FirstOrDefault(c => c.UserId == oUser.UserId);
                    if (user == null)
                    {
                        return false;
                    }

                    user.UserId = oUser.UserId;
                    user.UserFullName = oUser.UserFullName;
                    user.UserName = oUser.UserName;
                    user.UserIsActive = oUser.UserIsActive;
                    user.LastUpdateDate = DateTime.Now;
                    user.LastUpdateUserID = oUser.LastUpdateUserID;
                    user.EmployeeId = oUser.EmployeeId;
                    user.UserGroupID = oUser.UserGroupID;
                    user.DefaultAppDashboard = oUser.DefaultAppDashboard;

                    if (isPasswordReset == "Yes")
                    {
                        user.Password = oUser.Password;
                        user.Salt = oUser.Salt;
                    }

                    _databaseContext.SaveChanges();

                    // EF Core replaces ExecuteSqlCommand with ExecuteSqlRaw
                    string query = @"delete t_UserPermission where UserId='{0}'";
                    query = string.Format(query, oUser.UserId);
                    _databaseContext.Database.ExecuteSqlRaw(query);

                    foreach (var oDetail in oUser.userPermissions)
                    {
                        UserPermission permission = new UserPermission
                        {
                            UserID = user.UserId,
                            PermissionKey = oDetail.PermissionKey,
                        };
                        _databaseContext.UserPermissions.Add(permission);
                    }

                    _databaseContext.SaveChanges();
                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool InsertFirebaseTokenByUser(FirebaseTokenByUser omodel)
        {
            using (var transaction = _databaseContext.Database.BeginTransaction())
            {
                try
                {
                    // EF Core replaces ExecuteSqlCommand with ExecuteSqlRaw
                    string query = @"delete t_FirebaseTokenByUser where UserID='{0}' and AppType='{1}' and ProjectID='{2}'";
                    query = string.Format(query, omodel.UserID, omodel.AppType, omodel.ProjectID);
                    _databaseContext.Database.ExecuteSqlRaw(query);

                    FirebaseTokenByUser model = new FirebaseTokenByUser
                    {
                        UserID = omodel.UserID,
                        FirebaseToken = omodel.FirebaseToken,
                        AppType = omodel.AppType,
                        LastUpdateDate = omodel.LastUpdateDate,
                        ProjectID = string.IsNullOrEmpty(omodel.ProjectID) ? "apex-nucleus" : omodel.ProjectID
                    };

                    _databaseContext.FirebaseTokenByUsers.Add(model);
                    _databaseContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}