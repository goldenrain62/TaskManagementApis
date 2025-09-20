using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagementApis.Models;

public class Team {
  // Primary key
  [Key]
  public int Id { get; set; }

  public string Name { get; set; }

  public string Description { get; set; }

  // Timestamps
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation property: One team has many employees, leaders 
  public ICollection<Employee> Employees { get; set; } = new List<Employee>();
  public ICollection<Leader> Leaders { get; set; } = new List<Leader>();
  public ICollection<TASK> AssignedTasks { get; set; } = new List<TASK>();
  public ICollection<TASK> CreatedTasks { get; set; } = new List<TASK>();
}
