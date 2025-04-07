using SkiaSharp;

namespace CharacomOnline.Entity;

public class OverlayDataClass
{
  public string CharaName { get; set; } = "";
  public string MaterialName { get; set; } = "";
  public string Color { get; set; } = "";
  public SKBitmap OverlayBmp { get; set; } = new();
  public int SelectedItemCount { get; set; } = 0;
  public bool IsSelected { get; set; }

  public OverlayDataClass()
  {
    CharaName = "";
    MaterialName = "";
    Color = "";
    OverlayBmp = new SKBitmap();
    SelectedItemCount = 0;
    IsSelected = false;
  }

  public OverlayDataClass(
    string charaName,
    string materialName,
    string color,
    SKBitmap overlayBmp,
    int selectedItemCount,
    bool isSelected
  )
  {
    CharaName = charaName;
    MaterialName = materialName;
    Color = color;
    OverlayBmp = overlayBmp;
    SelectedItemCount = selectedItemCount;
    IsSelected = isSelected;
  }

  public OverlayDataClass(OverlayDataClass other)
    : this(
      other.CharaName,
      other.MaterialName,
      other.Color,
      other.OverlayBmp,
      other.SelectedItemCount,
      other.IsSelected
    ) { }
}
