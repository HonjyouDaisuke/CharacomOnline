using CharacomOnline.Entity;

namespace CharacomOnline.Service.TableService;

public class ImagesTableService
{
  private SupabaseService _supabaseService;

  public ImagesTableService(SupabaseService supabaseService)
  {
    _supabaseService = supabaseService;
  }

  public async Task AddImageFile(Supabase.Client client, string fileName, string filePath, long fileSize)
  {
    var imagesItem = new ImagesTable
    {
      FileName = fileName,
      FilePath = filePath,
      FileSize = fileSize,
    };

    // データベースに挿入
    // Console.WriteLine($"user name = {client.Auth.CurrentUser.Email}");
    var response = await client.From<ImagesTable>().Insert(imagesItem);
    if (response != null)
    {
      Console.WriteLine($"User {fileName} added successfully.");
    }
  }
}
