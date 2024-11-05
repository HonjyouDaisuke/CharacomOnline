using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;

namespace CharacomOnline.Service;

public struct FileInformation
{
	public string CharaName { get; set; }

	public string MaterialName { get; set; }

	public string NumString { get; set; }

	public string Size { get; set; }
}

public struct SizeInformation
{
	public long Width;
	public long Height;
}

public class FileHandleService
{
	public async Task<FileInformation> GetDataInfo(IBrowserFile file)
	{
		FileInformation resData = new();
		SizeInformation resSize = await GetImageSizeAsync(file);

		resData.CharaName = GetCharaNameFromFileName(file.Name);
		resData.MaterialName = GetMaterialNameFromFileName(file.Name);
		resData.NumString = GetNumStringFromFileName(file.Name);
		resData.Size = $"{resSize.Width} x {resSize.Height}";
		return resData;
	}

	// ImageSharp を使って画像サイズを取得
	public async Task<SizeInformation> GetImageSizeAsync(IBrowserFile file)
	{
		using var stream = file.OpenReadStream();
		SizeInformation size = new();

		// ImageSharp で画像を読み込み、画像情報から幅と高さを取得
		var image = await Image.LoadAsync(stream);
		size.Width = image.Width;
		size.Height = image.Height;
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
