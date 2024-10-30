using System.Drawing.Text;
using System.Threading;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Routing.Constraints;

namespace CharacomOnline;

public class FileUploadService
{
    private const long MaxFileSize = 10485760;
    private IWebHostEnvironment _webHostEnvironment;
    private readonly FileHandleService _fileHandleService;

    public FileUploadService(
        IWebHostEnvironment webHostEnvironment,
        FileHandleService fileHandleService
    )
    {
        _webHostEnvironment = webHostEnvironment;
        _fileHandleService = fileHandleService;
    }

    private async Task<string> SaveFileAsync(IBrowserFile file)
    {
        if (file.Size > MaxFileSize)
            return "";

        var uploadePath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
        Directory.CreateDirectory(uploadePath);

        var uniqueFileName = RandomStringMaker.makeString(10) + Path.GetExtension(file.Name);
        var filePath = Path.Combine(uploadePath, uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.OpenReadStream(MaxFileSize).CopyToAsync(stream);

        return $"/images/{uniqueFileName}";
    }

    public async Task<CharaDataClass> FileUploadAsync(IBrowserFile file)
    {
        var imageUrl = await SaveFileAsync(file);

        if (imageUrl == null)
            return null;

        FileInfomation fileInfo;

        fileInfo = await _fileHandleService.GetDataInfo(file);
        CharaDataClass uploadData = new CharaDataClass
        {
            Id = 0,
            CharaName = fileInfo.CharaName,
            MaterialName = fileInfo.MaterialName,
            Size = fileInfo.Size,
            ImageUrl = imageUrl,
        };
        return uploadData;
    }
}
