using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dextor.API.Models.Core
{
    [Table("t_User")]
    public class User
    {
        //[Key]
        //[ScaffoldColumn(false)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]

        [Key]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int UserId { get; set; }
        public string UserFullName { get; set; }

        [Required(ErrorMessage = "Please Enter User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
        public string Salt { get; set; }
        //public string UserSbUs { get; set; }
        public int? EmployeeId { get; set; }
        public int UserIsActive { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int? LastUpdateUserID { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? UserGroupID { get; set; }
        public int? DefaultAppDashboard { get; set; }
        public List<UserPermission> userPermissions { get; set; }
        public int? IsReportLogEnabled { get; set; }

    }
}
