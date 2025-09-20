using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.TaskDTOs;

public class TaskToUpdate : TaskBase {
  public int Id { get; set; }

  // Validation method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = base.Validate(validationContext).ToList();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    if (!db.Tasks.Any(t => t.Id == Id))
      results.Add(new ValidationResult(
        $"Task with Id {Id} does not exist.",
        new[] { nameof(Id) }));

    return results;
  }
}
