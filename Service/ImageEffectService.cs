using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Forms;
using SkiaSharp;

namespace CharacomOnline.Service;

public static class ImageEffectService
{
  public static SKColor ConvertRgbStringToSKColor(string rgbString)
  {
    var match = Regex.Match(rgbString, @"rgb\((\d+),\s*(\d+),\s*(\d+)\)");
    if (match.Success)
    {
      int r = int.Parse(match.Groups[1].Value);
      int g = int.Parse(match.Groups[2].Value);
      int b = int.Parse(match.Groups[3].Value);
      return new SKColor((byte)r, (byte)g, (byte)b);
    }

    throw new ArgumentException("Invalid RGB format", nameof(rgbString));
  }

  public static string? GetBinaryImageData(SKBitmap? skBitmap)
  {
    if (skBitmap == null)
      return null;

    try
    {
      // 画像をPNG形式にエンコードしてBase64文字列に変換
      using var image = SKImage.FromBitmap(skBitmap);
      if (image == null)
        return null;
      using var data = image.Encode(SKEncodedImageFormat.Png, 100);
      var base64 = Convert.ToBase64String(data.ToArray());
      return $"data:image/png;base64,{base64}";
    }
    catch (Exception ex)
    {
      Console.WriteLine($"image.Encodeのエラー:{ex.Message}");
      return null;
    }
  }

  /// <summary>
  ///  SKBitmapがすべて代かどうかを判定する
  /// </summary>
  /// <param name="skBitmap">チェックするビットマップ</param>
  /// <returns>true: 真っ白 false: 何か描かれている</returns>
  public static bool IsWhiteCanvas(SKBitmap skBitmap)
  {
    for (int i = 0; i < skBitmap.Height; i++)
    {
      for (int j = 0; j < skBitmap.Width; j++)
      {
        if (skBitmap.GetPixel(i, j) != SKColors.White)
        {
          return false;
        }
      }
    }

    return true;
  }

