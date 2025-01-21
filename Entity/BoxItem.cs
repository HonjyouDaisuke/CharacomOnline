using Newtonsoft.Json;

namespace CharacomOnline.Entity;

public class BoxItem
{
  [JsonProperty("id")]
  public string Id { get; set; }

  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("type")]
  public string Type { get; set; } // "file" or "folder"
}

public class BoxFolderResponse
{
  [JsonProperty("entries")]
  public List<BoxItem> Entries { get; set; }
}

public class BoxFolderDetails
{
  [JsonProperty("item_collection")]
  public ItemCollection ItemCollections { get; set; }

  public class ItemCollection
  {
    [JsonProperty("total_count")]
    public int TotalCount { get; set; }
  }
}