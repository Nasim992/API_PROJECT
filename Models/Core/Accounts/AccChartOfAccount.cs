using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dextor.API.Models.Core.Accounts
{
    [Table("t_AccChartOfAccount")]
    public class AccChartOfAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountID { get; set; }

        public int CoAId { get; set; }
        public int Tier { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public string DefaultSide { get; set; }

        public int IsPostingAccount { get; set; }

        public int IsActive { get; set; }

        public int? ViewPosition { get; set; }

        public int IsAllowManualEntry { get; set; }

        public int? ParentAccountID { get; set; }

        public int DefaultCurrencyID { get; set; }

        public double? Balance { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUserID { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? UpdateUserID { get; set; }

        public int? CoAType { get; set; }
        public string LegacyAccountCode { get; set; }
    }
}