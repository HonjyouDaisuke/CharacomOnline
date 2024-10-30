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

		resData.CharaName = GetCharaNameFromFileName(file.Name);
		resData.MaterialName = GetMaterialNameFromFileName(file.Name);
		resData.NumString = GetNumStringFromFileName(file.Name);
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

	private string GetCharaNameFromFileName(string fileName)
	{
		string result = "";
		result = fileName.Substring(0, 1);
		return result;
	}

	private string GetMaterialNameFromFileName(string fileName)
	{
		string result = "";
		result = fileName.Substring(1, fileName.LastIndexOf("-") - 1);
		return result;
	}

	private string GetNumStringFromFileName(string fileName)
	{
		string result = "";
		int startPos = fileName.LastIndexOf("-") + 1;
		int endPos = fileName.LastIndexOf(".") - startPos;
		result = fileName.Substring(startPos, endPos);
		return result;
	}
}
