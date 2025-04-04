using CharacomOnline.Entity;
using CharacomOnline.ImageProcessing;
using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;
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

  public OverlayDataClass? OverlayA { get; set; }
  public OverlayDataClass? OverlayB { get; set; }

  public SKBitmap? ViewBitmap { get; set; } = new();

  public string currentChara = "";
  public string currentMaterial = "";

  private SKBitmap? standardBitmap;

  private SKBitmap? standardThinBitmap;

  public async Task SetStandardBitmap(string accessToken, string charaName)
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

    standardBitmap = bmp.Copy();
    // 細線化
    var resized = ImageEffectService.ResizeBitmap(bmp.Copy(), 160, 160);
    var binary = ImageEffectService.GetBinaryBitmap(resized);
    ThinningProcess thin = new ThinningProcess(binary);
    standardThinBitmap = thin.ThinBinaryImage();
  }

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
      overlayBmp = ImageEffectService.OverlayImages(overlayBmp, item.ThinImage);
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

  private void ClearOverlayBitmap(SKBitmap? bmp)
  {
    bmp = new SKBitmap(160, 160);
    bmp = ImageEffectService.WhiteFilledBitmap(bmp);
  }

  public void ClearOverlayBitmapA()
  {
    if (OverlayA == null)
      return;
    ClearOverlayBitmap(OverlayA.OverlayBmp);
  }

  public void ClearOverlayBitmapB()
  {
    if (OverlayB == null)
      return;
    ClearOverlayBitmap(OverlayB.OverlayBmp);
  }

  public void ClearOverlayA()
  {
    OverlayA = new();
  }

  public void ClearOverlayB()
  {
    OverlayB = new();
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

  public void ChangeOverlayColor(bool isBoxA, string color)
  {
    var toColor = ImageEffectService.ConvertRgbStringToSKColor(color);
    var target = (isBoxA) ? OverlayA : OverlayB;

    Console.WriteLine($"target = {target!.MaterialName} itemCount = {target.SelectedItemCount}");
    if (target == null)
      return;
    if (target.OverlayBmp == null)
      return;
    target.OverlayBmp = ImageEffectService.ColorChangeBitmap(target.OverlayBmp, toColor);
  }

  public async Task MakeViewBitmap(bool isCenterLine, bool isGridLine, bool isStandard)
  {
    await Task.Run(() =>
    {
      if (OverlayA == null && OverlayB == null)
        return;
      ViewBitmap = new SKBitmap(160, 160);
      ViewBitmap = ImageEffectService.WhiteFilledBitmap(ViewBitmap);

      if (OverlayA!.SelectedItemCount == 0 && OverlayB!.SelectedItemCount > 0)
      {
        ViewBitmap = ImageEffectService.OverlayImages(ViewBitmap!, OverlayB.OverlayBmp);
      }
      else if (OverlayA.SelectedItemCount > 0 && OverlayB!.SelectedItemCount == 0)
      {
        ViewBitmap = ImageEffectService.OverlayImages(ViewBitmap!, OverlayA.OverlayBmp);
      }
      else
      {
        if (OverlayA?.SelectedItemCount > OverlayB?.SelectedItemCount)
        {
          ViewBitmap = ImageEffectService.OverlayImages(OverlayA.OverlayBmp, OverlayB.OverlayBmp);
        }
        else
        {
          ViewBitmap = ImageEffectService.OverlayImages(OverlayB!.OverlayBmp, OverlayA!.OverlayBmp);
        }
      }
      setStandard(isStandard);

      if (ViewBitmap != null)
      {
        ViewBitmap = ImageEffectService.ResizeBitmap(ViewBitmap, 320, 320);
      }

      setLinesToViewBitmap(isCenterLine, isGridLine, isStandard);
    });
  }

  private void setStandard(bool isStandard)
  {
    if (!isStandard)
      return;
    if (standardThinBitmap == null)
      return;
    if (ViewBitmap == null)
      return;
    var redThin = ImageEffectService.ColorChangeBitmap(standardThinBitmap, SKColors.Red);
    ViewBitmap = ImageEffectService.OverlayImages(ViewBitmap, redThin);
  }

  private void setLinesToViewBitmap(bool isCenterLine, bool isGridLine, bool isStandard)
  {
    if (ViewBitmap == null)
      return;
    if (isGridLine)
    {
      ViewBitmap = ImageEffectService.GridLine(ViewBitmap);
    }
    if (isCenterLine)
    {
      ViewBitmap = ImageEffectService.CenterLine(ViewBitmap);
    }
    if (isGridLine || isCenterLine)
    {
      ViewBitmap = ImageEffectService.OutlineFrame(ViewBitmap);
    }
  }
}
