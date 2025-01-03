using CharacomOnline.Entity;
using Supabase;
using Supabase.Postgrest.Exceptions;

namespace CharacomOnline.Service.TableService;

public class StrokeTableService(Client supabaseClient)
{
  private readonly Client _supabaseClient = supabaseClient;

  public async Task AddRecodeAsync(string charaName, string fileId)
  {
    var newItem = new StrokeMaster
    {
      CharaName = charaName,
      FileId = fileId,
    };

    try
    {
      var response = await _supabaseClient.From<StrokeMaster>().Insert(newItem);
      if (response == null)
      {
        Console.WriteLine($"Insert Error StandardData... CharaName={charaName}");
      }
    }
    catch (PostgrestException ex) when (ex.Message.Contains("unique_stroke_chara_file"))
    {
      Console.WriteLine($"Duplicate record detected. Skipping insert.");
    }
  }

  public async Task DeleteAllAsync()
  {
    try
    {
      // SQLで全データ削除を実行
      await _supabaseClient.From<StrokeMaster>().Delete();
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
  public async Task<List<StrokeMaster>> GetAllAsync()
  {
    try
    {
      // Supabaseからデータを取得
      var response = await _supabaseClient.From<StrokeMaster>().Select("chara_name, file_id").Get();

      // エラーチェック
      if (response == null)
      {
        Console.WriteLine($"エラーが発生しました");
        return new List<StrokeMaster>();  // エラーがあれば空のリストを返す
      }

      // 取得したデータを返す
      return response.Models.ToList(); // Modelsからリストに変換して返す
    }
    catch (Exception ex)
    {
      Console.WriteLine($"例外が発生しました: {ex.Message}");
      return new List<StrokeMaster>();  // 例外が発生した場合も空のリストを返す
    }
  }
}
