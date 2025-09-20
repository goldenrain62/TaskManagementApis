using System.ComponentModel.DataAnnotations;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.CommentDTOs;

public abstract class CommentBase : IValidatableObject {
  public string? Content { get; set; }

  public int? TaskId { get; set; }
  public int? RepliesTo { get; set; }

  // Validation method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = new List<ValidationResult>();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Check TaskId exists
    if (TaskId != null && !db.Tasks.Any(t => t.Id == TaskId))
      results.Add(new ValidationResult(
        $"TaskId {TaskId} does not exist.",
        new[] { nameof(TaskId) }));

    // Check RepliesTo exists
    if (RepliesTo != null && !db.Comments.Any(c => c.Id == RepliesTo))
      results.Add(new ValidationResult(
        $"RepliesTo {RepliesTo} does not exist.",
        new[] { nameof(RepliesTo) }));

    return results;
  }
}
