using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.AuthDTOs;
using TaskManagementApis.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : CustomBaseController<AuthController> {
  private readonly JwtService _jwtService;
  private readonly RefreshTokenService _rtService;
  private readonly HasherService _hasher;

  public AuthController(AppDbContext db, ILogger<AuthController> logger, 
      JwtService jwtService, RefreshTokenService rtService, HasherService hasher) : base(db, logger) {
      _jwtService = jwtService;
      _rtService = rtService;
      _hasher = hasher;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] Login loginDto) {
    try {
      if (Request.Cookies.TryGetValue("rftk", out var refreshToken) &&
          Request.Cookies.TryGetValue("uid", out var userId))
        return BadRequest("There's a user already authenticated. Please log out to authenticate a new one.");

      // Validate account 
      var acc = await _db.Accounts.Include(a => a.Role)
        .FirstOrDefaultAsync(a => a.Id == loginDto.Id
            && a.Password == _hasher.ComputeMD5Hash(loginDto.Password) 
            && a.State == "Active");

      if (acc == null) return BadRequest("Invalid account or inactive account");

      // Create, store refresh token and set cookies
      var (rawRefresh, hashedRefresh) = _rtService.CreateTokenPair();
      await _rtService.StoreAsync(acc.Id, hashedRefresh, DateTime.Now.AddDays(7), GetIp());
      SetCookies(rawRefresh, acc.Id.ToString(), acc.Username, acc.Role.RoleName, 7);

      // Create jwt token
      var jwtToken = _jwtService.GenerateToken(acc, acc.Role);

      return Ok(new { success = true, message = $"User with ID {acc.Id} logged in successfully", 
          token = jwtToken, expiresIn = "60m" });

    } catch (Exception ex) {
        _logger.LogError(ex, $"Fail to authenticate user");
        return StatusCode(500, "Internal server error");
    }
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh() {
    try {
      if (!Request.Cookies.TryGetValue("rftk", out var currentRefreshToken) ||
          !Request.Cookies.TryGetValue("uid", out var userId))
          return Unauthorized("Missing userId or refresh token.");

      // Get the old refresh token
      var oldRefreshToken = await _rtService.GetActiveAsync(int.Parse(userId), currentRefreshToken);
      // Create a new refresh token
      var (newRawRefresh, newHashedRefresh) = _rtService.CreateTokenPair();
      // Save the new refresh token
      await _rtService.StoreAsync(int.Parse(userId), newHashedRefresh, DateTime.Now.AddDays(7), GetIp());
      // Rotate the old refresh token 
      await _rtService.RotateAsync(oldRefreshToken, newHashedRefresh, GetIp());


      // Create a jwt token
      var acc = await _db.Accounts
        .Include(a => a.Role)
        .FirstOrDefaultAsync(a => a.Id == int.Parse(userId));

      var jwtToken = _jwtService.GenerateToken(acc, acc.Role);

      SetCookies(newRawRefresh, acc.Id.ToString(), acc.Username, acc.Role.RoleName, 7);

      return Ok(new { success = true, token = jwtToken, expiresIn = "60m" });

    } catch (Exception ex) {
        _logger.LogError(ex, "An error occured while refreshing token");
        return StatusCode(500, "Internal server error");
    }
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout() {
    try {
      if (Request.Cookies.TryGetValue("rftk", out var refreshToken) &&
          Request.Cookies.TryGetValue("uid", out var userId)) {

        var existingRefreshToken = await _rtService.GetActiveAsync(int.Parse(userId), refreshToken);
        await _rtService.RotateAsync(existingRefreshToken, "", GetIp());

        RemoveCookies();

        return Ok(new { success = true, message = $"User with ID {userId} logged out successfully."});
      }

      return BadRequest("No user to log out");
    } catch (Exception ex) {
        _logger.LogError(ex, "An error occured while logging out");
        return StatusCode(500, "Internal server error");
    }
  }

  [HttpPost("rac")]
  public async Task<IActionResult> RemoveAllCookies() {
    RemoveCookies();

    return Ok();
  }

  private void SetCookies(string rawRefresh, string userId, string userName, string userRole, int days) {
    Response.Cookies.Append("rftk", rawRefresh, new CookieOptions {
      HttpOnly = true, // True: js can't read the rawRefresh; False: js can read 
      Secure = true,   // true in production (HTTPS)
      SameSite = SameSiteMode.Strict,
      Expires = DateTimeOffset.Now.AddDays(days),
      Path = "/api/v1/"
    });

    Response.Cookies.Append("uid", userId, new CookieOptions {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTimeOffset.Now.AddDays(days),
      Path = "/api/v1/"
    });

    Response.Cookies.Append("uname", userName, new CookieOptions {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTimeOffset.Now.AddDays(days),
      Path = "/api/v1/"
    });

    Response.Cookies.Append("urole", userRole, new CookieOptions {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTimeOffset.Now.AddDays(days),
      Path = "/api/v1/"
    });
  }

  private void RemoveCookies() {
    Response.Cookies.Delete("rftk",  new CookieOptions {
      Path = "/api/v1/"
    });

    Response.Cookies.Delete("uid", new CookieOptions {
      Path = "/api/v1/"
    });

    Response.Cookies.Delete("uname",  new CookieOptions {
      Path = "/api/v1/"
    });

    Response.Cookies.Delete("urole",  new CookieOptions {
      Path = "/api/v1/"
    });
  }

  // Get Ip method
  private string? GetIp()
    => HttpContext.Connection.RemoteIpAddress?.ToString();
}

