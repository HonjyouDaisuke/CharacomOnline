using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

[Table("user_projects")] // テーブル名
public class UserProjectsTable : BaseModel
{
  [Column("user_id")]
  public Guid UserId { get; set; }

  [Column("project_id")]
  public Guid ProjectId { get; set; }

  [Column("role")]
  public Guid RoleId { get; set; }
}
