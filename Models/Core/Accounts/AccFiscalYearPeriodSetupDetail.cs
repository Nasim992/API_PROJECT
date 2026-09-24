using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dextor.API.Models.Core.Accounts
{
    [Table("t_AccFiscalYearPeriodSetupDetail")]
    public class AccFiscalYearPeriodSetupDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int PeriodID { get; set; }
        public int FiscalYearID { get; set; }
        public int CalendarYear { get; set; }
        public int CalendarMonth { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Days { get; set; }
        public int Period { get; set; }
        public int Status { get; set; }
        public int? PeriodStartUserID { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public int? PeriodEndUserID { get; set; }
        public DateTime? PeriodEndDate { get; set; }
    }
}