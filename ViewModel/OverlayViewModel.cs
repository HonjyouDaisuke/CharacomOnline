using Azure.Core;
using CharacomOnline.Entity;
using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;
using CharacomOnline.ViewModel;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using SkiaSharp;

namespace CharacomOnline.ViewModel;

public class OverlayViewModel
{
  private readonly CharaDataTableService _charaDataTableService;
  private readonly StandardTableService _standardTableService;
  private readonly SelectingItemsRepository _selectingItemsRepository;
  private readonly BoxFileService _boxFileService;

  public OverlayViewModel(
    BoxFileService boxFileService,
    StandardTableService standardTableService,
    SelectingItemsRepository selectingItemsRepository,
    CharaDataTableService charaDataTableService
  )
  {
    _charaDataTableService =
      charaDataTableService ?? throw new ArgumentNullException(nameof(charaDataTableService));
    _standardTableService =
      standardTableService ?? throw new ArgumentNullException(nameof(standardTableService));
    _selectingItemsRepository =
      selectingItemsRepository ?? throw new ArgumentNullException(nameof(selectingItemsRepository));
    _boxFileService = boxFileService ?? throw new ArgumentNullException(nameof(boxFileService));
  }

  public event Action<int>? ProgressChanged;

  public OverlayDataClass OverlayA { get; set; } = new();
  public OverlayDataClass OverlayB { get; set; } = new();

  public SKBitmap? ViewBitmap { get; set; } = new();

  public string currentChara = "";
  public string currentMaterial = "";

  public async Task<OverlayDataClass> MakeOverlayDataAsync(
    Guid projectId,
    string charaName,
    string materialName,
    string accessToken
  )
  {
    OverlayDataClass overlay = new();
    var selectedCharas = await _charaDataTableService.FetchSelectedCharaData(
      projectId,
      charaName,
      materialName,
      accessToken
    );
    if (selectedCharas == null)
      return overlay;
    var overlayBmp = new SKBitmap(160, 160);
    overlayBmp = ImageEffectService.WhiteFilledBitmap(overlayBmp);
    int count = 0;
    foreach (var item in selectedCharas)
    {
      if (!item.IsSelected)
        continue;
      if (overlayBmp == null)
        continue;
      if (item.ThinImage == null)
        continue;
      overlayBmp = ImageEffectService.OverlayBinaryImages(overlayBmp, item.ThinImage);
      count++;
    }
    if (overlayBmp == null)
      return overlay;
    //OverlayBmp = ImageEffectService.ColorChangeBitmap(OverlayBmp, SKColors.Blue);
    //await MakeViewBitmapAsync(isStandard, isCenterLine, isGridLine);
    overlay.CharaName = charaName;
    overlay.MaterialName = materialName;
    overlay.OverlayBmp = overlayBmp;
    overlay.SelectedItemCount = count;
    overlay.IsSelected = false;
    return overlay;
  }

  private void ClearOverlayBitmap(SKBitmap bmp)
  {
    bmp = new SKBitmap(160, 160);
    bmp = ImageEffectService.WhiteFilledBitmap(bmp);
  }

  public void ClearOverlayBitmapA()
  {
    ClearOverlayBitmap(OverlayA.OverlayBmp);
  }

  public void ClearOverlayBitmapB()
  {
    ClearOverlayBitmap(OverlayB.OverlayBmp);
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

  public async Task InitCharactersData(Guid projectId)
  {
    await _charaDataTableService.InitCharactersDataAsync(projectId);
  }

  public async Task InitMaterialsData(Guid projectId)
  {
    await _charaDataTableService.InitMaterialsData(projectId);
  }

  public List<string> GetMaterials()
  {
    return _selectingItemsRepository.GetMaterials();
  }
}
