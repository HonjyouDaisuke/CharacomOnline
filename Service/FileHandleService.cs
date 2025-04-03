using Microsoft.AspNetCore.Components.Forms;

//using SixLabors.ImageSharp;

namespace CharacomOnline.Service;

public struct SizeInformation
{
  public long Width;
  public long Height;
}

public class FileHandleService
{
  public FileInformation GetDataInfo(IBrowserFile file)
  {
    FileInformation resData = new();
    SizeInformation resSize = GetImageSizeAsync(file);

    resData.CharaName = GetCharaNameFromFileName(file.Name);
    resData.MaterialName = GetMaterialNameFromFileName(file.Name);
    resData.TimesName = GetNumStringFromFileName(file.Name);
    return resData;
  }

  // ImageSharp を使って画像サイズを取得
  public SizeInformation GetImageSizeAsync(IBrowserFile file)
  {
    using var stream = file.OpenReadStream();
    SizeInformation size = new();

    // ImageSharp で画像を読み込み、画像情報から幅と高さを取得
    //var image = await Image.LoadAsync(stream);
    size.Width = 10; //image.Width;
    size.Height = 10; //image.Height;
    return size;
  }

  private static string GetCharaNameFromFileName(string fileName)
  {
    return fileName[..1];
  }

  private static string GetMaterialNameFromFileName(string fileName)
  {
    return fileName[1..fileName.LastIndexOf('-')];
  }

  private static string GetNumStringFromFileName(string fileName)
  {
    int startPos = fileName.LastIndexOf('-') + 1;
    int endPos = fileName.LastIndexOf('.') - startPos;
    return fileName.Substring(startPos, endPos);
  }
}
