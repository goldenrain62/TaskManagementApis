using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.CommentDTOs;

public class CommentToCreate : CommentBase {
  [Required]
  public new string Content {
    get => base.Content;
    set => base.Content = value;
  }

  [Required]
  public new int? TaskId {
    get => base.TaskId;
    set => base.TaskId = value;
  }
}
