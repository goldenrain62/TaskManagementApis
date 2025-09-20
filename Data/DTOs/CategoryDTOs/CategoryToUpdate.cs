using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.CategoryDTOs;

public class CategoryToUpdate : CategoryBase {
  public int Id { get; set; }

  // Override Validate method
  public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = base.Validate(validationContext).ToList();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Validate Id
    if (!db.Categories.Any(c => c.Id == Id))
      results.Add(new ValidationResult(
          $"Category with ID {Id} does not exist.",
          new[] { nameof(Id) }));

    return results;
  } 
}
