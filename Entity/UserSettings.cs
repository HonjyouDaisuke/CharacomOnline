using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

[Table("user_settings")]
public class UserSettings : BaseModel
{
	[Column("id")]
	public Guid Id { get; set; }

	[Column("user_id")]
	public Guid UserId { get; set; }

	[Column("is_standard")]
	public bool IsStandard { get; set; }

	[Column("is_center_line")]
	public bool IsCenterLine { get; set; }

	[Column("is_grid_line")]
	public bool IsGridLine { get; set; }

	[Column("r")]
	public double R { get; set; }

	[Column("is_noize")]
	public bool IsNoize { get; set; }

}
