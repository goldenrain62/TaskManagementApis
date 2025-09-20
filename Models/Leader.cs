using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApis.Models;

public class Leader {
  [ForeignKey("Employee")]
  public int EmployeeId { get; set; }

  [ForeignKey("Team")]
  public int TeamId { get; set; }

  [ForeignKey("CreatedByUser")]
  public int? CreatedById { get; set; }

  public string Leadership { get; set; }

  // Timestamps
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation properties
  public Employee Employee { get; set; }
  public Team Team { get; set; }
  public Employee CreatedByUser { get; set; }
}
