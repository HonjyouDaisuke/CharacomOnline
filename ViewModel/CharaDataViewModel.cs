using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;
using Microsoft.AspNetCore.Components.Forms;
using SkiaSharp;

namespace CharacomOnline.ViewModel;

public class CharaDataViewModel(
	StorageService storageService,
	CharaDataRepository charaDataRepository,
	SelectingItemsRepository selectingItemsRepository,
	BoxFileService boxFileService,
	StandardTableService standardTableService,
	StrokeTableService strokeTableService,
	ImagesTableService imagesTableService,
	CharaDataTableService charaDataTableService
)
{
	public readonly CharaDataRepository _charaDataRepository =
		charaDataRepository ?? throw new ArgumentNullException(nameof(charaDataRepository));
	private readonly StorageService _storageService =
		storageService ?? throw new ArgumentNullException(nameof(storageService));
	private readonly ImagesTableService _imagesTableService =
		imagesTableService ?? throw new ArgumentNullException(nameof(imagesTableService));
	private readonly CharaDataTableService _charaDataTableService =
		charaDataTableService ?? throw new ArgumentNullException(nameof(charaDataTableService));
	private readonly StandardTableService _standardTableService =
		standardTableService ?? throw new ArgumentNullException(nameof(standardTableService));
	public readonly SelectingItemsRepository _selectingItemsRepository =
		selectingItemsRepository ?? throw new ArgumentNullException(nameof(selectingItemsRepository));
	private readonly StrokeTableService _strokeTableService =
		strokeTableService ?? throw new ArgumentNullException(nameof(strokeTableService));
	private readonly BoxFileService _boxFileService =
		boxFileService ?? throw new ArgumentNullException(nameof(boxFileService));
	public event Action<int>? ProgressChanged;
	public SKBitmap? OverlayBmp { get; set; }

	public SKBitmap? ViewBitmap { get; set; }

	public string currentChara = "";
	public string currentMaterial = "";

	public async Task MakeOverlayBitmapAsync(bool isStandard, bool isCenterLine, bool isGridLine)
	{
		await Task.Run(async () =>
		{
			OverlayBmp = new SKBitmap(160, 160);
			OverlayBmp = ImageEffectService.WhiteFilledBitmap(OverlayBmp);
			foreach (var item in _charaDataRepository.viewCharaData)
			{
				if (!item.IsSelected)
					continue;
				Console.WriteLine($"overLayに追加:{item.FileId}");
				if (OverlayBmp == null)
					continue;
				if (item.ThinImage == null)
					continue;
				OverlayBmp = ImageEffectService.OverlayBinaryImages(OverlayBmp, item.ThinImage);
			}
			if (OverlayBmp == null)
				return;
			OverlayBmp = ImageEffectService.ColorChangeBitmap(OverlayBmp, SKColors.Blue);
			await MakeViewBitmapAsync(isStandard, isCenterLine, isGridLine);
			Console.WriteLine("追加完了です。");
		});
	}

	public async Task MakeViewBitmapAsync(bool isStandard, bool isCenterLine, bool isGridLine)
	{
		ViewBitmap = new SKBitmap(160, 160);
		ViewBitmap = ImageEffectService.WhiteFilledBitmap(ViewBitmap);
		if (OverlayBmp == null) return;
		ViewBitmap = ImageEffectService.OverlayBinaryImages(ViewBitmap, OverlayBmp);
		if (ViewBitmap == null) return;
		await Task.Run(() =>
		{
			if (isStandard && charaDataRepository.StandardThinBmp != null)
			{
				Console.WriteLine("standard");
				var Standard = ImageEffectService.ColorChangeBitmap(charaDataRepository.StandardThinBmp, SKColors.Red);
				ViewBitmap = ImageEffectService.OverlayBinaryImages(ViewBitmap, Standard);
			}
			if (isGridLine)
			{
				Console.WriteLine("grid");
				ViewBitmap = ImageEffectService.GridLine(ViewBitmap);
			}
			if (isCenterLine)
			{
				Console.WriteLine("center");
				ViewBitmap = ImageEffectService.CenterLine(ViewBitmap);
			}
			if (isGridLine || isCenterLine)
			{
				Console.WriteLine("outline");
				ViewBitmap = ImageEffectService.OutlineFrame(ViewBitmap);
			}
		});
	}
	public void ClearOverlayBitmap()
	{
		OverlayBmp = new SKBitmap(160, 160);
		OverlayBmp = ImageEffectService.WhiteFilledBitmap(OverlayBmp);
	}

	public bool SelectChara(string chara)
	{
		if (chara == currentChara)
		{
			return false;
		}

		currentChara = chara;
		return true;
	}

	public bool SelectMaterial(string material)
	{
		if (material == currentMaterial)
		{
			return false;
		}

		currentMaterial = material;
		return true;
	}

	public async Task<Guid?> InsertCharaDataAsync(string fileName, Guid projectId, string fileId)
	{
		//var imageId = await DataInputAsync(file);
		//if (imageId == null) return null;

		var fileInfo = FileNameService.GetDataInfo(fileName);
		var charaDataId = await _charaDataTableService.CreateCharaData(projectId, fileId, fileInfo);
		return charaDataId;
	}

	public async Task<Guid?> DataInputAsync(IBrowserFile file)
	{
		// ファイルをStorageに保存
		var uniqueFileName = RandomStringMaker.MakeString(10) + Path.GetExtension(file.Name);
		await _storageService.UploadFileAsync(file, uniqueFileName);
		var bmp = await ImageEffectService.LoadImageFileAsync(file);
		if (bmp == null)
			return null;
		var thumbnailBmp = ImageEffectService.ResizeBitmap(bmp, 50, 50);
		if (thumbnailBmp == null)
			return null;
		var thumbnailBmpString = ImageEffectService.GetBinaryImageData(thumbnailBmp);
		if (thumbnailBmpString == null)
			return null;
		// CharaImage情報をDBに保存
		var imageId = await _imagesTableService.AddImageFile(
			uniqueFileName,
			"Images/",
			file.Size,
			thumbnailBmpString
		);

		return imageId;
	}

	public async Task FetchCharaDataAsync(Guid projectId, Guid modelId)
	{
		_charaDataRepository.ClearAppraisals();
		var charaDataList = await _charaDataTableService.FetchCharaDataFromProject(projectId, modelId);
		await _charaDataRepository.AddAppraisalsAsync(charaDataList);
	}

	public async Task InitCharactersData(Guid projectId)
	{
		await _charaDataTableService.InitCharactersDataAsync(projectId);
	}

	public async Task InitMaterialsData(Guid projectId)
	{
		await _charaDataTableService.InitMaterialsData(projectId);
	}

	public async Task GetViewCharaDataAsync(
		string accessToken,
		Guid projectId,
		CancellationToken token,
		bool isStandard,
		bool isCenterLine,
		bool isGridLine
	)
	{
		Console.WriteLine($"chara = {currentChara} materila = {currentMaterial}");
		if (string.IsNullOrEmpty(currentChara) || string.IsNullOrEmpty(currentMaterial))
			return;
		await _charaDataTableService.GetCharaListAsync(
			projectId,
			currentChara,
			currentMaterial,
			accessToken,
			token,
			(progress) =>
			{
				ProgressChanged?.Invoke(progress);
				Console.WriteLine($"progress:{progress}");
			}
		);
		await MakeOverlayBitmapAsync(isStandard, isCenterLine, isGridLine);
	}

	public async Task ClearViewCharaData()
	{
		await _charaDataRepository.ClearViewCharaDataAsync();
		ClearOverlayBitmap();
	}

	public async Task GetStandardData(string accessToken, string charaName)
	{
		var fileId = await _standardTableService.GetFileIdAsync(charaName);
		Console.WriteLine($"標準ID:{fileId}");
		if (fileId == null)
		{
			Console.WriteLine($"標準字体が見つかりませんでした。{fileId}");
			return;
		}
		var bmp = await _boxFileService.DownloadFileAsSKBitmapAsync(fileId, accessToken);
		if (bmp == null)
			return;
		_charaDataRepository.AddStandardBmp(bmp);
	}

	public async Task GetStrokeData(string accessToken, string charaName)
	{
		var fileId = await _strokeTableService.GetFileIdAsync(charaName);
		Console.WriteLine($"筆順ID:{fileId}");
		if (fileId == null)
		{
			Console.WriteLine($"筆順書体が見つかりませんでした。{fileId}");
			return;
		}
		var bmp = await _boxFileService.DownloadFileAsSKBitmapAsync(fileId, accessToken);
		if (bmp == null)
			return;
		_charaDataRepository.AddStrokeBmp(bmp);
	}

	public async Task SaveSelectChangeAsync(Guid charaId, bool isSelected, Guid currentUserId)
	{
		await _charaDataTableService.UpdateCharaSelectChangeAsync(charaId, isSelected, currentUserId);
	}
}
