using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.AuthDTOs;

public class Login {
  [Required]
  public int Id { get; set; }

  [Required]
  public string Password { get; set; }
}
