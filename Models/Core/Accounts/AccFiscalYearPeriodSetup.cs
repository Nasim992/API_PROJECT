using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dextor.API.Models.Core.Accounts
{
    [Table("t_AccFiscalYearPeriodSetup")]
    public class AccFiscalYearPeriodSetup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FiscalYearID { get; set; }
        public int CoAID { get; set; }
        public string Description { get; set; }
        public int FiscalYear { get; set; }
        public int IsActive { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdateUserID { get; set; }
        public DateTime? UpdateDate { get; set; }

        public List<AccFiscalYearPeriodSetupDetail> AccFiscalYearPeriodSetupDetails { get; set; }
    }
}