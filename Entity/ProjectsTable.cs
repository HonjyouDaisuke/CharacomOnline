namespace CharacomOnline.Entity;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("projects")] // テーブル名
public class ProjectsTable : BaseModel
{
  [Column("id")]
  public Guid Id { get; set; }

  [Column("title")]
  public string Title { get; set; }

  [Column("description")]
  public string Description { get; set; }
}
