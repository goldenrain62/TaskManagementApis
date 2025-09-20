using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApis.Models;

public class TaskState {
  [Key]
  public int Id { get; set; }

  public string Name { get; set; }
  public string Description { get; set; }

  [ForeignKey("CreatedByTeam")]
  public int? CreatedByTeamId { get; set; }

  [ForeignKey("CreatedBy")]
  public int? CreatedById { get; set; }

  [ForeignKey("UpdatedBy")]
  public int? UpdatedById { get; set; }

  // Timestamps
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation properties
  public Team CreatedByTeam { get; set; }
  public Employee CreatedBy { get; set; }
  public Employee UpdatedBy { get; set; }

  public ICollection<TASK> Tasks { get; set; } = new List<TASK>();
}
