using CharacomOnline.Entity;
using Supabase;
using System.Text.Json;

namespace CharacomOnline.Service.TableService;

public class GlobalSettingTableService(Client supabaseClient)
{
  private readonly Client _supabaseClient = supabaseClient;

  public async Task<GlobalSetting?> FetchGlobalSettings()
  {
    // `global_setting`テーブルからデータを取得
    var response = await _supabaseClient.Rpc("get_global_settings", "");
    var globalSetting = JsonSerializer.Deserialize<List<GlobalSetting>>(response.Content);
    Console.WriteLine("Fetch終了");
    return globalSetting.First<GlobalSetting>(); // 最初の行を返す

  }
}
