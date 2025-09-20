using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.AccountDTOs;
using TaskManagementApis.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AccountsController : CustomBaseController<AccountsController> {
  private readonly HasherService _hasher;

  public AccountsController(AppDbContext db, ILogger<AccountsController> logger, HasherService hasher)
    : base(db, logger) {

    _hasher = hasher;
  }

  [HttpGet]
  public async Task<IActionResult> GetAccounts() {
    try {
      var accounts = await _db.Accounts
        .Select(a => new {
          Id = a.Id, 
          Username = a.Username,
          State = a.State,
          Role = a.RoleId,
          UpdatedBy = a.UpdatedById,
          CreatedAt = a.CreatedAt,
          UpdatedAt = a.UpdatedAt
      }).ToListAsync();

      return Ok(new { success = true, message = "Successfully got all accounts.", results = accounts });
    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting accounts.");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpGet("by_username/{username}")]
  public async Task<IActionResult> GetAccountByUsername(string username) {
    try {
      var account = await _db.Accounts
        .Where(a => a.Username == username)
        .Select(a => new {
          Id = a.Id, 
          Username = a.Username,
          State = a.State,
          Role = a.RoleId,
          UpdatedBy = a.UpdatedById,
          CreatedAt = a.CreatedAt,
          UpdatedAt = a.UpdatedAt
      }).FirstOrDefaultAsync();

      if (account == null) return NotFound("Not found account. Please check Username.");

      return Ok(new {
        success = true,
        message = $"Successfully got the account with the Username {username}.",
        results = account
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting account with Username {username}.");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpGet("by_id/{id}")]
  public async Task<IActionResult> GetAccountById(int id) {
    try {
      var account = await _db.Accounts
        .Where(a => a.Id == id)
        .Select(a => new {
          Id = a.Id,
          Username = a.Username,
          State = a.State,
          Role = a.RoleId,
          UpdatedBy = a.UpdatedById,
          CreatedAt = a.CreatedAt,
          UpdatedAt = a.UpdatedAt
      }).FirstOrDefaultAsync();

      if (account == null) return NotFound("Not found account(s). Please check Id.");

      return Ok(new {
        success = true,
        message = $"Successfully got account with Id {id}.",
        results = account
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting accounts.");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpPost]
  [Authorize(Policy = "CanCreateAccount(s)")]
  public async Task<IActionResult> CreateAccounts([FromBody] List<AccountToCreate> accountsToCreate) {
    try {
      if (accountsToCreate.Count == 0) return BadRequest("No account to create.");

      // Check if there is any required attribute that is null
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (!AreValidData(accountsToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      List<int> accountIds = new List<int>();
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
 

      foreach (var accountToCreate in accountsToCreate) {
        if (accountToCreate.RoleId == null)
          throw new Exception("RoleId is missing");
        else if (accountToCreate.RoleId == 1 && !string.Equals("System Admin", userRole))
          return BadRequest("Only System Admin can create accounts with RoleId 1");

        var newAccount = new Account {
          Id = accountToCreate.Id,
          Username = accountToCreate.Username,
          Password = _hasher.ComputeMD5Hash(accountToCreate.Password),
          State = accountToCreate.State,
          RoleId = accountToCreate.RoleId ?? throw new Exception("Missing RoleId") 
        };


        accountIds.Add(newAccount.Id);

        _db.Accounts.Add(newAccount);
      }

      await _db.SaveChangesAsync();

      // Send response
      var newAccounts = await _db.Accounts
        .Where(a => accountIds.Contains(a.Id))
        .Select(a => new {
          Id = a.Id,
          Username = a.Username,
          Password = a.Password,
          State = a.State,
          Role = a.RoleId,
          UpdatedBy = a.UpdatedById,
          CreatedAt = a.CreatedAt,
          UpdatedAt = a.UpdatedAt
      }).ToListAsync();

      return Ok(new {
        success = true,
        message = "Successfully created new account(s).",
        result = newAccounts
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while creating account(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpPut]
  public async Task<IActionResult> UpdateAccounts([FromBody] List<AccountBase> accountsToUpdate) {
    try {
      if (accountsToUpdate.Count == 0)
        return BadRequest("No account to update.");

      if (!AreValidData(accountsToUpdate, HttpContext.RequestServices))
        return ValidationErrorResponse();


      List<int> accountIds = new List<int>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;


      // Update Account attributes
      foreach (var accountToUpdate in accountsToUpdate) {
        var account = await _db.Accounts
          .Where(a => a.Id == accountToUpdate.Id)
          .FirstOrDefaultAsync();

        // Check Authorization
        if (!string.Equals("System Admin", userRole) &&
            !string.Equals("HR", userRole) &&
            account.Id != userId)
          return BadRequest("Only System Admin, HR and this account's owner have the permission to update data.");


        // Update Account attributes
        if (string.Equals("System Admin", userRole) ||
            account.Id == userId) {
            account.Username = accountToUpdate.Username ?? account.Username;

            if (!string.IsNullOrWhiteSpace(accountToUpdate.Password))
              account.Password = _hasher.ComputeMD5Hash(accountToUpdate.Password);
        }

        if (string.Equals("System Admin", userRole) ||
            string.Equals("HR", userRole)) {
          account.State = accountToUpdate.State ?? account.State;

          if (string.Equals("HR", userRole) && accountToUpdate.RoleId == 1)
            return BadRequest("Only System Admin can grant a System Admin account.");

          account.RoleId = accountToUpdate.RoleId ?? account.RoleId;
        }

        account.UpdatedById = userId;
        account.UpdatedAt = DateTime.Now;

        // Add account.Id to accountIds for getting updated accounts
        accountIds.Add(account.Id);
      }

      await _db.SaveChangesAsync();

      // Send response
      var updatedAccounts = await _db.Accounts
        .Where(a => accountIds.Contains(a.Id))
        .Select(a => new {
          Id = a.Id, 
          Username = a.Username,
          State = a.State,
          Role = a.RoleId,
          UpdatedBy = a.UpdatedById,
          CreatedAt = a.CreatedAt,
          UpdatedAt = a.UpdatedAt
        }).ToListAsync();

      return Ok(new {
        success = true,
        message = $"Successfully updated all {accountsToUpdate.Count} account(s). " +
        "If there are any attributes that are not updated, you must be the account's owner or need an appropriate authority to update them.",
        result = updatedAccounts
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while updating account(s).");
      return StatusCode(500, "Internal server error.");
    }
  }

  [HttpDelete]
  [Authorize(Policy = "CanDeleteAccount(s)")]
  public async Task<IActionResult> DeleteAccounts([FromBody] List<int> userIds) {
    try {
      var accountsToDelete = new List<Account>();

      foreach (var userId in userIds) {
        var account = await _db.Accounts
          .Where(a => a.Id == userId)
          .FirstOrDefaultAsync();

        if (account == null) return NotFound($"Not found account with Id {userId} to delete.");


        accountsToDelete.Add(account);
      }

      _db.Accounts.RemoveRange(accountsToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {userIds.Count} account(s)." }); 
   
    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while deleting account(s).");
      return StatusCode(500, "Internal server error.");
    }
  }
}


