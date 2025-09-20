using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.TaskDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TasksController : CustomBaseController<TasksController> {

  public TasksController(AppDbContext db, ILogger<TasksController> logger) 
    : base(db, logger) {}


  [HttpGet]
  public async Task<IActionResult> GetTasks() {
    try {
      var tasks = await _db.Tasks
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          Progress = t.Progress,
          Priority = t.Priority,
          DueDate = t.Due,
          Rate = t.Rate,
          Note = t.Note,
          Category = t.CategoryId,
          ExecutedByTeam = t.ExecutedByTeamId,
          ExecutedByEmployee = t.ExecutedByEmployeeId,
          ParentTask = t.ParentTaskId,
          CurrentState = t.CurrentStateId,
          CreatedBy = t.CreatedById,
          UpdatedBy = t.UpdatedById,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
      }).ToListAsync();

      return Ok(new { success = true, message = "Successfully got all tasks.", result = tasks });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting tasks.");
      return StatusCode(500, "Interval server error.");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetTask(int id) {
    try {
      var task = await _db.Tasks
        .Where(t => t.Id == id)
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          Progress = t.Progress,
          Priority = t.Priority,
          DueDate = t.Due,
          Rate = t.Rate,
          Note = t.Note,
          Category = t.CategoryId,
          ExecutedByTeam = t.ExecutedByTeamId,
          ExecutedByEmployee = t.ExecutedByEmployeeId,
          ParentTask = t.ParentTaskId,
          CurrentState = t.CurrentStateId,
          CreatedBy = t.CreatedById,
          UpdatedBy = t.UpdatedById,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
      }).FirstOrDefaultAsync();

      if (task == null) return BadRequest("Not found Id.");

      return Ok(new { success = true, message = $"Successfully got task with ID {id}.", result = task });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting task with ID {id}.");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpGet("{id}/comments")]
  public async Task<IActionResult> GetTaskComments(int id) {
    try {
      var comments = await _db.Comments
        .Where(c => c.TaskId == id)
        .Select(c => new {
          Id = c.Id,
          Task = c.TaskId,
          RepliesTo = c.RepliesTo,
          CommentedBy = c.CommentedBy,
          Content = c.Content,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt,
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = $"Successfully got all comment(s) of the task with ID {id}.",
        results = comments
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting this task's comment(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpGet("{id}/subtasks")]
  public async Task<IActionResult> GetTaskSubs(int id) {
    try {
      var subtasks = await _db.Tasks
        .Where(t => t.ParentTaskId == id)
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          Progress = t.Progress,
          Priority = t.Priority,
          DueDate = t.Due,
          Rate = t.Rate,
          Note = t.Note,
          Category = t.CategoryId,
          ExecutedByTeam = t.ExecutedByTeamId,
          ExecutedByEmployee = t.ExecutedByEmployeeId,
          ParentTask = t.ParentTaskId,
          CurrentState = t.CurrentStateId,
          CreatedBy = t.CreatedById,
          UpdatedBy = t.UpdatedById,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = $"Successfully got all subtask(s) of the task with ID {id}.",
        results = subtasks
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting this task's subtask(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpPost]
  [Authorize(Policy = "CanCreateTask(s)")]
  public async Task<IActionResult> CreateTasks([FromBody] List<TaskToCreate> tasksToCreate) {
    try {
      if (tasksToCreate.Count == 0) return BadRequest("No task to create.");

      // Check if there are any required attributes that are null
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (!AreValidData(tasksToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      int maxId = await _db.Tasks.AnyAsync() ?
        await _db.Tasks.MaxAsync(t => t.Id) : 0;
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userTeam = await GetTeamId(userId);

      foreach (var task in tasksToCreate) {
        var newTask = new TASK {
          Name = task.Name,
          Description = task.Description,
          Progress = task.Progress ?? throw new Exception("Progress is missing"),
          Priority = task.Priority,
          Due = task.Due ?? throw new Exception("Due is missing"),
          Rate = task.Rate,
          Note = task.Note,
          CategoryId = task.CategoryId ?? throw new Exception("CategoryId is missing"),
          ExecutedByTeamId = task.ExecutedByTeamId ?? throw new Exception("ExecutedByTeamId is missing"),
          ExecutedByEmployeeId = task.ExecutedByEmployeeId ?? throw new Exception("ExecutedByEmployeeId is missing"),
          ParentTaskId = task.ParentTaskId,
          CurrentStateId = task.CurrentStateId ?? throw new Exception("CurrentStateId is missing"),
          CreatedByTeamId = userTeam,
          CreatedById = userId,
        };

        _db.Tasks.Add(newTask);
      }

      await _db.SaveChangesAsync();

      // Send response
      var newTasks = await _db.Tasks
        .Where(t => t.Id > maxId)
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          Progress = t.Progress,
          Priority = t.Priority,
          DueDate = t.Due,
          Rate = t.Rate,
          Note = t.Note,
          Category = t.CategoryId,
          ExecutedByTeam = t.ExecutedByTeamId,
          ExecutedByEmployee = t.ExecutedByEmployeeId,
          ParentTask = t.ParentTaskId,
          CurrentState = t.CurrentStateId,
          CreatedBy = t.CreatedById,
          UpdatedBy = t.UpdatedById,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = "Successfully created new task(s).", 
        result = newTasks
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while creating task(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpPut]
  [Authorize(Policy = "CanUpdateTask(s)")]
  public async Task<IActionResult> UpdateTasks([FromBody] List<TaskToUpdate> tasksToUpdate) {
    try {
      if (tasksToUpdate.Count == 0)
        return BadRequest("No task to update.");

      if (!AreValidData(tasksToUpdate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      List<int> taskIDs = new List<int>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
      var userTeam = await GetTeamId(userId);

      foreach (var taskToUpdate in tasksToUpdate) {
        var task = await _db.Tasks
          .Where(t => t.Id == taskToUpdate.Id)
          .FirstOrDefaultAsync();

        if (!string.Equals("System Admin", userRole) &&
            !string.Equals("Project Manager", userRole) &&
            task.CreatedByTeamId != userTeam)
          return BadRequest($"Only System Admin, Project Manager and leader/assistant leaders of team with Id {task.CreatedByTeamId} " +
              $"can update task with Id {task.Id}.");


        // Update Task attributes
        task.Name = taskToUpdate.Name ?? task.Name;
        task.Description = taskToUpdate.Description ?? task.Description;
        task.Progress = taskToUpdate.Progress ?? task.Progress;
        task.Priority = taskToUpdate.Priority ?? task.Priority;
        task.Due = taskToUpdate.Due ?? task.Due;
        task.Rate = taskToUpdate.Rate ?? task.Rate;
        task.Note = taskToUpdate.Note ?? task.Note;
        task.CategoryId = taskToUpdate.CategoryId ?? task.CategoryId;
        task.ExecutedByTeamId = taskToUpdate.ExecutedByTeamId ?? task.ExecutedByTeamId;
        task.ExecutedByEmployeeId = taskToUpdate.ExecutedByEmployeeId ?? task.ExecutedByEmployeeId;
        task.ParentTaskId = taskToUpdate.ParentTaskId ?? task.ParentTaskId;
        task.CurrentStateId = taskToUpdate.CurrentStateId ?? task.CurrentStateId;
        task.UpdatedById = int.Parse(Request.Cookies["uid"]);
        task.UpdatedAt = DateTime.Now;

        // Add task to taskIDs for getting updated task(s)
        taskIDs.Add(task.Id);
      }

      await _db.SaveChangesAsync();

      // Send response
      var updatedTasks = await _db.Tasks
        .Where(t => taskIDs.Contains(t.Id))
        .Select(t => new {
          Id = t.Id,
          Name = t.Name,
          Description = t.Description,
          Progress = t.Progress,
          Priority = t.Priority,
          DueDate = t.Due,
          Rate = t.Rate,
          Note = t.Note,
          Category = t.CategoryId,
          ExecutedByTeam = t.ExecutedByTeamId,
          ExecutedByEmployee = t.ExecutedByEmployeeId,
          ParentTask = t.ParentTaskId,
          CurrentState = t.CurrentStateId,
          CreatedBy = t.CreatedById,
          UpdatedBy = t.UpdatedById,
          CreatedAt = t.CreatedAt,
          UpdatedAt = t.UpdatedAt
      }).ToListAsync();

      return Ok(new {
          success = true,
          message = $"Successfully updated all {tasksToUpdate.Count} task(s).",
          result = updatedTasks
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while updating task(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpDelete]
  [Authorize(Policy = "CanDeleteTask(s)")]
  public async Task<IActionResult> DeleteTasks([FromBody] List<int> taskIDs) {
    try {
      var tasksToDelete = new List<TASK>();

      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

      foreach (var taskID in taskIDs) {
        var task = await _db.Tasks
          .Where(t => t.Id == taskID)
          .FirstOrDefaultAsync();

        if (task == null) return NotFound($"Not found task with Id {taskID} to delete.");

        if (!string.Equals("System Admin", userRole) &&
            !string.Equals("Project Manager", userRole))
          return BadRequest($"Only System Admin and Project Managers can delete tasks.");

        if (task.CurrentStateId != 16 && task.CurrentStateId != 17)
          return BadRequest("Tasks can only be deleted when their states are 'Archived' or 'Canceled'.");

        if (string.Equals("Project Manager", userRole) && task.CurrentStateId == 16)
          return BadRequest("Project Managers can delete tasks which states are 'Canceled'.");


        tasksToDelete.Add(task);
      }

      _db.Tasks.RemoveRange(tasksToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {taskIDs.Count} task(s)." }); 
 
    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while deleting task(s).");
      return StatusCode(500, "Interval server error.");
    }
  }
}
