using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApis.Models;

public class Account {
  // Primary key
  [Key]
  [ForeignKey("BelongsTo")]
  public int Id { get; set; }

  public string? Username { get; set; }

  public string Password { get; set; }

  public string State { get; set; }

  // Foreign keys
  [ForeignKey("Role")]
  public int RoleId { get; set; }

  [ForeignKey("UpdatedByUser")]
  public int? UpdatedById { get; set; }

  // Timestamps
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
 
  // Navigation properties
  public Employee BelongsTo { get; set; }
  public Employee UpdatedByUser { get; set; }
  public Role Role { get; set; }

  public ICollection<Comment> Comments { get; set; } = new List<Comment>();
  public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
