using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dextor.API.Models.Core.Accounts
{
    [Table("t_AccCostProfitCenter")]
    public class AccCostProfitCenter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProfitCenterID { get; set; }

        public int Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreateUserID { get; set; }
    }
}