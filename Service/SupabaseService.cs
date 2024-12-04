using Microsoft.AspNetCore.Components.Forms;
using SkiaSharp;
using Supabase;

namespace CharacomOnline.Service;

public class SupabaseService
{
  private readonly Client _supabaseClient;
  private readonly AppSettings _appSettings;

  public SupabaseService(AppSettings appSettings)
  {
    // SupabaseのURLとanonキーを使ってクライアントを初期化
    _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

    // SupabaseのURLとanonキーを使ってクライアントを初期化
    var supabaseUrl = _appSettings.SUPABASE_URL;
    var anonKey = _appSettings.ANON_KEY;

    var options = new SupabaseOptions
    {
      AutoConnectRealtime = false,
    };

    _supabaseClient = new Client(supabaseUrl, anonKey, options);
  }

  public string SupabaseUrl => _appSettings.SUPABASE_URL;
  public string AnonKey => _appSettings.ANON_KEY;

  // 公開バケットにファイルをアップロードするメソッド
  public async Task UploadFileAsync(IBrowserFile selectedFile, string uploadFileName)
  {
    // ストリームを開く (最大サイズを 10MB に設定)
    using var stream = selectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);

    using var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream);

    var fileBytes = memoryStream.ToArray();

    var filePath = $"Images/{uploadFileName}";
    var storage = _supabaseClient.Storage;
    var bucket = storage.From("Characom");

    try
    {
      await bucket.Upload(fileBytes, filePath);
      Console.WriteLine("アップロード成功");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"エラー: {ex.Message}");
    }
  }

  // SKBitmapでファイルをダウンロードするメソッド
  public async Task<SKBitmap?> DownloadFileAsBitmapAsync(string fileName)
  {
    var filePath = $"Images/{fileName}";
    var storage = _supabaseClient.Storage;
    var bucket = storage.From("Characom");

    try
    {
      var fileBytes = await bucket.Download(filePath, null); // ファイルをバイト配列として取得
      Console.WriteLine("ダウンロード成功: " + filePath);

      // バイト配列からSKBitmapを生成
      using var stream = new SKMemoryStream(fileBytes);
      return SKBitmap.Decode(stream);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"エラー: {ex.Message}");
      return null; // エラー時はnullを返す
    }
  }
}
