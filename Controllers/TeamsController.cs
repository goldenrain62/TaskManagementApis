using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.TeamDTOs;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TeamsController : CustomBaseController<TeamsController> {

  public TeamsController(AppDbContext db, ILogger<TeamsController> logger) 
    : base(db, logger) {}

  [HttpGet]
  public async Task<IActionResult> GetTeams() {
    try {
      var teams = await _db.Teams
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
        }).ToListAsync();

      return Ok(new { success = true, message = "Successfully get all teams.", result = teams });
    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting teams.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetTeam(int id) {
    try {
      var team = await _db.Teams
        .Where(t => t.Id == id)
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
      }).FirstOrDefaultAsync();


      if (team == null)
        return BadRequest("Not found ID.");

      return Ok(new {
          success = true,
          message = $"Successfully get the team with ID: {id}.",
          result = team
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting the team with ID: {id}.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpGet("{id}/employees")]
  public async Task<IActionResult> GetEmployeesInTeam(int id) {
    try {
      var employees = await _db.Employees 
        .Where(e => e.TeamId == id)
        .Select(e => new {
          Id = e.Id,
          Firstname = e.Firstname,
          Lastname = e.Lastname,
          Dob = e.Dob,
          Gender = e.Gender,
          Phone = e.Phone,
          Email = e.Email,
          Team = e.TeamId,
          Manager = e.ManagerId,
          CreatedAt = e.CreatedAt,
          UpdatedAt = e.UpdatedAt
        }).ToListAsync();

      if (employees == null) return NotFound("Team Id is not found");

      return Ok(new {
        success = true,
        message = $"Successfully get employees in the team with ID: {id}.",
        result = employees
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting employees in team with ID: {id}.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpGet("{id}/leaders")]
  public async Task<IActionResult> GetLeadersInTeam(int id) {
    try {
      var leaders = await _db.Leaders
        .Where(l => l.TeamId == id)
        .Include(l => l.Employee)
        .Select(l => new {
          Id = l.EmployeeId,
          Firstname = l.Employee.Firstname,
          Lastname = l.Employee.Lastname,
          Dob = l.Employee.Dob,
          Gender = l.Employee.Gender,
          Phone = l.Employee.Phone,
          Email = l.Employee.Email,
          Leadership = l.Leadership,
          CreatedAt = l.CreatedAt,
          UpdatedAt = l.UpdatedAt
      }).ToListAsync();

      if (leaders == null) return NotFound("Team Id is not found.");

      return Ok(new {
        success = true,
        message = $"Successfully get leaders in the team with ID: {id}.",
        result = leaders
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting leaders in the team with ID: {id}.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPost]
  [Authorize(Policy = "CanCreateTeam(s)")]
  public async Task<IActionResult> CreateTeams([FromBody] List<TeamToCreate> teamsToCreate) {
    try {
      if (teamsToCreate.Count == 0) return BadRequest("No team to create");

      // Check if there are any required attributes that are null
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (!AreValidData(teamsToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      int maxId = await _db.Teams.AnyAsync() ?
        await _db.Teams.MaxAsync(t => t.Id) : 0;

      foreach (var team in teamsToCreate) {
        var newTeam = new Team {
          Name = team.Name,
          Description = team.Description
        };

        _db.Teams.Add(newTeam);
      }

      await _db.SaveChangesAsync();

      // Send response
      var newTeams = await _db.Teams
        .Where(t => t.Id > maxId)
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
        })
        .ToListAsync();

      return Ok(new { success = true, message = "Successfully created new team(s).", result = newTeams });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while creating team(s).");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPut]
  [Authorize(Policy = "CanUpdateTeam(s)")]
  public async Task<IActionResult> UpdateTeams([FromBody] List<TeamToUpdate> teamsToUpdate) {
    try {
      if (teamsToUpdate.Count == 0) 
        return BadRequest("No team to update");

      if (!AreValidData(teamsToUpdate, HttpContext.RequestServices))
        return ValidationErrorResponse();


      List<int> teamIds = new List<int>();

      foreach (var teamToUpdate in teamsToUpdate) {
        var team = await _db.Teams
          .Where(t => t.Id == teamToUpdate.Id)
          .FirstOrDefaultAsync();


        // Update Team attributes
        team.Name = teamToUpdate.Name ?? team.Name;
        team.Description = teamToUpdate.Description ?? team.Description;
        team.UpdatedAt = DateTime.Now;

        // Add team.Id to teamIds for getting updated teams
        teamIds.Add(team.Id);
      }

      await _db.SaveChangesAsync();

      // Send response
      var updatedTeams = await _db.Teams
        .Where(t => teamIds.Contains(t.Id))
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
      }).ToListAsync();

      return Ok(new {
          success = true,
          message = $"Successfully updated all {teamsToUpdate.Count} team(s).",
          result = updatedTeams
        });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while updating team(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpDelete]
  [Authorize(Policy = "CanDeleteTeam(s)")]
  public async Task<IActionResult> DeleteTeams([FromBody] List<int> teamIds) {
    try {
      var teamsToDelete = new List<Team>();

      foreach (var teamId in teamIds) {
        if (teamId == -1)
          return BadRequest($"Team with Id {teamId} cannot be deleted under no circumstances.");

        var team = await _db.Teams
          .Where(t => t.Id == teamId)
          .FirstOrDefaultAsync();

        if (team == null) return NotFound($"Not found team with Id {teamId} to delete.");

        teamsToDelete.Add(team);
      }

      _db.Teams.RemoveRange(teamsToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {teamIds.Count} team(s)." }); 
 
    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while deleting team(s).");
      return StatusCode(500, "Internal server error");
    }
  }
}

