using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.TeamDTOs;

public class TeamToUpdate : TeamBase {
  public int Id { get; set; }

  // Override Validate method
  public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = base.Validate(validationContext).ToList();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Validate Id
    if (!db.Teams.Any(t => t.Id == Id))
      results.Add(new ValidationResult(
          $"Team with ID {Id} does not exist.",
          new[] { nameof(Id) }));

    return results;
  }
}
