using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dextor.API.Models.Core
{
    [Table("t_UserRegistration")]
    public class UserRegistration
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string MobileNo { get; set; }

        [Required]
        [StringLength(250)]
        public string UserFullName { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(150)]
        public string UniqueSerialNo { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(50)]
        public string AuthenticateMode { get; set; }

        [StringLength(50)]
        public string ActivatedBy { get; set; }

        public DateTime? ActivatedDate { get; set; }

        [StringLength(50)]
        public string VersionNo { get; set; }

        public int? AppId { get; set; }

        public int? EmployeeId { get; set; }
        public int? UserID { get; set; }
    }
}
