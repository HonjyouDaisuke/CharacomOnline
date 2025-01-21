using CharacomOnline.Entity;
using Microsoft.AspNetCore.Components.Forms;

namespace CharacomOnline.Service;

public class FileUploadService
{
  private const long MaxFileSize = 10485760;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly FileHandleService _fileHandleService;

  public FileUploadService(
      IWebHostEnvironment webHostEnvironment,
      FileHandleService fileHandleService)
  {
    _webHostEnvironment = webHostEnvironment;
    _fileHandleService = fileHandleService;
  }

  public async Task<CharaDataClass?> FileUploadAsync(IBrowserFile file)
  {
    var imageUrl = await SaveFileAsync(file);

    if (imageUrl == null)
    {
      return null;
    }

    FileInformation fileInfo;

    fileInfo = await _fileHandleService.GetDataInfo(file);
    CharaDataClass uploadData = new()
    {
      //Id = 0,
      CharaName = fileInfo.CharaName,
      MaterialName = fileInfo.MaterialName,
      TimesName = fileInfo.TimesName,
      // ImageUrl = imageUrl,
    };
    return uploadData;
  }

  private async Task<string> SaveFileAsync(IBrowserFile file)
  {
    if (file.Size > MaxFileSize)
    {
      return string.Empty;
    }

    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
    Directory.CreateDirectory(uploadPath);

    var uniqueFileName = RandomStringMaker.MakeString(10) + Path.GetExtension(file.Name);
    var filePath = Path.Combine(uploadPath, uniqueFileName);

    using var stream = new FileStream(filePath, FileMode.Create);
    await file.OpenReadStream(MaxFileSize).CopyToAsync(stream);

    return $"/images/{uniqueFileName}";
  }
}
