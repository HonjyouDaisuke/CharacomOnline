using CharacomOnline.Entity;
using Supabase;

namespace CharacomOnline.Service.TableService;

public class ImagesTableService(Client supabaseClient)
{
  private readonly Client _supabaseClient = supabaseClient;

  public async Task<Guid?> AddImageFile(
    string fileName,
    string filePath,
    long fileSize,
    string thumbnail
  )
  {
    Guid newId = Guid.NewGuid();
    var imagesItem = new ImagesTable
    {
      Id = newId,
      FileName = fileName,
      FilePath = filePath,
      FileSize = fileSize,
      Thumbnail = thumbnail,
    };

    // データベースに挿入
    // Console.WriteLine($"user name = {client.Auth.CurrentUser.Email}");
    var response = await _supabaseClient.From<ImagesTable>().Insert(imagesItem);
    var insertedItem = response.Models.FirstOrDefault();

    if (response == null || insertedItem == null)
    {
      Console.WriteLine($"Insert Error ImagesTable... FileName={fileName}");
      return null;
    }
    Console.WriteLine("ImagesTable Insert成功");
    return insertedItem.Id;
  }
}
