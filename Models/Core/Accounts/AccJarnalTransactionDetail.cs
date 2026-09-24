using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dextor.API.Models.Core.Accounts
{

    [Table("t_AccJournalTransactionDetail")]
    public class AccJournalTransactionDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionDetailId { get; set; }


        public int TransactionId { get; set; }


        public int AccountId { get; set; }

        public string DrCrIndicator { get; set; }


        public decimal Amount { get; set; }

        public string ReferenceNo { get; set; }


        public int? CostProfitCenterID { get; set; }

        public int ValueKey { get; set; }


    }
}