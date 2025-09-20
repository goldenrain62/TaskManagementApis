using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.TeamDTOs;

public abstract class TeamBase : IValidatableObject {
  [MaxLength(100)]
  public string? Name { get; set; }

  [MaxLength(350)]
  public string? Description { get; set; }

  // Validation Method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    => new List<ValidationResult>();
}
