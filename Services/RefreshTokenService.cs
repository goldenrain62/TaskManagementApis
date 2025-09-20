using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TaskManagementApis.Data;
using TaskManagementApis.Models;
using TaskManagementApis.Services;

namespace TaskManagementApis.Services;

public class RefreshTokenService {
  private readonly AppDbContext _db;
  private readonly HasherService _hasher;

  public RefreshTokenService(AppDbContext db, HasherService hasher) {
    _db = db;
    _hasher = hasher;
  }

  // create cryptographically strong random token (raw) + hashed version for DB
  public (string rawToken, string tokenHash) CreateTokenPair() {
    // 32 bytes random → base64url ~43 chars
    byte[] bytes = RandomNumberGenerator.GetBytes(32);
    string raw = _hasher.Base64UrlEncode(bytes);
    string hash = _hasher.HashSha256(raw);
    return (raw, hash);
  }

  public async Task StoreAsync(int userId, string tokenHash, DateTime expiresAt, string? ip) {
    var entity = new RefreshToken {
        UserId = userId,
        TokenHash = tokenHash,
        ExpiresAt = expiresAt,
        CreatedByIp = ip
    };

    _db.RefreshTokens.Add(entity);
    await _db.SaveChangesAsync();
  }

  public async Task<RefreshToken?> GetActiveAsync(int userId, string rawToken) {
    string hash = _hasher.HashSha256(rawToken);
    return await _db.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.TokenHash == hash && rt.IsActive);
  }

  public async Task RotateAsync(RefreshToken oldToken, string newTokenHash, string? ip) {
    oldToken.RevokedAt = DateTime.Now;
    oldToken.RevokedByIp = ip;
    oldToken.ReplacedByTokenHash = newTokenHash;
    oldToken.IsActive = false;
    await _db.SaveChangesAsync();
  }
}
