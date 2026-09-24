using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dextor.API.Models.Core.Accounts
{
    //[Table("t_AccountBalances")]
    public class AccountBalance
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BalanceID { get; set; }
        public int AccountID { get; set; }
        public int FiscalYear { get; set; }
        public int Period { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }

        // Computed property for ClosingBalance
        public decimal ClosingBalance => OpeningBalance + DebitAmount - CreditAmount;
    }
}