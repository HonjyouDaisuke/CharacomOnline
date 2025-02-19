namespace CharacomOnline.Entity;

using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("projects")] // テーブル名
public class ProjectsTable : BaseModel
{
  [Column("id")]
  public Guid Id { get; set; }

  [Column("title")]
  public string Title { get; set; } = "";

  [Column("description")]
  public string Description { get; set; } = "";

  [Column("folder_id")]
  public string FolderId { get; set; } = "";

  [Column("chara_folder_id")]
  public string CharaFolderId { get; set; } = "";

  [Column("created_by")]
  public Guid CreatedBy { get; set; }

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("updated_by")]
  public Guid UpdatedBy { get; set; }

  [Column("updated_at")]
  public DateTime UpdatedAt { get; set; }
}
