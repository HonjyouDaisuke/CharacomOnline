namespace CharacomOnline;

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
        string res = "";
        res =
            $"id: {this.Id} CharaName: {this.CharaName} MaterialName: {this.MaterialName} numString: {this.NumString} size: {this.Size} ({this.ImageUrl})";
        return res;
    }
}
