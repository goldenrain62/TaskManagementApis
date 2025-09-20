using System.ComponentModel.DataAnnotations;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.TaskDTOs;

public abstract class TaskBase : IValidatableObject {
  [MaxLength(50)]
  public string? Name { get; set; }

  [MaxLength(250)]
  public string? Description { get; set; }

  public int? Progress { get; set; }
  public int? Priority { get; set; }
  public DateTime? Due { get; set; }
  public double? Rate { get; set; }
  public string? Note { get; set; }

  // Foreign Key attributes
  public int? CategoryId { get; set; }
  public int? ExecutedByTeamId { get; set; }
  public int? ExecutedByEmployeeId { get; set; }
  public int? ParentTaskId { get; set; }
  public int? CurrentStateId { get; set; }

  // Validation method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = new List<ValidationResult>();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Check Progress
    if (Progress != null)
      if (Progress < 0 || Progress > 100)
        results.Add(new ValidationResult(
          $"Progress must be in [0, 100].",
          new[] { nameof(Progress) }));

    // Check Due
    if (Due != null && Due < DateTime.Now)
      results.Add(new ValidationResult(
        $"Invalid due date.",
        new[] { nameof(Due) }));

    // Check CategoryId exists
    if (CategoryId != null && !db.Categories.Any(c => c.Id == CategoryId))
      results.Add(new ValidationResult(
        $"Category with ID {CategoryId} does not exist.",
        new[] { nameof(CategoryId) }));

    // Check ExecutedByTeamId exists
    if (ExecutedByTeamId != null && !db.Teams.Any(t => t.Id == ExecutedByTeamId))
      results.Add(new ValidationResult(
        $"ExecutedByTeam with ID {ExecutedByTeamId} does not exist.",
        new[] { nameof(ExecutedByTeamId) }));

    // Check ExecutedByEmployeeId exists
    if (ExecutedByEmployeeId != null && !db.Employees.Any(e => e.Id == ExecutedByEmployeeId))
      results.Add(new ValidationResult(
        $"ExecutedByEmployee with ID {ExecutedByEmployeeId} does not exist.",
        new[] { nameof(ExecutedByEmployeeId) }));

    // Check ParentTaskId exists
    if (ParentTaskId != null && !db.Tasks.Any(t => t.Id == ParentTaskId))
      results.Add(new ValidationResult(
        $"ParentTask with ID {ParentTaskId} does not exist.",
        new[] { nameof(ParentTaskId) }));

    // Check CurrentStateId exists
    if (CurrentStateId != null && !db.TaskStates.Any(ts => ts.Id == CurrentStateId))
      results.Add(new ValidationResult(
        $"CurrentState with ID {CurrentStateId} does not exist.",
        new[] { nameof(CurrentStateId) }));


    return results;
  }
}
