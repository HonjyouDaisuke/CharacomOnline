using Supabase;

namespace CharacomOnline.Service;

public class SupabaseService
{
  private readonly Client _supabaseClient;
  private string _supabaseUrl;
  private string _anonKey;

  public SupabaseService()
  {
    // SupabaseのURLとanonキーを使ってクライアントを初期化
    _supabaseUrl = "https://imrymolanolzitkwcnhz.supabase.co";
    _anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imltcnltb2xhbm9seml0a3djbmh6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mjk3NDc4ODIsImV4cCI6MjA0NTMyMzg4Mn0.JQkLH8CdtHpjIQsU3DKMEscFKdiHmoA0NaQurk4ReM8"; // anonキー

    var options = new SupabaseOptions
    {
      AutoConnectRealtime = false,
    };

    _supabaseClient = new Client(_supabaseUrl, _anonKey, options);
  }

  public string SupabaseUrl { get => _supabaseUrl; set => _supabaseUrl = value; }
  public string AnonKey { get => _anonKey; set => _anonKey = value; }

  // 公開バケットにファイルをアップロードするメソッド
  public async Task UploadFileAsync(byte[] fileBytes, string fileName)
  {
    // 'Images'フォルダにファイルをアップロード
    var filePath = $"Images/{fileName}";
    var storage = _supabaseClient.Storage;
    var bucket = storage.From("Characom");  // 公開バケット名を指定
    try
    {
      await bucket.Upload(fileBytes, filePath); // ファイルアップロード
      Console.WriteLine("アップロード成功");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"エラー: {ex.Message}");
    }
  }
}
