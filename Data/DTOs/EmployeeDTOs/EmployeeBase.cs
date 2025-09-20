using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.EmployeeDTOs;

public abstract class EmployeeBase : IValidatableObject {
  [MaxLength(50)]
  public string? Firstname { get; set; }

  [MaxLength(50)]
  public string? Lastname { get; set; }

  public DateTime? Dob { get; set; } 
  public bool? Gender { get; set; }

  [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone must start with 0 and be exactly 10 digits.")]
  public string? Phone { get; set; }

  [EmailAddress(ErrorMessage = "Email format is invalid.")]
  public string? Email { get; set; }

  public int? TeamId { get; set; }
  public int? ManagerId { get; set; }

  // Validation Method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = new List<ValidationResult>();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Firstname: starts with uppercase, only letters
    if (!string.IsNullOrWhiteSpace(Firstname) &&
        !Regex.IsMatch(Firstname ?? "", @"^[A-Z][a-zA-Z]*$"))
      results.Add(new ValidationResult(
          "Firstname must start with an uppercase letter and contain only letters.",
          new[] { nameof(Firstname) }));

    // Lastname: optional, but if present must follow same rule
    if (!string.IsNullOrWhiteSpace(Lastname) && !Regex.IsMatch(Lastname, @"^[A-Z][a-zA-Z\s\-]*$"))
      results.Add(new ValidationResult(
          "Lastname must start with an uppercase letter and contain only letters, spaces, and hyphens.",
          new[] { nameof(Lastname) }));

    //  Age must be at least 18
    if (Dob != null) {
      var today = DateTime.Today;
      var age = today.Year - Dob.Value.Year;

      if (Dob.Value.Date > today.AddYears(-age)) age--;

      if (age < 18)
        results.Add(new ValidationResult(
          "Employee must be at least 18 years old.",
          new[] { nameof(Dob) }));
    }

    // Check Email exists
    if (Email != null && db.Employees.Any(e => e.Email == Email)) 
      results.Add(new ValidationResult(
        $"Email {Email} already exists.",
        new[] { nameof(Email) }));

    // Check TeamId exists
    if (TeamId != null) {
      if (!db.Teams.Any(t => t.Id == TeamId))
        results.Add(new ValidationResult(
            $"Team with ID {TeamId} does not exist.",
            new[] { nameof(TeamId) }));
      else if (TeamId == -1)
        results.Add(new ValidationResult(
            $"No employee can be a member of team with Id {TeamId}. Because it's an abstract team.",
            new[] { nameof(TeamId) }));
    }

    // Check ManagerId exists (if provided)
    if (ManagerId != null && !db.Employees.Any(e => e.Id == ManagerId))
      results.Add(new ValidationResult(
          $"Manager with ID {ManagerId} does not exist.",
          new[] { nameof(ManagerId) }));


    // Check Phone uniqueness
    if (!string.IsNullOrWhiteSpace(Phone) &&
        db.Employees.Any(e => e.Phone == Phone))
      results.Add(new ValidationResult(
          "Phone number already exists.",
          new[] { nameof(Phone) }));


    return results; 
  }
}
