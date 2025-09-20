using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TaskManagementApis.Data;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TaskManagementApis.Controllers;

public abstract class CustomBaseController<TController> : ControllerBase {
  protected readonly AppDbContext _db;
  protected readonly ILogger<TController> _logger;

  public CustomBaseController(AppDbContext db, ILogger<TController> logger) {
    _db = db;
    _logger = logger;
  }

  protected bool AreValidData<TDto>(List<TDto> inputData, IServiceProvider serviceProvider) {
    if (inputData == null)
      return false;

    // ModelState.Clear(); // Only do this if not relying on previous validation errors

    foreach (var data in inputData) {
      var context = new ValidationContext(data, serviceProvider, null);
      var results = new List<ValidationResult>();

      if (!Validator.TryValidateObject(data, context, results, true)) {
        foreach (var validationResult in results) {
          var field = validationResult.MemberNames.FirstOrDefault() ?? string.Empty;
          ModelState.AddModelError(field, validationResult.ErrorMessage);
        }
        return false;
      }
    }

    return true;
  }

  protected IActionResult ValidationErrorResponse() {
    var errors = ModelState
      .Where(kvp => kvp.Value.Errors.Count > 0)
      .Select(kvp => new {
          field = kvp.Key,
          messages = kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
      });

    return BadRequest(new { success = false, errors });
  }

  protected async Task<int?> GetTeamId(int userId) {
    var employee = await _db.Employees
      .Where(e => e.Id == userId)
      .FirstOrDefaultAsync();

    return employee?.TeamId;
  }
}
