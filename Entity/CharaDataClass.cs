using SkiaSharp;

namespace CharacomOnline.Entity;

public class CharaDataClass
{
  public Guid Id { get; set; }

  // public string? ProjectId { get; set; }

  public string? FileId { get; set; }

  public string? CharaName { get; set; }

  public string? MaterialName { get; set; }

  public string? TimesName { get; set; }

  public SKBitmap? SrcImage { get; set; }

  public string? Thumbnail { get; set; }

  public SKBitmap? ThinImage { get; set; }

  public bool IsSelected { get; set; } = false;

  public override string ToString()
  {
    return $"id: {Id}  CharaName: {CharaName} MaterialName: {MaterialName} FileId: {FileId}";
  }
  // スタイルを動的に決定するメソッド
  public string GetCardStyle()
  {
    return $"background-color: {(IsSelected ? "lightcyan" : "white")};";
  }
}
