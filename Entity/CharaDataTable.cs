using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace CharacomOnline.Entity;

[Table("chara_data")] // テーブル名
public class CharaDataTable : BaseModel
{
	[Column("id")]
	public Guid? Id { get; set; }

	[Column("project_id")]
	public Guid? ProjectId { get; set; }

	[Column("chara_image_id")]
	public Guid? CharaImageId { get; set; }

	[Column("model_id")]
	public Guid? ModelId { get; set; }

	[Column("material_name")]
	public string? MaterialName { get; set; }

	[Column("chara_name")]
	public string? CharaName { get; set; }

	[Column("times")]
	public string? TimesString { get; set; }

	[Column("size_string")]
	public string? SizeString { get; set; }
}
