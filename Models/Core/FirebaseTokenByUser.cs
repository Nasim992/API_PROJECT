using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dextor.API.Models.Core
{
    [Table("t_FirebaseTokenByUser")]
    public class FirebaseTokenByUser
    {
        [Key]
        [ScaffoldColumn(false)]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }
        public int UserID { get; set; }
        public string FirebaseToken { get; set; }
        public string AppType { get; set; }
        public string ProjectID { get; set; }
        public DateTime LastUpdateDate { get; set; }

    }
}
