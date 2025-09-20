using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.CommentDTOs;

public class CommentToUpdate : CommentBase {
  public int Id { get; set; }

  // Validation method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = base.Validate(validationContext).ToList();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    if (!db.Comments.Any(c => c.Id == Id))
      results.Add(new ValidationResult(
        $"Comment with Id {Id} does not exist.",
        new[] { nameof(Id) }));

    return results;
  }
}
