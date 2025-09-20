using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.AccountDTOs;

public class AccountToCreate : AccountBase {
  [Required]
  public new string Password {
    get => base.Password;
    set => base.Password = value;
  }

  [Required]
  public new string State {
    get => base.State;
    set => base.State = value;
  }

  [Required]
  public new int? RoleId {
    get => base.RoleId;
    set => base.RoleId = value;
  }
}
