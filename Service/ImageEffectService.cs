using Microsoft.AspNetCore.Components.Forms;
using SkiaSharp;

namespace CharacomOnline.Service;

public static class ImageEffectService
{
  public static string? GetBinaryImageData(SKBitmap skBitmap)
  {
    // 2値化画像をPNG形式にエンコードしてBase64文字列に変換
    using var image = SKImage.FromBitmap(skBitmap);
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    var base64 = Convert.ToBase64String(data.ToArray());
    return $"data:image/png;base64,{base64}";
  }

  public static async Task<SKBitmap?> LoadImageFileAsync(IBrowserFile file)
  {
    try
    {
      using var memoryStream = new MemoryStream();

      // 非同期でファイルをメモリにコピー
      await file.OpenReadStream().CopyToAsync(memoryStream);
      memoryStream.Seek(0, SeekOrigin.Begin); // ストリームの先頭に移動

      // メモリストリームからSkiaSharpで画像をデコード
      using var skStream = new SKManagedStream(memoryStream);
      var bitmap = SKBitmap.Decode(skStream);

      if (bitmap == null || bitmap.Width == 0 || bitmap.Height == 0)
      {
        return null;
      }

      return bitmap;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Loading Image Error... : {ex.Message}");
      return null;
    }
  }

  public static Task<SKBitmap?> GetBinaryBitmap(SKBitmap srcBitmap)
  {
    int threshold = GetThreshold(srcBitmap);
    var binaryBitmap = new SKBitmap(srcBitmap.Width, srcBitmap.Height);
    for (int y = 0; y < srcBitmap.Height; y++)
    {
      for (int x = 0; x < srcBitmap.Width; x++)
      {
        var color = srcBitmap.GetPixel(x, y);
        //// 輝度を計算して閾値を設定（128を例に使用）
        var brightness = (color.Red + color.Green + color.Blue) / 3;
        var binaryColor = brightness > threshold ? SKColors.White : SKColors.Black;
        binaryBitmap.SetPixel(x, y, binaryColor);
      }
    }

    return Task.FromResult<SKBitmap?>(binaryBitmap);
  }

  public static SKBitmap ResizeBitmap(SKBitmap skBitmap, int width, int height)
  {
    var resizedBitmap = skBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
    return resizedBitmap;
  }

  private static int GetThreshold(SKBitmap bitmap)
  {
    int[] histogram = new int[256];
    int totalPixels = bitmap.Width * bitmap.Height;

    for (int y = 0; y < bitmap.Height; y++)
    {
      for (int x = 0; x < bitmap.Width; x++)
      {
        histogram[
          (bitmap.GetPixel(x, y).Red + bitmap.GetPixel(x, y).Green + bitmap.GetPixel(x, y).Blue) / 3
        ]++;
      }
    }

    int sum = 0,
      sumB = 0,
      wB = 0,
      wF = 0;
    for (int i = 0; i < 256; i++)
    {
      sum += i * histogram[i];
    }

    float maxVariance = 0;
    int threshold = 0;

    for (int t = 0; t < 256; t++)
    {
      wB += histogram[t];
      if (wB == 0)
      {
        continue;
      }

      wF = totalPixels - wB;

      if (wF == 0)
      {
        break;
      }

      sumB += t * histogram[t];
      float mB = (float)sumB / wB;
      float mF = (float)(sum - sumB) / wF;
      float variance = wB * wF * (mB - mF) * (mB - mF);

      if (variance > maxVariance)
      {
        maxVariance = variance;
        threshold = t;
      }
    }

    return threshold;
  }
}
