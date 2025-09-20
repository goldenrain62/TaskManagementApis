using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.CommentDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CommentsController : CustomBaseController<CommentsController> {

  public CommentsController(AppDbContext db, ILogger<CommentsController> logger) 
    : base(db, logger) {}

  [HttpGet]
  public async Task<IActionResult> GetComments() {
    try {
      var comments = await _db.Comments
        .Select(c => new {
          Id = c.Id,
          Content = c.Content,
          TaskId = c.TaskId,
          RepliesTo = c.RepliesTo,
          CommentedBy = c.CommentedById,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt
      }).ToListAsync();

      return Ok(new { success = true, message = "Successfully got all comments.", results = comments });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting all comments.");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetComment(int id) {
    try {
      var comment = await _db.Comments
        .Where(c => c.Id == id)
        .Select(c => new {
          Id = c.Id,
          Content = c.Content,
          TaskId = c.TaskId,
          RepliesTo = c.RepliesTo,
          CommentedBy = c.CommentedById,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt
      }).FirstOrDefaultAsync();

      if (comment == null) return BadRequest("Not found Id");

      return Ok(new { success = true, message = $"Successfully got the comment with ID {id}.", results = comment });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting the comment with ID {id}.");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpPost]
  public async Task<IActionResult> CreateComments([FromBody] List<CommentToCreate> commentsToCreate) {
    try {
      if (commentsToCreate.Count == 0) return BadRequest("No comment to create.");

      // Check if there are any required attributes that are null
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (!AreValidData(commentsToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      int maxId = await _db.Comments.AnyAsync() ?
        await _db.Comments.MaxAsync(c => c.Id) : 0;
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

      foreach (var comment in commentsToCreate) {
        var newComment = new Comment {
          Content = comment.Content,
          TaskId = comment.TaskId ?? throw new Exception("TaskId is missing"),
          RepliesTo = comment.RepliesTo,
          CommentedById = userId
        };

        _db.Comments.Add(newComment);
      }

      await _db.SaveChangesAsync();

      // Send response
      var newComments = await _db.Comments
        .Where(c => c.Id > maxId)
        .Select(c => new {
          Id = c.Id,
          Content = c.Content,
          TaskId = c.TaskId,
          RepliesTo = c.RepliesTo,
          CommentedBy = c.CommentedById,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = "Successfully created new comment(s).", 
        result = newComments
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while creating comment(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpPut]
  public async Task<IActionResult> UpdateComments([FromBody] List<CommentToUpdate> commentsToUpdate) {
    try {
      if (commentsToUpdate.Count == 0)
        return BadRequest("No comment to update.");

      if (!AreValidData(commentsToUpdate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      List<int> commentIDs = new List<int>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

      foreach (var commentToUpdate in commentsToUpdate) {
        var comment = await _db.Comments
          .Where(c => c.Id == commentToUpdate.Id)
          .FirstOrDefaultAsync();

        if (comment.CommentedById != userId)
          return BadRequest("Only this comment's creator can update it.");


        // Update Comment attributes
        comment.Content = commentToUpdate.Content ?? comment.Content;
        comment.TaskId = commentToUpdate.TaskId ?? comment.TaskId;
        comment.RepliesTo = commentToUpdate.RepliesTo ?? comment.RepliesTo;
        comment.UpdatedAt = DateTime.Now;


        // Add comment.Id to commentIDs for getting updated comments
        commentIDs.Add(comment.Id);
      }

      await _db.SaveChangesAsync();

      // Send response
      var updatedComments = await _db.Comments
        .Where(c => commentIDs.Contains(c.Id))
        .Select(c => new {
          Id = c.Id,
          Content = c.Content,
          TaskId = c.TaskId,
          RepliesTo = c.RepliesTo,
          CommentedBy = c.CommentedById,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt
      }).ToListAsync();

      return Ok(new {
          success = true,
          message = $"Successfully updated all {commentsToUpdate.Count} comment(s).",
          result = updatedComments
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while updating comment(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpDelete]
  public async Task<IActionResult> DeleteComments([FromBody] List<int> commentIDs) {
    try {
      var commentsToDelete = new List<Comment>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;


      foreach (var commentID in commentIDs) {
        var comment = await _db.Comments
          .Where(c => c.Id == commentID)
          .FirstOrDefaultAsync();

        if (comment == null) return NotFound($"Not found comment with Id {commentID} to delete.");

        if (!string.Equals("System Admin", userRole) &&
            comment.CommentedById != userId)
          return BadRequest($"Only System Admin and this comment's creator can delete it.");

        commentsToDelete.Add(comment);
      }

      _db.Comments.RemoveRange(commentsToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {commentIDs.Count} comment(s)." }); 


    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while deleting comment(s).");
      return StatusCode(500, "Internal server error.");
    }
  }
}
