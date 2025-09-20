using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.RoleDTOs;

public abstract class RoleBase : IValidatableObject {
  [MaxLength(100)]
  public string? RoleName { get; set; }

  [MaxLength(250)]
  public string? Description { get; set; }

  // Validation Method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = new List<ValidationResult>();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Check RoleName exists
    if (!string.IsNullOrWhiteSpace(RoleName) &&
        db.Roles.Any(r => r.RoleName == RoleName))
      results.Add(new ValidationResult(
          $"RoleName '{RoleName}' already exists.",
          new[] { nameof(RoleName) }));

    return results;
  }
}

