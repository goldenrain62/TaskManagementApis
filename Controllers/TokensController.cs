using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Services;
using TaskManagementApis.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TokensController : CustomBaseController<TokensController> {

  public TokensController(AppDbContext db, ILogger<TokensController> logger)
    : base(db, logger) {}

  [HttpGet]
  [Authorize(Policy = "CanGetTokens")]
  public async Task<IActionResult> GetTokens() {
    try {
      var refreshTokens = await _db.RefreshTokens
        .Select(rf => new RefreshToken {
          Id = rf.Id,
          UserId = rf.Id,
          TokenHash = rf.TokenHash,
          ExpiresAt = rf.ExpiresAt,
          CreatedByIp = rf.CreatedByIp,
          RevokedAt = rf.RevokedAt,
          RevokedByIp = rf.RevokedByIp,
          ReplacedByTokenHash = rf.ReplacedByTokenHash,
          IsActive = rf.IsActive,
          CreatedAt = rf.CreatedAt,
          UpdatedAt = rf.UpdatedAt
        }).ToListAsync();

      return Ok(new { success = true, message = "Get all tokens successfully.", result = refreshTokens });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting refresh token(s).");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "CanDeleteTokens")]
  public async Task<IActionResult> DeleteToken(int id) {
    try {
      if (id == null) return BadRequest("Missing id");

      int deletedCount = await _db.RefreshTokens
        .Where(rf => rf.Id == id)
        .ExecuteDeleteAsync();

      if (deletedCount == 0)
        return BadRequest("Not found id");


      return Ok(new { success = true, message = $"Token with ID {id} token has been deleted." });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while deleting the refresh token with ID {id}.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpDelete("type/{type}")]
  [Authorize(Policy = "CanDeleteTokens")]
  public async Task<IActionResult> DeleteTokens(string type) {
    try {
      type = (type ?? "inactive").ToLowerInvariant();

      if (type != "all" && type != "inactive")
        return BadRequest("Invalid type. Must be 'all' or 'inactive'.");

      int deletedCount = type == "all" 
        ? await _db.RefreshTokens.ExecuteDeleteAsync()
        : await _db.RefreshTokens
          .Where(rf => !rf.IsActive).ExecuteDeleteAsync();

      return Ok(new { success = true, message = $"{deletedCount} tokens have been deleted." });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while deleting refresh token(s).");
      return StatusCode(500, "Internal server error");
    }
  }
}
