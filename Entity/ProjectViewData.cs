using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace CharacomOnline.Entity;

public class ProjectViewData : BaseModel
{
  [Column("id")]
  [JsonPropertyName("id")]
  public Guid Id { get; set; }

  [Column("title")]
  [JsonPropertyName("title")]
  public string Name { get; set; }

  [Column("description")]
  [JsonPropertyName("description")]
  public string Description { get; set; }

  [Column("user_count")]
  [JsonPropertyName("user_count")]
  public int Users { get; set; }

  [Column("chara_count")]
  [JsonPropertyName("chara_count")]
  public int Charactors { get; set; }

  public bool IsSelected { get; set; } = false;

  // スタイルを動的に決定するメソッド
  public string GetCardStyle()
  {
    return $"background-color: {(IsSelected ? "lightcyan" : "white")}; width: 300px; padding: 15px; border-radius: 8px; margin: 10px;";
  }
}