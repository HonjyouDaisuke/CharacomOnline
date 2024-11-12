using CharacomOnline.ImageProcessing.WeightDirectionSubProcess;
using CharacomOnline.Service;
using SkiaSharp;

namespace CharacomOnline.ImageProcessing;

public static class WeightedDirectionHistogramAnalyzer
{
	public static async Task<double[]> WeightedDirectionProcessAsync(SKBitmap srcBitmap)
	{
		var kajyu = await Task.Run(() => GenerateWeightedDirectionHistogram(srcBitmap));
		return kajyu;
	}

	public static double[] GenerateWeightedDirectionHistogram(SKBitmap srcBitmap)
	{
		SKBitmap workBitmap = ImageEffectService.ResizeBitmap(srcBitmap, 160, 160);
		if (workBitmap == null)
		{
			return [];
		}

		SKBitmap monoBmp = ImageEffectService.GetBinaryBitmap(workBitmap);
		if (monoBmp == null)
		{
			return [];
		}

		SKBitmap noiseReducedBmp = NoiseReductionClass.NoiseReduction(monoBmp);
		SKBitmap normalizedBmp = NormalizeClass.Normalize(noiseReducedBmp, 38.0);
		var bitmapArray = ImageEffectService.ImageArrayFromBitmap(normalizedBmp);
		var trackArray = BorderlineTrackingClass.BorderlineTracking(bitmapArray);
		Console.WriteLine("Start 量子化");
		var quantizedArray = QuantizationClass.Quantization(trackArray);

		Console.WriteLine("Start ガウスフィルタ");
		var gaussianArray = GaussianClass.GaussianFilterProcess(quantizedArray);

		Console.WriteLine("Start 方向圧縮");
		var compressedArray = DirectionCompressClass.DirectionalCompression(gaussianArray);

		Console.WriteLine("Start 同一視");
		var weightedArray = ContrarianClass.ContrarianIdentification(compressedArray);

		Console.WriteLine("Start 1次元化");
		var kajyuData = OneDepersonalization(weightedArray);

		return kajyuData;
	}

	/// <summary>
	///  加重方向指数ヒストグラム特徴を1次元配列にする
	/// </summary>
	/// <param name="srcArray">元データ(4 x 8 x 8)</param>
	/// <returns>256次元データ</returns>
	private static double[] OneDepersonalization(double[,,] srcArray)
	{
		double[] result = new double[
			srcArray.GetLength(0) * srcArray.GetLength(1) * srcArray.GetLength(2)
		];
		for (int d = 0; d < srcArray.GetLength(0); d++)
		{
			for (int j = 0; j < srcArray.GetLength(1); j++)
			{
				for (int i = 0; i < srcArray.GetLength(2); i++)
				{
					result[(d * 64) + (j * 8) + i] = srcArray[d, j, i];
				}
			}
		}

		return result;
	}
}
