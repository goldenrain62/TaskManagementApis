using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.TaskStateDTOs;

public class TaskStateToCreate : TaskStateBase {
  [Required]
  public new string Name {
    get => base.Name;
    set => base.Name = value;
  }

  [Required]
  public new string Description {
    get => base.Description;
    set => base.Description = value;
  }
}
