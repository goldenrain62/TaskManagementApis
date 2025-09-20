using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.EmployeeDTOs;

public class EmployeeToUpdate : EmployeeBase {
  public int Id { get; set; }

  // Override Validate method
  public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = base.Validate(validationContext).ToList();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Validate Id
    if (!db.Employees.Any(e => e.Id == Id))
      results.Add(new ValidationResult(
          $"Employee with ID {Id} does not exist.",
          new[] { nameof(Firstname) }));

    return results;
  }
}
