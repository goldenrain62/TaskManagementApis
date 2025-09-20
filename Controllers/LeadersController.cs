using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.LeaderDTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LeadersController : CustomBaseController<LeadersController> {

  public LeadersController(AppDbContext db, ILogger<LeadersController> logger) 
    : base(db, logger) {}

  
  [HttpGet]
  public async Task<IActionResult> GetLeaders() {
    try {
      var leaders = await _db.Leaders
        .Include(l => l.Employee)
        .Include(e => e.Team)
        .Select(l => new {
          LeaderId = l.EmployeeId,
          LeaderFirstName = l.Employee.Firstname,
          LeaderLastName = l.Employee.Lastname,
          TeamId = l.TeamId,
          Team = l.Employee.Team.Name,
          Leadership = l.Leadership,
          CreatedAt = l.CreatedAt,
          UpdatedAt = l.UpdatedAt
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = "Successfully get all leaders.",
        result = leaders
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting leaders.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPost]
  [Authorize(Policy = "CanCreateLeader(s)")]
  public async Task<IActionResult> CreateLeaders([FromBody] List<LeaderBase> leadersToCreate) {
    try {
      if (leadersToCreate.Count == 0) return BadRequest("No employee to assign.");

      if (!AreValidData(leadersToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      var employeeIds = new List<int>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

      foreach (var leaderToCreate in leadersToCreate) {
        var leader = new Leader {
          EmployeeId = leaderToCreate.EmployeeId,
          TeamId = leaderToCreate.TeamId,
          Leadership = leaderToCreate.Leadership,
          CreatedById = userId
        };

        employeeIds.Add(leader.EmployeeId);

        _db.Leaders.Add(leader);
      }

      await _db.SaveChangesAsync();

      var newLeaders = await _db.Leaders
        .Where(l => employeeIds.Contains(l.EmployeeId))
        .Include(l => l.Employee)
        .Include(e => e.Team)
        .Select(l => new {
          LeaderId = l.EmployeeId,
          LeaderFirstName = l.Employee.Firstname,
          LeaderLastName = l.Employee.Lastname,
          TeamId = l.TeamId,
          Team = l.Employee.Team.Name,
          Leadership = l.Leadership,
          CreatedAt = l.CreatedAt,
          UpdatedAt = l.UpdatedAt
      }).ToListAsync();

      return Ok(new { success = true, message = "Successfully created new leader(s).", result = newLeaders });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while creating leader(s).");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpDelete]
  [Authorize(Policy = "CanDeleteLeader(s)")]
  public async Task<IActionResult> DeleteLeaders([FromBody] List<int> leaderIds) {
    try {
      var leadersToDelete = new List<Leader>();

      foreach (var leaderId in leaderIds) {
        var leader = await _db.Leaders
          .Where(l => l.EmployeeId == leaderId)
          .FirstOrDefaultAsync();

        if (leader == null) return NotFound($"Not found leader with Id {leaderId} to delete.");


        leadersToDelete.Add(leader);
      }

      _db.Leaders.RemoveRange(leadersToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {leaderIds.Count} leader(s)." }); 
 
    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while deleting leader(s).");
      return StatusCode(500, "Internal server error");
    }
  }
}
