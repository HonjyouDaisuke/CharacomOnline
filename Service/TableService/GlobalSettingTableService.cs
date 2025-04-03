using System.Text.Json;
using CharacomOnline.Entity;
using Supabase;

namespace CharacomOnline.Service.TableService;

public class GlobalSettingTableService(Client supabaseClient)
{
  private readonly Client _supabaseClient = supabaseClient;

  public async Task<GlobalSetting?> FetchGlobalSettings()
  {
    // `global_setting`テーブルからデータを取得
    var response = await _supabaseClient.Rpc("get_global_settings", "");
    if (response == null || response.Content == null)
      return null;
    var globalSetting = JsonSerializer.Deserialize<List<GlobalSetting>>(response.Content);
    Console.WriteLine("Fetch終了");
    if (globalSetting == null)
      return null;
    return globalSetting.First<GlobalSetting>(); // 最初の行を返す
  }
}
