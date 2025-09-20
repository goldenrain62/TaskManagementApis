using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.RoleDTOs;

public class RoleToUpdate : RoleBase {
  public int Id { get; set; }

  // Override Validate method
  public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = base.Validate(validationContext).ToList();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Validate Id
    if (!db.Roles.Any(r => r.Id == Id))
      results.Add(new ValidationResult(
          $"Role with ID {Id} does not exist.",
          new[] { nameof(Id) }));

    return results;
  }
}
