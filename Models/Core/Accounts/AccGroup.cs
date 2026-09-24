using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dextor.API.Models.Core.Accounts
{
    [Table("t_AccGroup")]
    public class AccGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccGroupID { get; set; }

        public int CoAId { get; set; }

        public string GroupCode { get; set; }

        public string GroupDescription { get; set; }

        public char NormalBalance { get; set; }

        public int NumberRangeFrom { get; set; }

        public int NumberRangeTo { get; set; }

        public int IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUserID { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? UpdateUserID { get; set; }

    }
}