using System.ComponentModel.DataAnnotations;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.TaskStateDTOs;

public class TaskStateToUpdate : TaskStateBase {
  public int Id { get; set; }

  // Validation method
  public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = base.Validate(validationContext).ToList();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    if (!db.TaskStates.Any(ts => ts.Id == Id))
      results.Add(new ValidationResult(
        $"TaskState with ID {Id} does not exist.",
        new[] { nameof(Id) }));

    return results;
  }
}
