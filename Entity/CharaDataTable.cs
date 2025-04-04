﻿using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

[Table("chara_data")] // テーブル名
public class CharaDataTable : BaseModel
{
  [Column("id")]
  public Guid? Id { get; set; }

  [Column("project_id")]
  public Guid? ProjectId { get; set; }

  [Column("file_id")]
  public string? FileId { get; set; }

  [Column("chara_name")]
  public string? CharaName { get; set; }

  [Column("material_name")]
  public string? MaterialName { get; set; }

  [Column("is_selected")]
  public bool IsSelected { get; set; } = false;

  [Column("times_name")]
  public string? TimesName { get; set; }

  [Column("updated_at")]
  public DateTime UpdatedAt { get; set; }

  [Column("updated_by")]
  public Guid? UpdatedBy { get; set; }

}
