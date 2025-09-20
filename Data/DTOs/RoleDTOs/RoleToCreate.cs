using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.RoleDTOs;

public class RoleToCreate : RoleBase {
  [Required]
  public new string RoleName {
    get => base.RoleName;
    set => base.RoleName = value;
  }

  [Required]
  public new string Description {
    get => base.Description;
    set => base.Description = value;
  }
}

