using SkiaSharp;

namespace CharacomOnline.Entity;

public class CharaDataClass
{
  public int? Id { get; set; }

  public string ProjectId { get; set; }

  public string FileId { get; set; }

  public string? CharaName { get; set; }

  public string? MaterialName { get; set; }

  public string? TimesName { get; set; }

  public SKBitmap? SrcImage { get; set; }

  public string? Thumbnail { get; set; }

  public override string ToString()
  {
    return $"id: {Id} ProjectId:{ProjectId} CharaName: {CharaName} MaterialName: {MaterialName} FileId: {FileId}";
  }
}
