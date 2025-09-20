using System.ComponentModel.DataAnnotations;

namespace TaskManagementApis.Data.DTOs.CategoryDTOs;

public class CategoryToCreate : CategoryBase {
  [Required]
  public new string Name {
    get => base.Name;
    set => base.Name = value;
  }
}
