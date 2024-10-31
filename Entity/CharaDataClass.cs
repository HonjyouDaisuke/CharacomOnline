namespace CharacomOnline.Entity;

public class CharaDataClass
{
	public int? Id { get; set; }

	public string? CharaName { get; set; }

	public string? MaterialName { get; set; }

	public string? NumString { get; set; }

	public string? Size { get; set; }

	public string? ImageUrl { get; set; }

	public override string ToString()
	{
		return $"id: {Id} CharaName: {CharaName} MaterialName: {MaterialName} numString: {NumString} size: {Size} ({ImageUrl})";
	}
}
