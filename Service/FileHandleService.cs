using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;

public struct FileInfomation
{
    public string CharaName { get; set; }
    public string MaterialName { get; set; }
    public string NumString { get; set; }
    public string Size { get; set; }
}

public struct SizeInfomation
{
    public long Width;
    public long Height;
}

public class FileHandleService
{
    public async Task<FileInfomation> GetDataInfo(IBrowserFile file)
    {
        FileInfomation resData = new FileInfomation();
        SizeInfomation resSize = await GetImageSizeAsync(file);

        resData.CharaName = "あ";
        resData.MaterialName = "a-bb";
        resData.NumString = "aaa";
        resData.Size = $"{resSize.Width} x {resSize.Height}";
        return resData;
    }

    // ImageSharp を使って画像サイズを取得
    public async Task<SizeInfomation> GetImageSizeAsync(IBrowserFile file)
    {
        using var stream = file.OpenReadStream();
        SizeInfomation size = new SizeInfomation();

        // ImageSharp で画像を読み込み、画像情報から幅と高さを取得
        var image = await Image.LoadAsync(stream);
        size.Width = image.Width;
        size.Height = image.Height;
        return size;
    }
}
