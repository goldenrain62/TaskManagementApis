using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.CategoryDTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CategoriesController : CustomBaseController<CategoriesController> {

  public CategoriesController(AppDbContext db, ILogger<CategoriesController> logger)
    : base(db, logger) {}

  [HttpGet]
  public async Task<IActionResult> GetCategories() {
    try {
      var categories = await _db.Categories
        .Select(c => new {
          Id = c.Id,
          Name = c.Name,
          Description = c.Description,
          CreatedByTeam = c.CreatedByTeamId,
          CreatedBy = c.CreatedById,
          UpdatedBy = c.UpdatedById,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = "Successfully get all categories.",
        results = categories
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting categories.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetCategory(int id) {
    try {
      var category = await _db.Categories
        .Where(c => c.Id == id)
        .Select(c => new {
          Id = c.Id,
          Name = c.Name,
          Description = c.Description,
          CreatedByTeam = c.CreatedByTeamId,
          CreatedBy = c.CreatedById,
          UpdatedBy = c.UpdatedById,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt
      }).FirstOrDefaultAsync();

      if (category == null) return NotFound("Not found category ID.");

      return Ok(new {
        success = true,
        message = $"Successfully get the category with ID: {id}.",
        result = category 
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting the category with ID {id}.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPost]
  [Authorize(Policy = "CanCreateCategory/Categories")]
  public async Task<IActionResult> CreateCategories([FromBody] List<CategoryToCreate> categoriesToCreate) {
    try {
      if (categoriesToCreate.Count == 0) return BadRequest("No category to create.");

      // Check if there are any required attributes that are null
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (!AreValidData(categoriesToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      int maxId = await _db.Categories.AnyAsync() ?
        await _db.Categories.MaxAsync(c => c.Id) : 0;
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);


      foreach (var category in categoriesToCreate) {
        var newCategory = new Category {
          Name = category.Name,
          Description = category.Description,
          CreatedByTeamId = await GetTeamId(userId),
          CreatedById = userId
        };

        _db.Categories.Add(newCategory);
      }

      await _db.SaveChangesAsync();

      // Send response
      var newCategories = await _db.Categories
        .Where(c => c.Id > maxId)
        .Select(c => new {
          Id = c.Id,
          Name = c.Name,
          Description = c.Description,
          CreatedByTeam = c.CreatedByTeamId,
          CreatedBy = c.CreatedById,
          UpdatedBy = c.UpdatedById,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = "Successfully created new category/categories.", 
        result = newCategories
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while creating category/categories.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPut]
  [Authorize(Policy = "CanUpdateCategory/Categories")]
  public async Task<IActionResult> UpdateCategories([FromBody] List<CategoryToUpdate> categoriesToUpdate) {
    try {
      if (categoriesToUpdate.Count == 0)
        return BadRequest("No category to update");

      if (!AreValidData(categoriesToUpdate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      List<int> categoryIds = new List<int>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

      foreach (var categoryToUpdate in categoriesToUpdate) {
        var category = await _db.Categories
          .Where(c => c.Id == categoryToUpdate.Id)
          .FirstOrDefaultAsync();

        if (!string.Equals("System Admin", userRole) &&
            !string.Equals("Project Manager", userRole) &&
            category.CreatedByTeamId != await GetTeamId(userId))
          return BadRequest($"Only System Admin, Project Manager and leader/assistant leaders of team with Id {category.CreatedByTeamId} " +
              $"can update category with Id {category.Id}.");

        // Update Category attribute
        category.Name = categoryToUpdate.Name ?? category.Name;
        category.Description = categoryToUpdate.Description ?? category.Description;
        category.UpdatedById = userId;
        category.UpdatedAt = DateTime.Now;

        // Add category.Id to categoryIds for getting updated categories
        categoryIds.Add(category.Id);
      }

      await _db.SaveChangesAsync();

      // Send response
      var updatedCategories = await _db.Categories
        .Where(c => categoryIds.Contains(c.Id))
        .Select(c => new {
          Id = c.Id,
          Name = c.Name,
          Description = c.Description,
          CreatedByTeam = c.CreatedByTeamId,
          CreatedBy = c.CreatedById,
          UpdatedBy = c.UpdatedById,
          CreatedAt = c.CreatedAt,
          UpdatedAt = c.UpdatedAt
      }).ToListAsync();

      return Ok(new {
          success = true,
          message = $"Successfully updated all {updatedCategories.Count()} category/categories.",
          result = updatedCategories
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while updating category/categories.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpDelete]
  [Authorize(Policy = "CanDeleteCategory/Categories")]
  public async Task<IActionResult> DeleteCategories([FromBody] List<int> categoryIDs) {
    try {
      var categoriesToDelete = new List<Category>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

      foreach (var categoryID in categoryIDs) {
        var category = await _db.Categories
          .Where(c => c.Id == categoryID)
          .FirstOrDefaultAsync();

        if (category == null) return NotFound($"Not found category with Id {categoryID} to delete.");

        if (!string.Equals("System Admin", userRole) &&
            !string.Equals("Project Manager", userRole) &&
            category.CreatedByTeamId != await GetTeamId(userId))
          return BadRequest($"Only System Admin, Project Manager and leader/assistant leaders of team with Id {category.CreatedByTeamId} " +
              $"can delete category with Id {category.Id}.");

        categoriesToDelete.Add(category);
      }

      _db.Categories.RemoveRange(categoriesToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {categoryIDs.Count} category/categories." }); 

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while deleting category/categories.");
      return StatusCode(500, "Internal server error");
    }
  }
}
