using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.RoleDTOs;
using TaskManagementApis.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RolesController : CustomBaseController<RolesController> {

  public RolesController(AppDbContext db, ILogger<RolesController> logger) : base(db, logger) {} 

  [HttpGet]
  public async Task<IActionResult> GetRoles() {
    try {

      var roles = await _db.Roles
        .Select(r => new {
          r.Id,
          r.RoleName,
          r.Description,
          r.CreatedAt,
          r.UpdatedAt
        }).ToListAsync();

      return Ok(new { 
        success = true,
        message = "Successfully got all roles.",
        result = roles 
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting role(s) data.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetRole(int id) {
    try {

      var role = await _db.Roles
        .Where(r => r.Id == id)
        .Select(r => new {
          r.Id,
          r.RoleName,
          r.Description,
          r.CreatedAt,
          r.UpdatedAt
        }).FirstOrDefaultAsync();

      if (role == null) return NotFound();

      return Ok(new {
        success = true,
        message = $"Successfully got the role with ID {id}.",
        result = role
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting the role with ID {id}.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPost]
  [Authorize(Policy = "CanCreateRole(s)")]
  public async Task<IActionResult> CreateRoles([FromBody] List<RoleToCreate> rolesToCreate) {
    try {
      if (rolesToCreate.Count == 0) return BadRequest("No role(s) to create."); 

      // Check if there are any required attributes that are null
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Validate
      if (!AreValidData(rolesToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse(); 

      // Save new role(s) in db
      List<string> newRoleNames = new List<string>();

      foreach (var roleToCreate in rolesToCreate ) {
        var newRole = new Role {
          RoleName = roleToCreate.RoleName,
          Description = roleToCreate.Description,
          CreatedAt = DateTime.Now,
          UpdatedAt = DateTime.Now
        };

        newRoleNames.Add(newRole.RoleName);
        _db.Roles.Add(newRole);
      }

      await _db.SaveChangesAsync();

      // Send response
      var newRoles = await _db.Roles
        .Where(r => newRoleNames.Contains(r.RoleName))
        .Select(r => new {
          Id = r.Id,
          RoleName = r.RoleName,
          Description = r.Description,
          CreatedAt = r.CreatedAt,
          UpdatedAt = r.UpdatedAt
        })
        .ToListAsync();

      return Ok(new {
          success = true,
          message = "Successfully created new role(s).",
          result = newRoles
      });

    } catch (Exception ex) {
        _logger.LogError(ex, "An error occured while creating role(s) data");
        return StatusCode(500, "Internal server error");
    }
  }

  [HttpPut]
  [Authorize(Policy = "CanUpdateRole(s)")]
  public async Task<IActionResult> UpdateRoles([FromBody] List<RoleToUpdate> rolesToUpdate) {
    try {
      if (rolesToUpdate.Count == 0) return BadRequest("No role(s) data to update");

      // Validate
      if (!AreValidData(rolesToUpdate, HttpContext.RequestServices))
        return ValidationErrorResponse(); 


      List<int> roleIds = new List<int>();


      foreach (var roleToUpdate in rolesToUpdate) {
        var role = await _db.Roles.FindAsync(roleToUpdate.Id);


        // Update Role attributes
        role.RoleName = roleToUpdate.RoleName ?? role.RoleName;
        role.Description = roleToUpdate.Description ?? role.Description;
        role.UpdatedAt = DateTime.Now;

        // Add role.Id to roleIds list for getting updated role
        roleIds.Add(role.Id);
      }

      await _db.SaveChangesAsync();

      // Send response
      var updatedRoles = await _db.Roles
        .Where(r => roleIds.Contains(r.Id))
        .Select(r => new {
            Id = r.Id,
            RoleName = r.RoleName,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
      }).ToListAsync();


      return Ok(new {
        success = true,
        message = $"Successfully updated all {roleIds.Count} role(s).",
        result = updatedRoles
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while updating role(s) data");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpDelete]
  [Authorize(Policy = "CanDeleteRole(s)")]
  public async Task<IActionResult> DeleteRoles([FromBody] List<int> roleIds) {
    try {
      var rolesToDelete = new List<Role>();

      foreach (var roleId in roleIds) {
        var role = await _db.Roles
          .Where(r => r.Id == roleId)
          .FirstOrDefaultAsync();

        if (role == null) return NotFound($"Not found role with Id {roleId} to delete.");


        rolesToDelete.Add(role);
      }

      _db.Roles.RemoveRange(rolesToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {roleIds.Count} role(s)." }); 

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while deleting role(s) data");
      return StatusCode(500, "Internal server error");
    }
  }
}
