using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dextor.API.Models.Core.Accounts
{
    [Table("t_AccCoATierConfig")]
    public class CoATierConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CoAId { get; set; }
        public int Tier { get; set; }
        public string TierDescription { get; set; }
        public int CodeLength { get; set; }
        public int IsAutoNoRange { get; set; }
        public string Prefix { get; set; }
        public int CodeConvention { get; set; }
        public string Suffix { get; set; }
        public int? NextCode { get; set; }

        [ForeignKey("CoAId")]
        public CoASetup CoASetup { get; set; }
    }
}