using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApis.Models;

public class Comment {
  // Primary key
  [Key]
  public int Id { get; set; }

  public string Content { get; set; }

  // Foreign keys
  [ForeignKey("Task")]
  public int TaskId { get; set; }

  [ForeignKey("ParentComment")]
  public int? RepliesTo { get; set; }

  [ForeignKey("CommentedBy")]
  public int CommentedById { get; set; }

  // Timestamps
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation properties
  public TASK Task { get; set; }
  public Comment ParentComment { get; set; }
  public Account CommentedBy { get; set; }

  public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}


