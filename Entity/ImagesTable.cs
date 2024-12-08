namespace CharacomOnline.Entity;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("ImagesTable")] // テーブル名
public class ImagesTable : BaseModel
{
  [Column("file_name")]
  public string FileName { get; set; }

  [Column("file_path")]
  public string FilePath { get; set; }

  [Column("file_size")]
  public long FileSize { get; set; }

}
