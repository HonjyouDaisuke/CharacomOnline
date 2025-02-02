using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

[Table("stroke_master")]
public class StrokeMaster : BaseModel
{
  [Column("chara_name")]
  public string? CharaName { get; set; }

  [Column("file_id")]
  public string? FileId { get; set; }
}
