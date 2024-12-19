using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;
using Microsoft.AspNetCore.Components.Forms;

namespace CharacomOnline.ViewModel;

public class CharaDataViewModel(StorageService storageService, CharaDataRepository charaDataRepository, ImagesTableService imagesTableService, CharaDataTableService charaDataTableService)
{
	public readonly CharaDataRepository _charaDataRepository = charaDataRepository ?? throw new ArgumentNullException(nameof(charaDataRepository));
	private FileHandleService FileHandleService { get; set; } = new FileHandleService();
	private readonly StorageService _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
	private readonly FileUploadService _fileUploadService;
	private readonly ImagesTableService _imagesTableService = imagesTableService ?? throw new ArgumentNullException(nameof(imagesTableService));
	private readonly CharaDataTableService _charaDataTableService = charaDataTableService ?? throw new ArgumentNullException(nameof(charaDataTableService));

	public async Task<Guid?> InsertCharaDataAsync(IBrowserFile file, Guid projectId, Guid modelId)
	{
		var imageId = await DataInputAsync(file);
		if (imageId == null) return null;

		var fileInfo = await FileHandleService.GetDataInfo(file);
		var charaDataId = await _charaDataTableService.CreateCharaData(projectId, (Guid)imageId, modelId, fileInfo);
		return charaDataId;
	}

	public async Task<Guid?> DataInputAsync(IBrowserFile file)
	{
		// ÉtÉ@ÉCÉãÇStorageÇ…ï€ë∂
		var uniqueFileName = RandomStringMaker.MakeString(10) + Path.GetExtension(file.Name);
		await _storageService.UploadFileAsync(file, uniqueFileName);
		// CharaImageèÓïÒÇDBÇ…ï€ë∂
		var imageId = await _imagesTableService.AddImageFile(uniqueFileName, "Images/", file.Size);

		return imageId;
	}

	public async Task FetchCharaDataAsync(Guid projectId, Guid modelId, Action<double, double> updateProgress = null)
	{
		_charaDataRepository.ClearAppraisals();
		var charaDataList = await _charaDataTableService.FetchCharaDataFromProject(projectId, modelId, updateProgress);
		await _charaDataRepository.AddAppraisalsAsync(charaDataList);
	}
}
