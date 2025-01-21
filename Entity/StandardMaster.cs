using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

// PostgreSQL側のテーブル名を正確に指定
[Table("standard_master")]
public class StandardMaster : BaseModel
{
  [Column("chara_name")]
  public string? CharaName { get; set; }

  [Column("file_id")]
  public string? FileId { get; set; }
}

