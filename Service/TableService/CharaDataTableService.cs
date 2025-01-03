using CharacomOnline.Entity;
using Supabase;
using System.Text.Json;

namespace CharacomOnline.Service.TableService;

public class CharaDataRecode
{
  public Guid? CharaDataId { get; set; }  // "chara_data_id"
  public string? MaterialName { get; set; } // "material_name"
  public string? CharaName { get; set; }    // "chara_name"
  public string? Times { get; set; }        // "times"
  public string? SizeString { get; set; }   // "size_string"
  public string? FileName { get; set; }     // "file_name"
  public string? FilePath { get; set; }     // "file_path"
  public string? Thumbnail { get; set; }   // "thumbnail"
}

public class CharaDataTableService(Client supabaseClient, StorageService storageService)
{
  private readonly Client _supabaseClient = supabaseClient;
  private readonly StorageService _storageService = storageService;

  public List<CharaDataRecode>? DeserializeJson(string json)
  {
    if (json == null) return null;
    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true, // JSONのプロパティ名の大小文字を無視
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    return JsonSerializer.Deserialize<List<CharaDataRecode>>(json, options);
  }

  public async Task<List<CharaDataClass>?> FetchCharaDataFromProject(Guid projectId, Guid modelId)
  {
    var response = await _supabaseClient.Rpc("get_chara_data_from_project_id", new { project_id_input = projectId, model_id_input = modelId });
    List<CharaDataClass>? resultCharaData = new List<CharaDataClass>();

    Guid? charaDataId = null;
    if (string.IsNullOrEmpty(response.Content)) return null;
    try
    {
      var content = DeserializeJson(response.Content);
      if (content == null) return null;


      int total = content.Count;
      var tasks = content.Select(async (item, index) =>
      {
        CharaDataClass cdc = new CharaDataClass
        {
          Id = index + 1,
          CharaName = item.CharaName,
          MaterialName = item.MaterialName,
          TimesName = item.Times,
          Thumbnail = item.Thumbnail,
          SrcImage = await _storageService.DownloadFileAsBitmapAsync(item.FilePath, item.FileName)
        };

        return cdc;
      });

      // 全タスクを実行して結果を収集
      resultCharaData = (await Task.WhenAll(tasks)).ToList();
    }
    catch (JsonException ex)
    {
      Console.WriteLine("JSON Parsing Error: " + ex.Message);
      return null; // エラー時は空リストを返す
    }

    return resultCharaData;
  }

  public async Task<bool> IsCharaDataExists(Guid projectId, string fileId)
  {
    // RPC関数を呼び出す
    var response = await _supabaseClient.Rpc<bool>(
        "check_chara_data_exists",
        new { p_project_id = projectId, p_file_id = fileId }    // パラメーターを渡す
    );

    return response;
  }

  public async Task<Guid?> CreateCharaData(Guid projectId, string fileId, FileInformation fileInfo)
  {
    if (await IsCharaDataExists(projectId, fileId)) return null;
    Guid newUuid = Guid.NewGuid();

    var newCharaData = new CharaDataTable
    {
      Id = newUuid,
      ProjectId = projectId,
      FileId = fileId.ToString(),
      MaterialName = fileInfo.MaterialName,
      CharaName = fileInfo.CharaName,
      TimesName = fileInfo.TimesName,
    };
    try
    {
      // データベースに挿入
      var response = await _supabaseClient.From<CharaDataTable>().Insert(newCharaData);

      if (response != null)
      {
        var inData = response.Models.First();
        Console.WriteLine($"Project '{inData.ProjectId}' added successfully with Chara: {inData.CharaName}");
        return inData.Id; // ID を返す
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"CharaDataInsertError : {ex.Message}");
    }

    return null;
  }


}
