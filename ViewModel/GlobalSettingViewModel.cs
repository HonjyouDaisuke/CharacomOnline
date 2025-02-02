using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;

namespace CharacomOnline.ViewModel;

public class GlobalSettingViewModel(GlobalSettingRepository globalSettingRepository, StandardTableService standardTableService, StrokeTableService strokeTableService, BoxFileService boxFileService)
{
  public readonly GlobalSettingRepository _globalSettingRepository = globalSettingRepository ?? throw new ArgumentNullException(nameof(globalSettingRepository));
  private readonly StandardTableService _standardTableService = standardTableService ?? throw new ArgumentNullException(nameof(standardTableService));
  private readonly StrokeTableService _strokeTableService = strokeTableService ?? throw new ArgumentNullException(nameof(strokeTableService));
  private readonly BoxFileService _boxFileService = boxFileService ?? throw new ArgumentNullException(nameof(boxFileService));

  public async Task InsertStandardMasterAsync(string charaName, string fileId)
  {
    await standardTableService.AddRecodeAsync(charaName, fileId);
  }

  public async Task InsertStrokeMasterAsync(string charaName, string fileId)
  {
    await strokeTableService.AddRecodeAsync(charaName, fileId);
  }

  public void AllClearMaster()
  {
    _globalSettingRepository.ClearStandard();
    _globalSettingRepository.ClearStroke();
  }


  public async Task FetchAllMasterAsync()
  {
    Console.WriteLine("開始");

    var standard = await _standardTableService.GetAllAsync();
    var stroke = await _strokeTableService.GetAllAsync();

    _globalSettingRepository.ClearStandard();
    _globalSettingRepository.ClearStroke();
    foreach (var item in standard)
    {
      _globalSettingRepository.AddStandard(item);
    }

    foreach (var item in stroke)
    {
      _globalSettingRepository.AddStroke(item);
    }
    Console.WriteLine($"fetch終了 - standard = {standard.Count}  stroke = {stroke.Count}");
  }
}
