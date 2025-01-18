using CharacomOnline.Entity;
using Supabase;
using Supabase.Postgrest.Exceptions;

namespace CharacomOnline.Service.TableService;

public class StandardTableService(Client supabaseClient)
{
  private readonly Client _supabaseClient = supabaseClient;

  public async Task AddRecodeAsync(string charaName, string fileId)
  {
    var newItem = new StandardMaster
    {
      CharaName = charaName,
      FileId = fileId,
    };
    try
    {
      var response = await _supabaseClient.From<StandardMaster>().Insert(newItem);
      if (response == null)
      {
        Console.WriteLine($"Insert Error StandardData... CharaName={charaName}");
      }
    }
    catch (PostgrestException ex) when (ex.Message.Contains("unique_standard_chara_file"))
    {
      Console.WriteLine($"Duplicate record detected. Skipping insert.");
    }
  }

  public async Task DeleteAllAsync()
  {
    try
    {
      // SQLで全データ削除を実行
      await _supabaseClient.From<StandardMaster>().Delete();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"削除中にエラーが発生しました: {ex.Message}");
      throw;
    }
  }

  /// <summary>
  /// 標準書体をすべて取得します。
  /// </summary>
  /// <returns>List<StandardTable></returns>
  public async Task<List<StandardMaster>> GetAllAsync()
  {
    try
    {
      // Supabaseからデータを取得
      var response = await _supabaseClient.From<StandardMaster>().Select("chara_name, file_id").Get();
      Console.WriteLine($"(Standard){response.ToString()}");

      // エラーチェック
      if (response == null)
      {
        Console.WriteLine($"エラーが発生しました");
        return new List<StandardMaster>();  // エラーがあれば空のリストを返す
      }

      // 取得したデータを返す
      return response.Models.ToList(); // Modelsからリストに変換して返す
    }
    catch (Exception ex)
    {
      Console.WriteLine($"例外が発生しました: {ex.Message}");
      return new List<StandardMaster>();  // 例外が発生した場合も空のリストを返す
    }
  }

  public async Task<string?> GetFileIdAsync(string charaName)
  {
    try
    {
      var standardResponse = await _supabaseClient
            .From<StandardMaster>()
            .Select("file_id")
            .Filter("chara_name", Supabase.Postgrest.Constants.Operator.Equals, charaName)
            .Get();

      return standardResponse.Models.FirstOrDefault()?.FileId;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"例外が発生しました(standard master): {ex.Message}");
      return null;
    }
  }
}

