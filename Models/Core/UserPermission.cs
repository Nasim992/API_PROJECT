using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; 

namespace Dextor.API.Models.Core 
{
    [Table("t_UserPermission", Schema = "dbo")]
    [PrimaryKey(nameof(UserID), nameof(PermissionKey))] // FIX: Composite key defined here
    public class UserPermission
    {
        public int UserID { get; set; }

        [StringLength(50)]
        public string PermissionKey { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }
    }
}