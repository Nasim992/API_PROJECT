using Dextor.API.Models.Security;
using Dextor.API.Data.IRepositories;
using Dextor.API.Models.Core;
using Dextor.API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Dextor.API.Models.BindingModel;

namespace Dextor.API.Controllers;

[ApiController]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly DatabaseContext _db;
    private readonly IUserRepository _userRepository;
    private readonly IUserRegistrationRepository _userRegistrationRepository;
    private readonly IReportLogRepository _reportLogRepository;

    public TokenController(
        IConfiguration config,
        DatabaseContext db,
        IUserRepository userRepository,
        IUserRegistrationRepository userRegistrationRepository,
        IReportLogRepository reportLogRepository)
    {
        _config = config;
        _db = db;
        _userRepository = userRepository;
        _userRegistrationRepository = userRegistrationRepository;
        _reportLogRepository = reportLogRepository;
    }

    // FIX 2: Restrict to Form-UrlEncoded (Standard OAuth2) to prevent binding crashes
    [HttpPost("api/token")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> GetToken([FromForm] TokenRequest request)
    {
        try
        {
            var username = request.UserName?.Trim();
            var password = request.Password;
            var uniqueSerialNo = request.UniqueSerialNo;
            var applicationType = request.ApplicationType ?? string.Empty;
            var versionNo = string.IsNullOrWhiteSpace(request.ERPVersionNo) ? "1.0" : request.ERPVersionNo;

            // 1. Basic Validations
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { error = "invalid_grant", error_description = "Please provide user name" });
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(new { error = "invalid_grant", error_description = "Please provide password" });
            }

            if (applicationType.Equals("ANDROID", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(uniqueSerialNo))
            {
                return BadRequest(new { error = "empty_device_info", error_description = "Unique Serial No Is Empty" });
            }

            // 2. Find User
            var anUser = _userRepository.Find(a => a.UserName == username);
            if (anUser == null)
            {
                return BadRequest(new { error = "invalid_username", error_description = "Provided username or password is incorrect" });
            }

            if (applicationType.Equals("ANDROID", StringComparison.OrdinalIgnoreCase) && anUser.EmployeeId == null)
            {
                return BadRequest(new { IsSuccess = "false", error = "employee_tag", error_description = "Incomplete employee information with your user name" });
            }

            // 3. User Details View Query
            var userDetails = await _db.Database
                .SqlQueryRaw<vUser>("SELECT * FROM v_User WHERE ISNULL(IsActive, 0) = 1 AND UserName = {0}", username)
                .FirstOrDefaultAsync();

            if (userDetails == null)
            {
                return BadRequest(new { error = "No Active User Found", error_description = "No Active User Found" });
            }

            // 4. Password Verification
            var mph3 = new PdsaHash(PdsaHash.PdsaHashType.MD5);
            var hashedInputPassword = mph3.CreateHash(password, userDetails.Salt);

          //  var test = CrackLegacyHash(password, userDetails.Salt,userDetails.Password);

            if (hashedInputPassword != userDetails.Password)
            {
                return BadRequest(new { IsSuccess = "false", error = "invalid_grant", error_description = "Provided username or password is incorrect" });
            }

            // 5. Android Device Registration Check
            if (applicationType.Equals("ANDROID", StringComparison.OrdinalIgnoreCase))
            {
                var aRegistration = _userRegistrationRepository.Find(a =>
                    a.UserName == username && a.UniqueSerialNo == uniqueSerialNo);

                if (aRegistration == null)
                {
                    return BadRequest(new { error = "unrecognize_device", error_description = "UniQue Serial Not Matched on your Registration" });
                }

                if (aRegistration.Status != "Active")
                {
                    return BadRequest(new { error = "inactive_user", error_description = "Inactive user. Please contact IT department." });
                }

                if (aRegistration.VersionNo != versionNo)
                {
                    aRegistration.VersionNo = versionNo;
                    _userRegistrationRepository.Update(aRegistration);
                    _userRegistrationRepository.Save();
                }
            }

            // 6. Build Permissions JSON
            var permissionKeys = (userDetails.PermissionKey ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            var userPermissions = permissionKeys
                .Select(pk => new UserPermission { UserID = userDetails.UserID, PermissionKey = pk })
                .ToList();
            var menuPermissionListString = JsonSerializer.Serialize(userPermissions);

            // 7. Fetch System Version
            var erpVersionNo = await _db.Database
                .SqlQueryRaw<int>("SELECT ERPVersionNo FROM t_ThisSystem WHERE ID = 1")
                .FirstOrDefaultAsync();

            // 8. Generate JWT Token
            var tokenString = GenerateJwt(userDetails, erpVersionNo, menuPermissionListString);

            // 9. Return the exact response payload expected by old clients
            var response = new TokenResponse
            {
                AccessToken = tokenString,
                TokenType = "bearer",
                ExpiresIn = 86400, // 24 hours
                UserName = userDetails.UserName ?? "",
                UserFullName = userDetails.UserFullName ?? "",
                ERPVersionNo = erpVersionNo.ToString(),
                RoleID = userDetails.RoleID.ToString(),
                RoleName = userDetails.RoleName ?? "",
                EmployeeID = userDetails.EmployeeID.ToString() ?? "-1",
                UserId = userDetails.UserID.ToString(),
                UserGroup = userDetails.UserGroup ?? "0",
                UserGroupId = userDetails.UserGroupID?.ToString() ?? "0",
                DefaultAppDashboard = userDetails.DefaultAppDashboard?.ToString() ?? "0",
                MenuPermissionList = menuPermissionListString,
                EmployeeName = userDetails.EmployeeName ?? "",
                EmployeeCode = userDetails.EmployeeCode ?? "",
                CardNo = userDetails.CardNo ?? "0",
                IsAllowBackgroundLocation = userDetails.IsAllowBackgroundLocation.ToString(),
                LocationSyncInterval = userDetails.LocationSyncInterval.ToString(),
                ReportLogEnable = userDetails.IsReportLogEnabled.ToString()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "exception", error_description = ex.Message });
        }
    }

    private string GenerateJwt(vUser user, int erpVersionNo, string menuPermissions)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserFullName ?? user.UserName ?? ""),
            new Claim(ClaimTypes.Role, user.RoleName ?? ""),
            
            // FIX 1: Added the question mark (?) after EmployeeID to handle nullable integers safely
            new Claim("EmployeeId", user.EmployeeID.ToString() ?? "-1"),

            new Claim("UserId", user.UserID.ToString()),
            new Claim("UserName", user.UserName ?? ""),
            new Claim("ERPVersionNo", erpVersionNo.ToString()),
            new Claim("MenupermissionList", menuPermissions)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    private string CrackLegacyHash(string password, string salt, string targetHash)
    {
        var encodings = new Dictionary<string, Encoding>
    {
        { "UTF8", Encoding.UTF8 },
        { "ASCII", Encoding.ASCII },
        { "Unicode (Default .NET 4.5)", Encoding.Unicode }
    };

        using var md5 = System.Security.Cryptography.MD5.Create();

        foreach (var enc in encodings)
        {
            // 1. Password + Salt
            if (Convert.ToBase64String(md5.ComputeHash(enc.Value.GetBytes(password + salt))) == targetHash)
                return $"MATCH! Use {enc.Key} and order: plainText + salt";

            // 2. Salt + Password
            if (Convert.ToBase64String(md5.ComputeHash(enc.Value.GetBytes(salt + password))) == targetHash)
                return $"MATCH! Use {enc.Key} and order: salt + plainText";

            // 3. Just Password (no salt used)
            if (Convert.ToBase64String(md5.ComputeHash(enc.Value.GetBytes(password))) == targetHash)
                return $"MATCH! Use {enc.Key} and ignore the salt completely.";

            // 4. Base64 Decoded Salt
            try
            {
                byte[] pwdBytes = enc.Value.GetBytes(password);
                byte[] saltBytes = Convert.FromBase64String(salt);

                // Password Bytes + Salt Bytes
                byte[] combo1 = new byte[pwdBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(pwdBytes, 0, combo1, 0, pwdBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, combo1, pwdBytes.Length, saltBytes.Length);
                if (Convert.ToBase64String(md5.ComputeHash(combo1)) == targetHash)
                    return $"MATCH! Base64 Decode the Salt, then Password + Salt using {enc.Key}";

                // Salt Bytes + Password Bytes
                byte[] combo2 = new byte[pwdBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, combo2, 0, saltBytes.Length);
                Buffer.BlockCopy(pwdBytes, 0, combo2, saltBytes.Length, pwdBytes.Length);
                if (Convert.ToBase64String(md5.ComputeHash(combo2)) == targetHash)
                    return $"MATCH! Base64 Decode the Salt, then Salt + Password using {enc.Key}";
            }
            catch { }
        }

        return "No match found. The legacy PDSA class did something custom.";
    }
}