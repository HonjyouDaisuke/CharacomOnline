using CharacomOnline.Entity;
using CharacomOnline.ImageProcessing;
using CharacomOnline.Repositories;
using CharacomOnline.Service;
using SkiaSharp;
namespace CharacomOnline.ViewModel;

public class ImageProcessViewModel(ImageProcessRepository imageProcessData)
{
  public readonly ImageProcessRepository imageProcessRepository = imageProcessData ?? throw new ArgumentNullException(nameof(imageProcessData));

  public async Task AddAppraisalData(SKBitmap srcBitmap)
  {
    var smallBmp = ImageEffectService.ResizeBitmap(srcBitmap, 160, 160);
    var binaryBmp = ImageEffectService.GetBinaryBitmap(smallBmp);
    ThinningProcess thinningProcess = new ThinningProcess(binaryBmp);
    Console.WriteLine("開始します");
    var thinBmp = await thinningProcess.ThinBinaryImageAsync();
    Console.WriteLine("終了です。");

    //imageProcessData.AddAppraisalData(srcBitmap, thinBmp);
  }

  public async Task CreateAppraisalOverlay(List<CharaDataClass> items)
  {
    foreach (var item in items)
    {
      if (imageProcessRepository.AppraisalCharaData.Contains(item)) continue;

      var smallBmp = ImageEffectService.ResizeBitmap(item.SrcImage, 160, 160);
      var binaryBmp = ImageEffectService.GetBinaryBitmap(smallBmp);
      ThinningProcess thinningProcess = new ThinningProcess(binaryBmp);
      Console.WriteLine("開始します");
      var thinBmp = await thinningProcess.ThinBinaryImageAsync();
      Console.WriteLine("終了です。");
      imageProcessRepository.AddAppraisalData(item, item.SrcImage, thinBmp);
    }
    await imageProcessRepository.CreateAppraisalImageAsync();
  }
}
