using SkiaSharp;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

public class OverlayDataClass
{
  public string CharaName { get; set; }
  public string MaterialName { get; set; }
  public string Color { get; set; }
  public SKBitmap OverlayBmp { get; set; }
  public int SelectedItemCount { get; set; }
  public bool IsSelected { get; set; }
}
