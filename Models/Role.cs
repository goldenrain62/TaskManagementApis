using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApis.Models;

public class Role {
  // Primary key
  [Key]
  public int Id { get; set; }

  public string RoleName { get; set; }
  public string Description { get; set; }

  // Timestamps
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation properties
  public ICollection<Account> Accounts { get; set; } = new List<Account>();
}

