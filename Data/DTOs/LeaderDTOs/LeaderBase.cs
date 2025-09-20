using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TaskManagementApis.Data;

namespace TaskManagementApis.Data.DTOs.LeaderDTOs;

public class LeaderBase : IValidatableObject {
  public int EmployeeId { get; set; }
  public int TeamId { get; set; }

  [MaxLength(10)]
  public string Leadership { get; set; }


  // Validation method
  public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
    var results = new List<ValidationResult>();
    var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

    // Validate EmployeeId, TeamId
    if (!db.Employees.Any(e => e.Id == EmployeeId))
      results.Add(new ValidationResult(
        $"Employee with ID {EmployeeId} does not exist.",
        new[] { nameof(EmployeeId) }));

    if (!db.Teams.Any(t => t.Id == TeamId)) 
      results.Add(new ValidationResult(
        $"Team with ID {TeamId} does not exist.",
        new[] { nameof(TeamId) }));

    // Validate Leadership
    if (Leadership != "Leader" && Leadership != "Assistant")
      results.Add(new ValidationResult(
          "Leadership must be \"Leader\" or \"Assistant\".",
          new[] { nameof(Leadership) }));

    // Validate TeamId, Leadership. A Team can have only one Team Leader and have many Assistant Leaders.
    if (db.Leaders.Any(l => l.TeamId == TeamId && l.Leadership == "Leader"))
      results.Add(new ValidationResult(
        $"A Team can only have one Leader.",
        new[] { nameof(Leadership) }));

    return results;
  }
}
