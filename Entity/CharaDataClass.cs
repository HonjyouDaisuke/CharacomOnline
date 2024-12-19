using SkiaSharp;

namespace CharacomOnline.Entity;

public class CharaDataClass
{
	public int? Id { get; set; }

	public string? CharaName { get; set; }

	public string? MaterialName { get; set; }

	public string? TimesString { get; set; }

	public string? Size { get; set; }

	public SKBitmap? SrcImage { get; set; }

	public override string ToString()
	{
		return $"id: {Id} CharaName: {CharaName} MaterialName: {MaterialName} numString: {TimesString} size: {Size} ";
	}
}
