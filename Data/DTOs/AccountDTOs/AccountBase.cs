using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.AccountDTOs;

public class AccountBase : IValidatableObject {
  public int Id { get; set; }
  public string? Username { get; set; }
  public string? Password { get; set; }
  public string? State { get; set; }
  public int? RoleId { get; set; }

  // Validation method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = new List<ValidationResult>();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Validate Id
    if (!db.Employees.Any(e => e.Id == Id))
      results.Add(new ValidationResult(
          $"Account with Id {Id} does not exist.",
          new[] { nameof(Id) }));
    else if (Id == -1)
      results.Add(new ValidationResult(
          $"Unable to create an account for employee with Id {Id}. Because it's an abstract employee.",
          new[] { nameof(Id) }));

    // Validate Username
    if (!string.IsNullOrWhiteSpace(Username) && db.Accounts.Any(a => a.Username == Username))
      results.Add(new ValidationResult(
        $"Username {Username} already exists.",
        new[] { nameof(Username) }));

    // Validate Password
    if (!string.IsNullOrWhiteSpace(Password)) {
      bool hasUpper = Password.Any(char.IsUpper);
      bool hasLower = Password.Any(char.IsLower);
      bool hasDigit = Password.Any(char.IsDigit);
      bool hasSpecial = Password.Any(ch => !char.IsLetterOrDigit(ch));

      if (Password.Length < 10 || !hasUpper || !hasLower || !hasDigit || !hasSpecial)
        results.Add(new ValidationResult(
          "Password must be at least 10 characters and include uppercase, lowercase, number, and special character.",
          new[] { nameof(Password) }));
    }

    // Validate State
    if (!string.IsNullOrWhiteSpace(State) &&
        State != "Active" && State != "InActive")
      results.Add(new ValidationResult(
        "State must be 'Active' or 'InActive'.",
        new[] { nameof(State) }));

    // Validate RoleId
    if (RoleId != null && !db.Roles.Any(r => r.Id == RoleId))
      results.Add(new ValidationResult(
        $"RoleID with the ID {RoleId} does not exist.",
        new[] { nameof(RoleId) }));

    return results;
  }
}
