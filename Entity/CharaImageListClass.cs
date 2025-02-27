﻿using System.Text.Json.Serialization;

namespace CharacomOnline.Entity;

public class CharaImageListClass
{
  [JsonPropertyName("id")]
  public Guid? Id { set; get; }

  [JsonPropertyName("file_id")]
  public string? FileId { set; get; }

  [JsonPropertyName("is_selected")]
  public bool IsSelected { get; set; }

  public override string ToString()
  {
    return $"id: {Id}  FileId: {FileId}";
  }
}
