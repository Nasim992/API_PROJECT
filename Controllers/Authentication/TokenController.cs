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

namespace Dextor.API.Controllers.Authentication;

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
    [HttpGet("api/token")]
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


            var anUser = _userRepository.Find(a => a.UserName == username);
            if (anUser == null)
            {
                return BadRequest(new { error = "invalid_username", error_description = "Provided username or password is incorrect" });
            }

            if (applicationType.Equals("ANDROID", StringComparison.OrdinalIgnoreCase) && anUser.EmployeeId == null)
            {
                return BadRequest(new { IsSuccess = "false", error = "employee_tag", error_description = "Incomplete employee information with your user name" });
            }


            var userDetails = await _db.Database
                .SqlQueryRaw<vUser>("SELECT * FROM v_User WHERE ISNULL(IsActive, 0) = 1 AND UserName = {0}", username)
                .FirstOrDefaultAsync();

            if (userDetails == null)
            {
                return BadRequest(new { error = "No Active User Found", error_description = "No Active User Found" });
            }

            var mph3 = new PdsaHash(PdsaHash.PdsaHashType.MD5);
            var hashedInputPassword = mph3.CreateHash(password, userDetails.Salt);


            if (hashedInputPassword != userDetails.Password)
            {
                return BadRequest(new { IsSuccess = "false", error = "invalid_grant", error_description = "Provided username or password is incorrect" });
            }

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

            var permissionKeys = (userDetails.PermissionKey ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            var userPermissions = permissionKeys
                .Select(pk => new UserPermission { UserID = userDetails.UserID, PermissionKey = pk })
                .ToList();
            var menuPermissionListString = JsonSerializer.Serialize(userPermissions);

            var erpVersionNo = await _db.Database
                .SqlQueryRaw<int>("SELECT ERPVersionNo AS Value FROM t_ThisSystem WHERE ID = 1")
                .FirstOrDefaultAsync();

            var tokenString = GenerateJwt(userDetails, erpVersionNo, menuPermissionListString);

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
            
            new Claim("EmployeeId", user.EmployeeID.ToString() ?? "-1"),

            new Claim("UserId", user.UserID.ToString()),
            new Claim("UserName", user.UserName ?? ""),
            new Claim("ERPVersionNo", erpVersionNo.ToString()),
            //new Claim("MenupermissionList", menuPermissions)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            //expires: DateTime.UtcNow.AddDays(1),
            expires: DateTime.UtcNow.AddMinutes(10), // 10 minutes expiration
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}