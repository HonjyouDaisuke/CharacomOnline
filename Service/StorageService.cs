using Microsoft.AspNetCore.Components.Forms;
using SkiaSharp;

namespace CharacomOnline.Service;

public class StorageService
{
  private SupabaseService _supabaseService;

  public StorageService(SupabaseService supabaseService)
  {
    _supabaseService = supabaseService;
  }

  // 公開バケットにファイルをアップロードするメソッド
  public async Task UploadFileAsync(IBrowserFile selectedFile, string uploadFileName)
  {
    var supabaseClient = await _supabaseService.GetClientAsync();

    // ストリームを開く (最大サイズを 10MB に設定)
    using var stream = selectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);

    using var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream);
    var fileBytes = memoryStream.ToArray();

    var filePath = $"Images/{uploadFileName}";
    var storage = supabaseClient.Storage;
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
    var supabaseClient = await _supabaseService.GetClientAsync();
    var filePath = $"Images/{fileName}";
    var storage = supabaseClient.Storage;
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
