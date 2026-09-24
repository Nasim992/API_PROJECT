using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dextor.API.Models.Core.Accounts
{
    [Table("t_AccCoASetup")]
    public class CoASetup
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CoAId { get; set; }

        public string CoACode { get; set; }

        public string CoADescription { get; set; }

        public int CompanyID { get; set; }

        public int NoofTier { get; set; }

        public int IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreateUserID { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? UpdateUserID { get; set; }

        public List<CoATierConfig> CoATierConfigs { get; set; }
    }



}