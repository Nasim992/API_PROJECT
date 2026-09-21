using System.Text.Json.Serialization;

namespace Dextor.API.Models.Security;

public class TokenResponse
{
    // OAuth 2.0 Standard fields
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "bearer";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; } = 86400; // 1 Day in seconds

    // Your custom properties from CreateProperties()
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("userfullname")]
    public string UserFullName { get; set; } = string.Empty;

    [JsonPropertyName("ERPVersionNo")]
    public string ERPVersionNo { get; set; } = string.Empty;

    [JsonPropertyName("RoleID")]
    public string RoleID { get; set; } = string.Empty;

    [JsonPropertyName("RoleName")]
    public string RoleName { get; set; } = string.Empty;

    [JsonPropertyName("EmployeeID")]
    public string EmployeeID { get; set; } = string.Empty;

    [JsonPropertyName("UserId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("UserGroup")]
    public string UserGroup { get; set; } = "0";

    [JsonPropertyName("UserGroupId")]
    public string UserGroupId { get; set; } = "0";

    [JsonPropertyName("DefaultAppDashboard")]
    public string DefaultAppDashboard { get; set; } = "0";

    [JsonPropertyName("MenuPermissionList")]
    public string MenuPermissionList { get; set; } = string.Empty;

    [JsonPropertyName("EmployeeName")]
    public string EmployeeName { get; set; } = string.Empty;

    [JsonPropertyName("EmployeeCode")]
    public string EmployeeCode { get; set; } = string.Empty;

    [JsonPropertyName("CardNo")]
    public string CardNo { get; set; } = "0";

    [JsonPropertyName("IsAllowBackgroundLocation")]
    public string IsAllowBackgroundLocation { get; set; } = "0";

    [JsonPropertyName("LocationSyncInterval")]
    public string LocationSyncInterval { get; set; } = "0";

    [JsonPropertyName("ReportLogEnable")]
    public string ReportLogEnable { get; set; } = "0";

    [JsonPropertyName("pmsSelf")]
    public string PmsSelf { get; set; } = "https://apexicures.com/PMS/PMS/GetPmsData";

    [JsonPropertyName("pmsSupervisor")]
    public string PmsSupervisor { get; set; } = "https://apexicures.com/PMS/PMS/EmployeesPMSList";

    [JsonPropertyName("pmsAdmin")]
    public string PmsAdmin { get; set; } = "https://apexicures.com/PMS/PMS/EmployeesPMSList";
}