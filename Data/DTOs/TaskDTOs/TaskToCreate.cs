using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.TaskDTOs;

public class TaskToCreate : TaskBase {
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

  [Required]
  public new int? Progress {
    get => base.Progress;
    set => base.Progress = value;
  }

  [Required]
  public new DateTime? Due {
    get => base.Due;
    set => base.Due = value;
  }

  [Required]
  public new int? CategoryId {
    get => base.CategoryId;
    set => base.CategoryId = value;
  }

  [Required]
  public new int? ExecutedByTeamId {
    get => base.ExecutedByTeamId;
    set => base.ExecutedByTeamId = value;
  }

  [Required]
  public new int? ExecutedByEmployeeId {
    get => base.ExecutedByEmployeeId;
    set => base.ExecutedByEmployeeId = value;
  }

  [Required]
  public new int? CurrentStateId {
    get => base.CurrentStateId;
    set => base.CurrentStateId = value;
  }
}
