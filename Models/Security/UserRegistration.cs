namespace Dextor.API.Models.Security
{
    public class UserRegistrationView
    {
        public const string ViewName = "v_UserRegistration";
    }
    public class UserRegistrationTable
    {
        public const string Table = "t_UserRegistration";
    }

    public class UserRegistration
    {
        public int ID { get; set; }
        public string MobileNo { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string UniqueSerialNo { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string AuthenticateMode { get; set; }
        public string ActivatedBy { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public string VersionNo { get; set; }
        public int? AppID { get; set; }
        public int? EmployeeID { get; set; }
        public int? UserID { get; set; }
    }

    public class v_UserRegistration
    {
        public int ID { get; set; }
        public string MobileNo { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string UniqueSerialNo { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }  // Changed from int to string
        public string AuthenticateMode { get; set; }  // Changed from int to string
        public string ActivatedBy { get; set; }
        public string ActivatedByName { get; set; }  // Added from view
        public DateTime? ActivatedDate { get; set; }  // Added missing property
        public string VersionNo { get; set; }  // Changed from int to string
        public int? AppID { get; set; }
        public string EmployeeID { get; set; }  // Added from view
        public string EmployeeCode { get; set; }  // Added from view
                                                  //public string EmployeePhoto { get; set; }  // Added from view
        public int? EmployeeName { get; set; }
        public int? UserID { get; set; }
    }
}
