using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dextor.API.Models.Core
{
    [Table("t_ReportLog")]
    public class ReportLog
    {
        [Key]
        public int LogID { get; set; }
        public string ReportNo { get; set; }
        public string PermissionKey { get; set; }
        public string ReportName { get; set; }
        public long? EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string AccessTerminal { get; set; }
        public string DeviceIdentity { get; set; }
    }
}
