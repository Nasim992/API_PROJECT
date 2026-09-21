using Dextor.API.Models.Core;
using System.Collections.Generic;

namespace Dextor.API.Models.ViewModel
{
    public class FfSession
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleID { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public int ApplicationVersionNo { get; set; }
        public List<UserPermission> MenupermissionList { get; set; }
    }

    public class vUser
    {
        public int UserID { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string CardNo { get; set; }
        public string EmployeeName { get; set; }
        public Dictionary.IsActive IsActive { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastUpdateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string PermissionKey { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public byte[] EmployeePhoto { get; set; }
        public int? DefaultAppDashboard { get; set; }
        public int? UserGroupID { get; set; }
        public int LocationSyncInterval { get; set; }
        public int IsAllowBackgroundLocation { get; set; }
        public string UserGroup { get; set; }
        public int IsReportLogEnabled { get; set; }
    }
}
