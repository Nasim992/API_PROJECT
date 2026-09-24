using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;
using System.Xml.Linq;

namespace Dextor.API.Models.Core.Accounts
{

    [Table("t_AccJournalTransaction")]
    public class AccJournalTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int TransactionId { get; set; }
        public string TransactionNo { get; set; }
        public int CoAId { get; set; }
        public DateTime DocumentDate { get; set; }
        public int DocumentType { get; set; }
        public int FiscalYear { get; set; }
        public int Period { get; set; }
        public DateTime? PostingDate { get; set; }
        public int? PostingUserID { get; set; }
        public int CurrencyID { get; set; }
        public string Reference { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserID { get; set; }
        public int Status { get; set; }

        public List<AccJournalTransactionDetail> AccJournalTransactionDetails { get; set; }

    }
}