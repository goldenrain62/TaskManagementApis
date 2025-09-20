using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.EmployeeDTOs;

public class EmployeeToCreate : EmployeeBase {
  [Required]
  public new string Firstname {
    get => base.Firstname;
    set => base.Firstname = value;
  }

  [Required]
  public new DateTime? Dob {
    get => base.Dob;
    set => base.Dob = value;
  }

  [Required]
  public new bool? Gender {
    get => base.Gender;
    set => base.Gender = value;
  }

  [Required]
  public new string Phone {
    get => base.Phone;
    set => base.Phone = value;
  }

  [Required]
  public new string Email {
    get => base.Email;
    set => base.Email = value;
  }

  [Required]
  public new int? TeamId {
    get => base.TeamId;
    set => base.TeamId = value;
  }
}


