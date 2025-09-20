using Microsoft.AspNetCore.Mvc;
using TaskManagementApis.Models;
using TaskManagementApis.Data;
using TaskManagementApis.Data.DTOs.EmployeeDTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApis.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EmployeesController : CustomBaseController<EmployeesController> {

  public EmployeesController(AppDbContext db, ILogger<EmployeesController> logger) 
    : base(db, logger) {}

  [HttpGet]
  public async Task<IActionResult> GetEmployees() {
    try {
      var employees = await _db.Employees
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

      return Ok(new {
        success = true,
        message = "Successfully get all employees.",
        result = employees 
      });

    } catch (Exception ex) {
      _logger.LogError(ex, "An error occured while getting employees.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetEmployee(int id) {
    try {
      var employee = await _db.Employees
        .Where(e => e.Id == id)
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
          UpdatedAt = e.UpdatedAt,
          UpdatedBy = e.UpdatedById
      }).FirstOrDefaultAsync();

      if (employee == null) return NotFound("Not found ID");

      return Ok(new {
        success = true,
        message = $"Successfully get the employee with ID: {id}.",
        result = employee 
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting the employee with ID: {id}.");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpGet("{id}/subordinates")]
  public async Task<IActionResult> GetSubordinates(int id) {
    try {
      var subordinates = await _db.Employees
        .Where(e => e.ManagerId == id)
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
          UpdatedAt = e.UpdatedAt,
          UpdatedBy = e.UpdatedById
      }).ToListAsync();

      if (subordinates == null) 
        return NotFound("Not found ID or this employee is not a team leader or an assistant leader.");

      return Ok(new {
        success = true,
        message = $"Successfully get subordinates of the employee with ID: {id}.",
        result = subordinates 
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while getting subordinates of the employee with ID: {id}.");
      return StatusCode(500, "Internal server error");
    }
  }


  [HttpPost]
  [Authorize(Policy = "CanCreateEmployee(s)")]
  public async Task<IActionResult> CreateEmployees([FromBody] List<EmployeeToCreate> employeesToCreate) {
    try {
      if (employeesToCreate.Count == 0) return BadRequest("No employee to create.");

      // Check if there are any required attributes that are null
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (!AreValidData(employeesToCreate, HttpContext.RequestServices))
        return ValidationErrorResponse();

      int maxId = await _db.Employees.AnyAsync() ?
        await _db.Employees.MaxAsync(e => e.Id) : 0;

      foreach (var employee in employeesToCreate) {
        var newEmployee = new Employee {
          Firstname = employee.Firstname,
          Lastname = employee.Lastname,
          Dob = employee.Dob,
          Gender = employee.Gender ?? throw new Exception("Gender is missing"),
          Phone = employee.Phone,
          Email = employee.Email,
          TeamId = employee.TeamId ?? throw new Exception("TeamId is missing"),
          ManagerId = employee.ManagerId
        };

        _db.Employees.Add(newEmployee);
      }

      await _db.SaveChangesAsync();

      // Send response
      var newEmployees = await _db.Employees
        .Where(e => e.Id > maxId)
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
          UpdatedAt = e.UpdatedAt,
          UpdatedBy = e.UpdatedById
      }).ToListAsync();

      return Ok(new { success = true, message = "Successfully created new employee(s).", result = newEmployees });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while creating employee(s).");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpPut]
  public async Task<IActionResult> UpdateEmployees([FromBody] List<EmployeeToUpdate> employeesToUpdate) {
    try {
      if (employeesToUpdate.Count == 0) 
        return BadRequest("No employee to update");

      if (!AreValidData(employeesToUpdate, HttpContext.RequestServices))
        return ValidationErrorResponse();


      List<int> employeeIds = new List<int>();
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

      foreach (var employeeToUpdate in employeesToUpdate) {
        var employee = await _db.Employees
          .Where(e => e.Id == employeeToUpdate.Id)
          .FirstOrDefaultAsync();

        // Check Authorization
        if (!string.Equals("System Admin", userRole) &&
            !string.Equals("HR", userRole) &&
            employee.Id != userId)
          return BadRequest("Only System Admin, HR and account's owner have the permission to update employee(s).");


        // Update Employee attributes
        if (string.Equals("System Admin", userRole) || employee.Id == userId) {
          employee.Firstname = employeeToUpdate.Firstname ?? employee.Firstname;
          employee.Lastname = employeeToUpdate.Lastname ?? employee.Lastname;
          employee.Dob = employeeToUpdate.Dob ?? employee.Dob;
          employee.Gender = employeeToUpdate.Gender ?? employee.Gender;
          employee.Phone = employeeToUpdate.Phone ?? employee.Phone;
          employee.Email = employeeToUpdate.Email ?? employee.Email;
        }

        if (string.Equals("System Admin", userRole) || string.Equals("HR", userRole)) {
          employee.TeamId = employeeToUpdate.TeamId ?? employee.TeamId;
          employee.ManagerId = employeeToUpdate.ManagerId ?? employee.ManagerId;
        }

        employee.UpdatedById = userId;
        employee.UpdatedAt = DateTime.Now;


        // Add employee.Id to employeeIds for getting updated employees
        employeeIds.Add(employee.Id);
      }

      await _db.SaveChangesAsync();

      // Send response
      var updatedEmployees = await _db.Employees
        .Where(e => employeeIds.Contains(e.Id))
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
          UpdatedAt = e.UpdatedAt,
          UpdatedBy = e.UpdatedById
        }).ToListAsync();

      return Ok(new {
          success = true,
          message = $"Successfully updated all {updatedEmployees.Count()} employee(s). " +
          "If there are any attributes that are not updated, you must be that employee or need a higher authority to update them.",
          result = updatedEmployees
      });

    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while updating employee(s).");
      return StatusCode(500, "Internal server error");
    }
  }

  [HttpDelete]
  [Authorize(Policy = "CanDeleteEmployee(s)")]
  public async Task<IActionResult> DeleteEmployees([FromBody] List<int> employeeIds) {
    try {
      var employeesToDelete = new List<Employee>();

      foreach (var employeeId in employeeIds) {
        if (employeeId == 1 || employeeId == -1)
          return BadRequest($"Employee with Id {employeeId} cannot be deleted under no circumstances.");

        var employee = await _db.Employees
          .Where(e => e.Id == employeeId)
          .FirstOrDefaultAsync();

        if (employee == null) return NotFound($"Not found employee with Id {employeeId} to delete.");


        employeesToDelete.Add(employee);
      }

      _db.Employees.RemoveRange(employeesToDelete);
      await _db.SaveChangesAsync();


      return Ok(new { success = true, message = $"Successfully deleted all {employeeIds.Count} employee(s)." }); 
 
    } catch (Exception ex) {
      _logger.LogError(ex, $"An error occured while deleting employee(s).");
      return StatusCode(500, "Internal server error");
    }
  }
}


