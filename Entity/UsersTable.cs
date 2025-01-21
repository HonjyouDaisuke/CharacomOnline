using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;


[Table("users")] // テーブル名
public class UsersTable : BaseModel
{
  [Column("id")]
  public Guid Id { get; set; }

  [Column("user_name")]
  public string? Name { get; set; }

  [Column("email")]
  public string? Email { get; set; }

  [Column("picture_url")]
  public string? PictureUrl { get; set; }

  [Column("user_role")]
  public string? UserRole { get; set; }

  [Column("updated_at")]
  public string? UpdatedAt { get; set; }

  public override string ToString()
  {
    return $"id:{Id} name:{Name} email:{Email} picture:{PictureUrl} role:{UserRole}";
  }
}
