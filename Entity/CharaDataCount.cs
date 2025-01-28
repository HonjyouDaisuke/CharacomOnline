using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

public class CharaDataCount : BaseModel
{
  [Column("chara_name")]
  [JsonPropertyName("chara_name")]
  public string CharaName { get; set; }

  [Column("material_name")]
  [JsonPropertyName("material_name")]
  public string MaterialName { get; set; }

  [Column("selected_count")]
  [JsonPropertyName("selected_count")]
  public int SelectedCount { get; set; }

  [Column("chara_count")]
  [JsonPropertyName("chara_count")]
  public int CharaCount { get; set; }
}
