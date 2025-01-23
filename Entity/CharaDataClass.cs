using SkiaSharp;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CharacomOnline.Entity;

[Table("chara_data")]
public class CharaDataClass : BaseModel
{
  [PrimaryKey("id")]
  [Column("id")]
  public Guid Id { get; set; }

  // public string? ProjectId { get; set; }

  [Column("file_id")]
  public string? FileId { get; set; }

  [Column("chara_name")]
  public string? CharaName { get; set; }

  [Column("material_name")]
  public string? MaterialName { get; set; }

  [Column("times_name")]
  public string? TimesName { get; set; }

  public SKBitmap? SrcImage { get; set; }

  public string? Thumbnail { get; set; }

  public SKBitmap? ThinImage { get; set; }

  [Column("is_selected")]
  public bool IsSelected { get; set; } = false;

  [Column("updated_by")]
  public Guid? UpdatedBy { get; set; }

  [Column("updated_at")]
  public DateTime? UpdatedAt { get; set; }

  public override string ToString()
  {
    return $"id: {Id}  CharaName: {CharaName} MaterialName: {MaterialName} FileId: {FileId}";
  }

  // �X�^�C���𓮓I�Ɍ��肷�郁�\�b�h
  public string GetCardStyle()
  {
    return $"background-color: {(IsSelected ? "lightcyan" : "white")};";
  }
}
