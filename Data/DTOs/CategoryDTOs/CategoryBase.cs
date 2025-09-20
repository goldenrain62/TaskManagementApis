using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.CategoryDTOs;

public abstract class CategoryBase : IValidatableObject {
  [MaxLength(100)]
  public string? Name { get; set; }

  [MaxLength(250)]
  public string? Description { get; set; }

  // Validation method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    => new List<ValidationResult>();
}

