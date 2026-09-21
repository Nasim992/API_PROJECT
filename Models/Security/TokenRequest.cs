using Microsoft.AspNetCore.Mvc;

namespace Dextor.API.Models.Security;

public class TokenRequest
{
    // Standard OAuth2 fields
    [FromForm(Name = "grant_type")]
    public string? GrantType { get; set; }

    [FromForm(Name = "username")]
    public string? UserName { get; set; }

    [FromForm(Name = "password")]
    public string? Password { get; set; }

    // Your custom form parameters
    [FromForm(Name = "UniqueSerialNo")]
    public string? UniqueSerialNo { get; set; }

    [FromForm(Name = "ApplicationType")]
    public string? ApplicationType { get; set; }

    [FromForm(Name = "ERPVersionNo")]
    public string? ERPVersionNo { get; set; }
}