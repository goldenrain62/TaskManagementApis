using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApis.Models;

public class TASK {
  // Primary Key
  [Key]
  public int Id { get; set; }

  public string Name { get; set; }
  public string Description { get; set; }
  public int Progress { get; set; }
  public int? Priority { get; set; }
  public DateTime Due { get; set; }
  public double? Rate { get; set; }
  public string? Note { get; set; }

  // Foreign Keys
  [ForeignKey("Category")]
  public int CategoryId { get; set; }

  [ForeignKey("ExecutedByTeam")]
  public int ExecutedByTeamId { get; set; }

  [ForeignKey("ExecutedByEmployee")]
  public int ExecutedByEmployeeId { get; set; }

  [ForeignKey("ParentTask")]
  public int? ParentTaskId { get; set; }

  [ForeignKey("CurrentState")]
  public int CurrentStateId { get; set; }

  [ForeignKey("CreatedByTeam")]
  public int? CreatedByTeamId { get; set; }

  [ForeignKey("CreatedByUser")]
  public int CreatedById { get; set; }

  [ForeignKey("UpdatedByUser")]
  public int? UpdatedById { get; set; }

  // Timestamps
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation Properties
  public Category Category { get; set; }
  public Team ExecutedByTeam { get; set; }
  public Employee ExecutedByEmployee { get; set; }
  public TASK ParentTask { get; set; }
  public TaskState CurrentState { get; set; }
  public Team CreatedByTeam { get; set; }
  public Employee CreatedByUser { get; set; }
  public Employee UpdatedByUser { get; set; }

  public ICollection<TASK> SubTasks { get; set; } = new List<TASK>();
  public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

