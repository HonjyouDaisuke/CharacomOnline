namespace CharacomOnline.Entity;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

[Table("projects")] // テーブル名
public class ProjectsTable : BaseModel
{
  [Column("id")]
  public Guid Id { get; set; }

  [Column("title")]
  public string Title { get; set; }

  [Column("description")]
  public string Description { get; set; }

  [Column("folder_id")]
  public string FolderId { get; set; }

  [Column("chara_folder_id")]
  public string CharaFolderId { get; set; }
}
