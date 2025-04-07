using ApexCharts;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

public static class NotificationType
{
  public const string System = "System";
  public const string Message = "Message";
  public const string Warning = "Warning";
  public const string Info = "Info";
  public const string Error = "Error";
}

[Table("notifications")] // テーブル名
public class Notification : BaseModel
{
  [Column("user_id")]
  public Guid UserId { get; set; }

  [Column("title")]
  public string Title { get; set; } = "";

  [Column("message")]
  public string Message { get; set; } = "";

  [Column("type")]
  public string Type { get; set; } = "";

  [Column("is_read")]
  public bool IsRead { get; set; } = false;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.Now;

  [Column("updated_at")]
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

  [Column("read_at")]
  public DateTime? ReadAt { get; set; }
}
