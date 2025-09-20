using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApis.Models;

public class RefreshToken {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Account")]
    public int UserId { get; set; }

    public string TokenHash { get; set; }      // store SHA256 hash, not raw token

    public DateTime ExpiresAt { get; set; }
    // public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByTokenHash { get; set; }      // for rotation chain
    public bool IsActive { get; set; } = true;

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Account Account { get; set; }
}
