using CharacomOnline.Entity;
using CharacomOnline.ImageProcessing;
using CharacomOnline.Service;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace CharacomOnline.Repositories;

public class CharaDataRepository
{
  private readonly List<CharaDataClass> targets = new();
  public List<CharaDataClass> viewCharaData = new();
  private readonly object _viewCharaDataLock = new();
  public ObservableCollection<CharaDataClass> Appraisals { get; set; } =
      new ObservableCollection<CharaDataClass>();
  public SKBitmap? StandardBmp { get; set; }
  public SKBitmap? StandardThinBmp { get; set; }
  public SKBitmap? StrokeBmp { get; set; }

  public async Task<bool> IsViewCharaDataExistsAsync(Guid id)
  {
    return await Task.Run(() =>
    {
      lock (_viewCharaDataLock)
      {
        return viewCharaData.Any(item => item.Id == id);
      }
    });
  }

  public async Task AddViewCharaDataAsync(CharaDataClass newItem)
  {
    if (newItem == null) return;

    await Task.Run(() =>
    {
      lock (_viewCharaDataLock)
      {
        viewCharaData.Add(newItem);
      }
    });
  }

  public async Task ClearViewCharaDataAsync()
  {
    await Task.Run(() =>
    {
      viewCharaData.Clear();
    });
  }

  public async Task AddAppraisalsAsync(List<CharaDataClass>? data)
  {
    if (data == null) return;

    await Task.Run(() =>
    {
      foreach (var item in data)
      {
        if (item == null) continue;
        lock (Appraisals)
        {
          Appraisals.Add(item); // スレッドセーフに操作
        }
      }
    });
  }

  public void ClearAppraisals()
  {
    Appraisals.Clear();
  }

  public void AddStandardBmp(SKBitmap? standardBmp)
  {
    StandardBmp = standardBmp;
    if (StandardBmp == null) return;
    var resized = ImageEffectService.ResizeBitmap(StandardBmp, 160, 160);
    var binary = ImageEffectService.GetBinaryBitmap(resized);
    ThinningProcess thin = new ThinningProcess(binary);
    StandardThinBmp = thin.ThinBinaryImage();
  }

  public void AddStrokeBmp(SKBitmap? strokeBmp)
  {
    StrokeBmp = strokeBmp;
  }
}