  public static async Task<SKBitmap?> LoadImageFileAsync(IBrowserFile file)
  {
    try
    {
      using var memoryStream = new MemoryStream();

      // 非同期でファイルをメモリにコピー
      await file.OpenReadStream().CopyToAsync(memoryStream);
      memoryStream.Seek(0, SeekOrigin.Begin); // ストリームの先頭に移動

      // メモリストリームからSkiarSharpで画像をデコード
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

  public static SKBitmap GetBinaryBitmap(SKBitmap srcBitmap)
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

    return binaryBitmap;
  }

  public static byte[,] ImageArrayFromBitmap(SKBitmap srcBitmap)
  {
    byte[,] result = new byte[srcBitmap.Height, srcBitmap.Width];

    for (int j = 0; j < srcBitmap.Height; j++)
    {
      for (int i = 0; i < srcBitmap.Width; i++)
      {
        var color = srcBitmap.GetPixel(i, j);
        result[j, i] = (color == SKColors.White) ? (byte)0 : (byte)1;
      }
    }

    return result;
  }

  public static SKBitmap? WhiteFilledBitmap(SKBitmap srcBitmap)
  {
    if (srcBitmap == null)
      return null;
    // 指定したサイズで新しいBitmapを作成
    SKBitmap bitmap = new(srcBitmap.Width, srcBitmap.Height);

    using (var canvas = new SKCanvas(bitmap))
    {
      // 白で塗りつぶす
      canvas.Clear(SKColors.White);
    }

    return bitmap; // 塗りつぶしたBitmapを返す
  }

  /// <summary>
  /// 2つの画像を重ねて、白を透明にする
  /// </summary>
  /// <param name="baseImage">背景画像</param>
  /// <param name="overlayImage">重ねる画像</param>
  /// <returns>合成後の SKBitmap</returns>
  static public SKBitmap OverlayImages(SKBitmap baseImage, SKBitmap overlayImage)
  {
    if (baseImage == null || overlayImage == null)
    {
      throw new ArgumentNullException("画像が null です。");
    }

    int width = Math.Max(baseImage.Width, overlayImage.Width);
    int height = Math.Max(baseImage.Height, overlayImage.Height);

    // 合成用のキャンバスを作成
    SKBitmap resultBitmap = new SKBitmap(width, height);
    using (SKCanvas canvas = new SKCanvas(resultBitmap))
    {
      // 透明な背景を作成
      canvas.Clear(SKColors.Transparent);

      // 白色を透明にした背景画像を描画
      using (SKPaint paint = new())
      {
        // paint.FilterQuality = SKFilterQuality.High;
        paint.IsAntialias = true;
        // 畳み込みフィルタのサイズとカーネルを設定
        SKSizeI kernelSize = new SKSizeI(3, 3); // 3x3 カーネル
        float[] kernelArray = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        ReadOnlySpan<float> kernel = kernelArray;
        float gain = 1.0f;
        float bias = 0.0f;
        SKPointI kernelOffset = new SKPointI(1, 1); // カーネルのオフセット
        SKShaderTileMode tileMode = SKShaderTileMode.Clamp; // タイルモード
        bool convolveAlpha = true; // アルファチャンネルを畳み込む

        // SKImageFilterを使って畳み込み処理を設定
        SKImageFilter filter = SKImageFilter.CreateMatrixConvolution(
          kernelSize,
          kernel,
          gain,
          bias,
          kernelOffset,
          tileMode,
          convolveAlpha
        );

        // 作成したフィルターをSKPaintに設定
        paint.ImageFilter = filter;
      }
      canvas.DrawBitmap(MakeWhiteTransparent(baseImage), 0, 0);

      // 重ねる画像を描画
      canvas.DrawBitmap(MakeWhiteTransparent(overlayImage), 0, 0);
    }

    return resultBitmap;
  }

  /// <summary>
  /// 白色を透明にする
  /// </summary>
  /// <param name="bitmap">元画像</param>
  /// <returns>白を透明化した SKBitmap</returns>
  static private SKBitmap MakeWhiteTransparent(SKBitmap bitmap)
  {
    SKBitmap newBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

    for (int y = 0; y < bitmap.Height; y++)
    {
      for (int x = 0; x < bitmap.Width; x++)
      {
        SKColor color = bitmap.GetPixel(x, y);
        if (color.Red > 200 && color.Green > 200 && color.Blue > 200)
        {
          // 白を透明にする
          newBitmap.SetPixel(x, y, new SKColor(0, 0, 0, 0));
        }
        else
        {
          newBitmap.SetPixel(x, y, color);
        }
      }
    }
    return newBitmap;
  }

  public static SKBitmap? OverlayBinaryImages(SKBitmap? backBitmap, SKBitmap? foreBitmap)
  {
    if (backBitmap == null)
      return null;
    if (foreBitmap == null)
      return null;
    if (backBitmap.Width != foreBitmap.Width || backBitmap.Height != foreBitmap.Height)
    {
      return null;
    }
    if (backBitmap.Width == 0 || backBitmap.Height == 0)
    {
      return null;
    }
    if (foreBitmap.Width == 0 || foreBitmap.Height == 0)
    {
      return null;
    }
    // 背景画像と同じサイズでアルファチャンネルを持つ新しいビットマップを作成
    SKBitmap resultBitmap =
      new(backBitmap.Width, backBitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

    using (var canvas = new SKCanvas(resultBitmap))
    {
      // 背景画像を描画
      canvas.DrawBitmap(backBitmap, 0, 0);

      // 前景画像のピクセルを走査して白色を透明にする
      for (int y = 0; y < foreBitmap.Height; y++)
      {
        for (int x = 0; x < foreBitmap.Width; x++)
        {
          var color = foreBitmap.GetPixel(x, y);
          if (color.Alpha != 255 || (color.Red == 255 && color.Green == 255 && color.Blue == 255))
          {
            continue; // 白色または完全に透明な場合はスキップ
          }

          resultBitmap.SetPixel(x, y, color);
        }
      }
    }

    return resultBitmap;
  }

  public static SKBitmap ColorChangeBitmap(SKBitmap srcBitmap, SKColor toColor)
  {
    SKBitmap resultBitmap = new(srcBitmap.Width, srcBitmap.Height);
    SKColor backColor = srcBitmap.GetPixel(1, 1);

    for (int y = 0; y < srcBitmap.Height; y++)
    {
      for (int x = 0; x < srcBitmap.Width; x++)
      {
        // 現在のピクセルの色を取得
        var pixelColor = srcBitmap.GetPixel(x, y);

        // 白以外であれば、toColorに変換、白であればそのまま
        if (pixelColor == backColor) // SKColors.White)
        {
          // Console.Write($"0");
          resultBitmap.SetPixel(x, y, SKColors.White);
        }
        else
        {
          // Console.Write($"1");
          resultBitmap.SetPixel(x, y, toColor);
        }
      }
      // Console.WriteLine("");
    }

    return resultBitmap;
  }

  public static SKBitmap ResizeBitmap(SKBitmap skBitmap, int width, int height)
  {
    // SKSamplingOptions を使用して線形補間でリサイズ
    var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None);

    // 新しいサイズでビットマップをリサイズ
    var resizedBitmap = skBitmap.Resize(new SKImageInfo(width, height), samplingOptions);

    // リサイズが失敗した場合、元のビットマップを返す
    return resizedBitmap ?? skBitmap;
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
      wB = 0;
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

      int wF = totalPixels - wB;
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

  public static SKBitmap? OutlineFrame(SKBitmap? bitmap)
  {
    if (bitmap == null)
      return null;
    var paint = new SKPaint();

    paint.Color = SKColors.Black;
    paint.StrokeWidth = 1;
    using (var canvas = new SKCanvas(bitmap))
    {
      canvas.DrawLine(0, 0, bitmap.Width - 1, 0, paint);
      canvas.DrawLine(0, bitmap.Height - 1, bitmap.Width - 1, bitmap.Height - 1, paint);
      canvas.DrawLine(0, 0, 0, bitmap.Height - 1, paint);
      canvas.DrawLine(bitmap.Width - 1, 0, bitmap.Width - 1, bitmap.Height - 1, paint);
    }
    return bitmap;
  }

  public static SKBitmap? GridLine(SKBitmap? bitmap)
  {
    if (bitmap == null)
      return null;
    var heightSize = bitmap.Height / 8;
    var widthSize = bitmap.Width / 8;
    var paint = new SKPaint();

    paint.Color = SKColors.DarkGray;
    paint.StrokeWidth = 1;
    using (var canvas = new SKCanvas(bitmap))
    {
      for (int i = 1; i < 8; i++)
      {
        canvas.DrawLine(0, i * heightSize, bitmap.Width, i * heightSize, paint);
        canvas.DrawLine(i * widthSize, 0, i * widthSize, bitmap.Height, paint);
      }
    }
    return bitmap;
  }

  public static SKBitmap? CenterLine(SKBitmap? bitmap)
  {
    if (bitmap == null)
      return null;
    var heightSize = bitmap.Height / 2;
    var widthSize = bitmap.Width / 2;
    var paint = new SKPaint();

    paint.Color = SKColors.Red;
    paint.StrokeWidth = 1;

    using (var canvas = new SKCanvas(bitmap))
    {
      canvas.DrawLine(0, heightSize, bitmap.Width, heightSize, paint);
      canvas.DrawLine(widthSize, 0, widthSize, bitmap.Height, paint);
    }
    return bitmap;
  }
}
