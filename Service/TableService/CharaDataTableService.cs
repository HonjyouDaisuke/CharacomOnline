using System.Text.Json;
using System.Text.Json.Serialization;
using CharacomOnline.Entity;
using CharacomOnline.ImageProcessing;
using CharacomOnline.Repositories;
using Supabase;

namespace CharacomOnline.Service.TableService;

public class CharaDataRecode
{
  public Guid? CharaDataId { get; set; } // "chara_data_id"
  public string? MaterialName { get; set; } // "material_name"
  public string? CharaName { get; set; } // "chara_name"
  public string? Times { get; set; } // "times"
  public string? SizeString { get; set; } // "size_string"
  public string? FileName { get; set; } // "file_name"
  public string? FilePath { get; set; } // "file_path"
  public string? Thumbnail { get; set; } // "thumbnail"
}

public class CharaDataTableService(
  Client supabaseClient,
  StorageService storageService,
  SelectingItemsRepository selectingItemsRepository,
  BoxFileService boxFileService,
  CharaDataRepository charaDataRepository
)
{
  private readonly Client _supabaseClient = supabaseClient;
  private readonly StorageService _storageService = storageService;
  private readonly SelectingItemsRepository _selectingItemsRepository = selectingItemsRepository;
  private readonly BoxFileService _boxFileService = boxFileService;
  private readonly CharaDataRepository _charaDataRepository = charaDataRepository;

  public List<CharaDataRecode>? DeserializeJson(string json)
  {
    if (json == null)
      return null;
    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true, // JSONのプロパティ名の大小文字を無視
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    return JsonSerializer.Deserialize<List<CharaDataRecode>>(json, options);
  }

  public async Task<List<CharaDataClass>?> FetchCharaDataFromProject(Guid projectId, Guid modelId)
  {
    var response = await _supabaseClient.Rpc(
      "get_chara_data_from_project_id",
      new { project_id_input = projectId, model_id_input = modelId }
    );
    List<CharaDataClass>? resultCharaData = new List<CharaDataClass>();

    if (string.IsNullOrEmpty(response.Content))
      return null;
    try
    {
      var content = DeserializeJson(response.Content);
      if (content == null)
        return null;

      int total = content.Count;

      var tasks = content.Select(
        async (item, index) =>
        {
          if (item.FilePath == null)
            return null;
          if (item.FileName == null)
            return null;
          CharaDataClass cdc = new CharaDataClass
          {
            CharaName = item.CharaName,
            MaterialName = item.MaterialName,
            TimesName = item.Times,
            Thumbnail = item.Thumbnail,
            SrcImage = await _storageService.DownloadFileAsBitmapAsync(
              item.FilePath,
              item.FileName
            ),
          };

          return cdc;
        }
      );

      // 全タスクを実行して結果を収集
      CharaDataClass[] charaDataClasses = (await Task.WhenAll(tasks));
      resultCharaData = charaDataClasses.ToList();
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
      new { p_project_id = projectId, p_file_id = fileId } // パラメーターを渡す
    );

    return response;
  }

  public async Task<Guid?> CreateCharaData(Guid projectId, string fileId, FileInformation fileInfo)
  {
    if (await IsCharaDataExists(projectId, fileId))
      return null;
    Guid newGuid = Guid.NewGuid();

    var newCharaData = new CharaDataTable
    {
      Id = newGuid,
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
        Console.WriteLine(
          $"Project '{inData.ProjectId}' added successfully with Chara: {inData.CharaName}"
        );
        return inData.Id; // ID を返す
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"CharaDataInsertError : {ex.Message}");
    }

    return null;
  }

  public class CharaName
  {
    [JsonPropertyName("chara_name")]
    public string? Name { get; set; }
  }

  public class MaterialName
  {
    [JsonPropertyName("material_name")]
    public string? Name { set; get; }
  }

  public async Task InitCharactersDataAsync(Guid projectId)
  {
    // Supabase から RPC を呼び出す
    Console.WriteLine($"start fetch ProjectId:{projectId}");
    var response = await _supabaseClient.Rpc(
      "get_unique_chara_names",
      new { p_project_id = projectId }
    );
    Console.WriteLine(response.Content);

    // レスポンスの Content は string 型なので、JSON としてデシリアライズ
    if (!string.IsNullOrEmpty(response.Content))
    {
      try
      {
        // JSON を List<string> に変換
        var charas = JsonSerializer.Deserialize<List<CharaName>>(response.Content);
        if (charas == null)
          return;

        _selectingItemsRepository.ClearCharacters();
        foreach (var item in charas)
        {
          if (item.Name == null)
            continue;
          _selectingItemsRepository.AddCharacters(item.Name);
        }
      }
      catch (JsonException ex)
      {
        // デシリアライズエラー処理
        Console.WriteLine("JSON Parsing Error: " + ex.Message);
      }
    }
    else
    {
      // エラーハンドリング（必要に応じて）
      Console.WriteLine("Error: No content returned");
    }
  }

  public async Task InitMaterialsData(Guid projectId)
  {
    // Supabase から RPC を呼び出す
    var response = await _supabaseClient.Rpc(
      "get_unique_material_names",
      new { p_project_id = projectId }
    );
    Console.WriteLine(response.Content);
    // レスポンスの Content は string 型なので、JSON としてデシリアライズ
    if (!string.IsNullOrEmpty(response.Content))
    {
      try
      {
        // JSON を List<string> に変換
        var materials = JsonSerializer.Deserialize<List<MaterialName>>(response.Content);

        if (materials == null)
          return;

        _selectingItemsRepository.ClearMaterials();
        foreach (var item in materials)
        {
          if (item.Name == null)
            continue;
          _selectingItemsRepository.AddMaterials(item.Name);
        }
      }
      catch (JsonException ex)
      {
        // デシリアライズエラー処理
        Console.WriteLine("JSON Parsing Error: " + ex.Message);
      }
    }
    else
    {
      // エラーハンドリング（必要に応じて）
      Console.WriteLine("Error: No content returned");
    }
  }

  public async Task GetCharaListAsync(
    Guid projectId,
    string charaName,
    string materialName,
    string accessToken,
    CancellationToken token,
    Action<int>? onProgress = null
  )
  {
    // Supabase から RPC を呼び出す
    Console.WriteLine($"start fetch ProjectId:{projectId}");
    var response = await _supabaseClient.Rpc(
      "get_chara_data_by_project_and_name",
      new
      {
        input_project_id = projectId,
        input_chara_name = charaName,
        input_material_name = materialName,
      }
    );

    Console.WriteLine(response.Content);

    if (!string.IsNullOrEmpty(response.Content))
    {
      try
      {
        // JSON を List に変換
        var charas = JsonSerializer.Deserialize<List<CharaImageListClass>>(response.Content);
        if (charas == null)
          return;

        await _charaDataRepository.ClearViewCharaDataAsync();

        int totalTasks = charas.Count;
        int progress = 0;

        Console.WriteLine($"個数は: {totalTasks}");

        Console.WriteLine("Foreach スタート---->");
        foreach (var item in charas)
        {
          if (item.Id == null || item.FileId == null)
            continue;

          token.ThrowIfCancellationRequested();
          Console.WriteLine($"FileId = {item.FileId} を始めます");

          // 存在確認をデータベースから行い、確認後に挿入処理
          var exists = await _charaDataRepository.IsViewCharaDataExistsAsync((Guid)item.Id);
          if (exists)
          {
            Console.WriteLine($"データは既に存在しています: {item.Id}");
            continue;
          }

          Console.WriteLine($"FileId = {item.FileId} を保存します");
          // 新しいデータを挿入する処理
          await ProcessCharaDataAsync(item, charaName, materialName, accessToken);
          Console.WriteLine($"FileId = {item.FileId} を保存終わり");

          progress++;
          onProgress?.Invoke((progress * 100) / totalTasks);
        }
        Console.WriteLine("<---- Foreach 終わり");
      }
      catch (JsonException ex)
      {
        // デシリアライズエラー処理
        Console.WriteLine("JSON Parsing Error: " + ex.Message);
      }
    }
    else
    {
      // レスポンスエラー処理
      Console.WriteLine("Error: No content returned");
    }
  }

  private async Task ProcessCharaDataAsync(
    CharaImageListClass item,
    string charaName,
    string materialName,
    string accessToken
  )
  {
    if (item.Id == null)
      return;
    if (item.FileId == null)
      return;

    // 必要なデータを準備
    CharaDataClass newItem =
      new()
      {
        Id = (Guid)item.Id,
        FileId = item.FileId,
        CharaName = charaName,
        MaterialName = materialName,
        IsSelected = item.IsSelected,
        SrcImage = await _boxFileService.DownloadFileAsSKBitmapAsync(item.FileId, accessToken),
      };

    if (newItem.SrcImage == null)
      return;
    Console.WriteLine($"fileId:{item.FileId} isSelected:{item.IsSelected}");

    // サムネイルと細線画像を生成
    newItem.Thumbnail = ImageEffectService.GetBinaryImageData(
      ImageEffectService.ResizeBitmap(newItem.SrcImage, 50, 50)
    );
    var resizeBmp = ImageEffectService.ResizeBitmap(newItem.SrcImage, 160, 160);
    var binary = ImageEffectService.GetBinaryBitmap(resizeBmp);
    var thinning = new ThinningProcess(binary);
    newItem.ThinImage = await thinning.ThinBinaryImageAsync();

    // データが存在するか再度確認
    bool exists = await _charaDataRepository.IsViewCharaDataExistsAsync(newItem.Id);
    if (exists)
    {
      Console.WriteLine($"データは既に存在します: {newItem.Id}");
      return;
    }

    // データをリポジトリに追加
    await _charaDataRepository.AddViewCharaDataAsync(newItem);

    Console.WriteLine($"データを挿入しました: {newItem.Id}");
  }

  public async Task UpdateCharaSelectChangeAsync(Guid charaId, bool isSelected, Guid userId)
  {
    try
    {
      Console.WriteLine($"これからデータを埋め込みます。userId={userId} selected={isSelected}");
      var checkExistence = await _supabaseClient
        .From<CharaDataClass>()
        .Where(x => x.Id == charaId)
        .Single();

      if (checkExistence == null)
      {
        Console.WriteLine($"該当するデータが見つかりませんでした: charaId={charaId}");
        return;
      }

      var response = await _supabaseClient
        .From<CharaDataClass>() // 更新するテーブル
        .Where(x => x.Id == charaId) // 条件を LINQ の形で指定
        .Set(x => x.IsSelected, isSelected) // IsSelected を更新
        .Set(x => x.UpdatedBy, userId) // UpdatedBy を更新
        .Set(x => x.UpdatedAt, DateTime.UtcNow) // UpdatedAt を更新
        .Update();

      Console.WriteLine($"Response: {response.ToString()}");
      if (response.Models.Count > 0)
      {
        foreach (var model in response.Models)
        {
          Console.WriteLine($"Updated record: {model.Id}");
        }
      }
      else
      {
        Console.WriteLine("更新されたデータがありません。");
      }
      // レスポンスを確認
      if (response != null && response.Models.Count > 0)
      {
        Console.WriteLine($"checkを更新しました:{charaId}");
      }
      else
      {
        Console.WriteLine($"更新に失敗しました。{charaId}");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }
}
