using SkiaSharp;

namespace CharacomOnline.Entity;

public class ImageProcessDataClass
{
  public SKBitmap? SrcBitmap { get; set; }
  public SKBitmap? ThinningBitmap { get; set; }
  public SKBitmap? HistogramBitmap { get; set; }
  public string? SrcImageData { get; set; }
  public string? ThinningImageData { get; set; }
  public string? HistogramImageData { get; set; }

}
