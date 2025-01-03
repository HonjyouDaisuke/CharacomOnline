using CharacomOnline.Entity;
using CharacomOnline.Service;
using SkiaSharp;

namespace CharacomOnline.Repositories;

public class ImageProcessRepository
{
  //public ObservableCollection<ImageProcessDataClass> imageProcessData = new ObservableCollection<ImageProcessDataClass>();
  public List<ImageProcessDataClass> Appraisals = new();
  public List<CharaDataClass> AppraisalCharaData = new();
  public string AppraisalsSrcImageData = "";
  public string AppraisalsThinImageData = "";

  public void ClearAppraisalData()
  {
    Appraisals.Clear();
  }

  public void AddAppraisalData(CharaDataClass item, SKBitmap srcBitmap, SKBitmap thinBitmap)
  {
    ImageProcessDataClass newItem = new ImageProcessDataClass();

    newItem.SrcBitmap = ImageEffectService.GetBinaryBitmap(srcBitmap);
    newItem.SrcBitmap = ImageEffectService.ResizeBitmap(newItem.SrcBitmap, 160, 160);
    newItem.SrcImageData = ImageEffectService.GetBinaryImageData(srcBitmap);
    newItem.ThinningBitmap = thinBitmap;
    newItem.ThinningImageData = ImageEffectService.GetBinaryImageData(newItem.ThinningBitmap);
    Appraisals.Add(newItem);
    AppraisalCharaData.Add(item);
  }

  public async Task CreateAppraisalImageAsync()
  {
    SKBitmap src = new SKBitmap(160, 160);
    src = ImageEffectService.WhiteFilledBitmap(src);
    SKBitmap thin = new SKBitmap(160, 160);
    thin = ImageEffectService.WhiteFilledBitmap(thin);

    foreach (var item in Appraisals)
    {
      src = ImageEffectService.OverlayBinaryImages(src, item.SrcBitmap);
      thin = ImageEffectService.OverlayBinaryImages(thin, item.ThinningBitmap);
    }
    AppraisalsSrcImageData = ImageEffectService.GetBinaryImageData(src);
    AppraisalsThinImageData = ImageEffectService.GetBinaryImageData(thin);
  }
}
