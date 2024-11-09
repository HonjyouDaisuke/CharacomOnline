namespace CharacomOnline.ImageProcessing.WeightDirectionSubProcess;

/// <summary>
/// 量子化を行うクラス
/// </summary>
public static class QuantizationClass
{
	private const int COUNTSIZE = 10;
	private const int DIRECTIONS = 16;
	private const int MAXX = 16;
	private const int MAXY = 16;

	/// <summary>
	///  量子化
	///  160 x 160 →　10 x 10マスを合計して16x16に圧縮
	/// </summary>
	/// <param name="srcArray">方向線素データ</param>
	/// <returns>量子化後のデータ.</returns>
	public static byte[,,] Quantization(byte[,,] srcArray)
	{
		byte[,,] quantizationData = new byte[DIRECTIONS, MAXY, MAXX];

		for (int d = 0; d < DIRECTIONS; d++)
		{
			for (int y = 0; y < MAXY; y++)
			{
				for (int x = 0; x < MAXX; x++)
				{
					quantizationData[d, y, x] = CountQuantizationFeature(srcArray, d, x, y);
				}
			}
		}

		return quantizationData;
	}

	/// <summary>
	///  量子化用補助関数.
	/// </summary>
	/// <param name="src">元データ.</param>
	/// <param name="d">方向.</param>
	/// <param name="x">x座標.</param>
	/// <param name="y">y座標.</param>
	/// <returns>量子化合計値.</returns>
	private static byte CountQuantizationFeature(byte[,,] src, int d, int x, int y)
	{
		byte sum = 0;

		for (int m = 0; m < COUNTSIZE; m++)
		{
			for (int n = 0; n < COUNTSIZE; n++)
			{
				sum += src[d, (COUNTSIZE * y) + m, (COUNTSIZE * x) + n];
			}
		}

		return sum;
	}
}