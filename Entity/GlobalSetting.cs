using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace CharacomOnline.Entity;

[Table("global_setting")]
public class GlobalSetting : BaseModel
{
  [Column("id")]
  [JsonPropertyName("id")]
  public Guid Id { get; set; }

  [Column("top_folder_id")]
  [JsonPropertyName("top_folder_id")]
  public string? TopFolderId { get; set; }

  [Column("standard_folder_id")]
  [JsonPropertyName("standard_folder_id")]
  public string? StandardFolderId { get; set; }

  [Column("stroke_folder_id")]
  [JsonPropertyName("stroke_folder_id")]
  public string? StrokeFolderId { get; set; }
}
