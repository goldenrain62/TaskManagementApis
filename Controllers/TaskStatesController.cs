using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.TaskStateDTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TaskStatesController : CustomBaseController<TaskStatesController> {
  public TaskStatesController(AppDbContext db, ILogger<TaskStatesController> logger)
    : base(db, logger) {}

  [HttpGet]
  public async Task<IActionResult> GetTaskStates() {
    try {
      var taskStates = await _db.TaskStates
        .Select(ts => new {
          Id = ts.Id,
          Name = ts.Name,
          Description = ts.Description,
          CreatedByTeam = ts.CreatedByTeamId,
          CreatedBy = ts.CreatedById,
          UpdatedBy = ts.UpdatedById,
          CreatedAt = ts.CreatedAt,
          UpdatedAt = ts.UpdatedAt
      }).ToListAsync();

      return Ok(new { success = true, message = "Successfully got all task state(s).", result = taskStates });
    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting taskstate(s).");
      return StatusCode(500, "Internal Server Error.");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetTaskState(int id) {
    try {
      var taskState = await _db.TaskStates
        .Where(ts => ts.Id == id)
        .Select(ts => new {
          Id = ts.Id,
          Name = ts.Name,
          Description = ts.Description,
          CreatedByTeam = ts.CreatedByTeamId,
          CreatedBy = ts.CreatedById,
          UpdatedBy = ts.UpdatedById,
          CreatedAt = ts.CreatedAt,
          UpdatedAt = ts.UpdatedAt
      }).FirstOrDefaultAsync();

      if (taskState == null) return BadRequest("Not found Id");

      return Ok(new {
        success = true,
        message = $"Successfully got task state with Id {id}.",
        result = taskState
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting taskstate(s).");
      return StatusCode(500, "Internal Server Error.");
    }
  }

  [HttpPost]
  [Authorize(Policy = "CanCreateTaskState(s)")]
  public async Task<IActionResult> CreateTaskStates([FromBody] List<TaskStateToCreate> taskStatesToCreate) {
    try {
      if (taskStatesToCreate.Count == 0) return BadRequest("No task state to create.");

      // Check if there are any required attributes that are null
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (!AreValidData(taskStatesToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      int maxId = await _db.TaskStates.AnyAsync() ?
        await _db.TaskStates.MaxAsync(ts => ts.Id) : 0;
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

      foreach (var taskStateToCreate in taskStatesToCreate) {
        var newTaskState = new TaskState {
          Name = taskStateToCreate.Name,
          Description = taskStateToCreate.Description,
          CreatedByTeamId = await GetTeamId(userId),
          CreatedById = userId
        };

        _db.TaskStates.Add(newTaskState);
      }

      await _db.SaveChangesAsync();

      var newTaskStates = await _db.TaskStates
        .Where(ts => ts.Id > maxId)
        .Select(ts => new {
          Id = ts.Id,
          Name = ts.Name,
          Description = ts.Description,
          CreatedByTeam = ts.CreatedByTeamId,
          CreatedBy = ts.CreatedById,
          UpdatedBy = ts.UpdatedById,
          CreatedAt = ts.CreatedAt,
          UpdatedAt = ts.UpdatedAt
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = $"Successfully created {taskStatesToCreate.Count} task state(s).",
        result = newTaskStates
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while creating task state(s).");
      return StatusCode(500, "Internal Server Error.");
    }
  }

  [HttpPut]
  [Authorize(Policy = "CanUpdateTaskState(s)")]
  public async Task<IActionResult> UpdateTaskStates([FromBody] List<TaskStateToUpdate> taskStatesToUpdate) {
    try {
      if (taskStatesToUpdate.Count == 0)
        return BadRequest("No task state to update.");

      if (!AreValidData(taskStatesToUpdate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      List<int> taskStateIds = new List<int>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

      foreach (var taskStateToUpdate in taskStatesToUpdate) {
        var taskState = await _db.TaskStates
          .Where(ts => ts.Id == taskStateToUpdate.Id)
          .FirstOrDefaultAsync();

        if (!string.Equals("System Admin", userRole) &&
            !string.Equals("Project Manager", userRole) &&
            taskState.CreatedByTeamId != await GetTeamId(userId))
          return BadRequest($"Only System Admin, Project Manager and leader/assistant leaders of team with Id {taskState.CreatedByTeamId} " +
              $"can update task state with Id {taskState.Id}.");

        // Update TaskState attribute
        taskState.Name = taskStateToUpdate.Name ?? taskState.Name;
        taskState.Description = taskStateToUpdate.Description ?? taskState.Description;
        taskState.UpdatedById = userId;
        taskState.UpdatedAt = DateTime.Now;

        // Add category.Id to categoryIds for getting updated categories
        taskStateIds.Add(taskState.Id);
      }

      await _db.SaveChangesAsync();

      // Send response
      var updatedTaskStates = await _db.TaskStates
        .Where(ts => taskStateIds.Contains(ts.Id))
        .Select(ts => new {
          Id = ts.Id,
          Name = ts.Name,
          Description = ts.Description,
          CreatedByTeam = ts.CreatedByTeamId,
          CreatedBy = ts.CreatedById,
          UpdatedBy = ts.UpdatedById,
          CreatedAt = ts.CreatedAt,
          UpdatedAt = ts.UpdatedAt
      }).ToListAsync();

      return Ok(new {
          success = true,
          message = $"Successfully updated all {updatedTaskStates.Count()} task state(s).",
          result = updatedTaskStates
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while updating task state(s).");
      return StatusCode(500, "Internal Server Error.");
    }
  }

  [HttpDelete]
  [Authorize(Policy = "CanDeleteTaskState(s)")]
  public async Task<IActionResult> DeleteTaskStates(List<int> taskStateIds) {
    try {
      var taskStatesToDelete = new List<TaskState>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

      foreach (var taskStateId in taskStateIds) {
        var taskState = await _db.TaskStates
          .Where(ts => ts.Id == taskStateId)
          .FirstOrDefaultAsync();

        if (taskState == null) return NotFound($"Not found task state with Id {taskStateId} to delete.");

        if (!string.Equals("System Admin", userRole) &&
            !string.Equals("Project Manager", userRole) &&
            taskState.CreatedByTeamId != await GetTeamId(userId))
          return BadRequest($"Only System Admin, Project Manager and leader/assistant leaders of team with Id {taskState.CreatedByTeamId} " +
              $"can delete taskState with Id {taskState.Id}.");

        taskStatesToDelete.Add(taskState);
      }

      _db.TaskStates.RemoveRange(taskStatesToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {taskStateIds.Count} task state(s)." }); 


    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while deleting task state(s).");
      return StatusCode(500, "Internal Server Error.");
    }
  }
}
