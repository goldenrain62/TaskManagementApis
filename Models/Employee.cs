using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApis.Models;

public class Employee {
  // Primary key
  [Key]
  public int Id { get; set; }

  public string Firstname { get; set; }
  public string? Lastname { get; set; }

  public DateTime? Dob { get; set; } 
  public bool? Gender { get; set; }
  public string Phone { get; set; }
  public string Email { get; set; }

  // Foreign keys
  [ForeignKey("Team")]
  public int? TeamId { get; set; }

  [ForeignKey("Manager")]
  public int? ManagerId { get; set; }

  [ForeignKey("UpdatedByUser")]
  public int? UpdatedById { get; set; }

  // Timestamps
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation properties
  public Team Team { get; set; }
  public Employee Manager { get; set; }
  public Employee UpdatedByUser { get; set; }

  public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
  public ICollection<TASK> CreatedTasks { get; set; } = new List<TASK>();
  public ICollection<TASK> AssignedTasks { get; set; } = new List<TASK>();
  public ICollection<TaskState> TaskStatesCreated { get; set; } = new List<TaskState>();
}

