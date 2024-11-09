namespace CharacomOnline.ImageProcessing.WeightDirectionSubProcess;

/// <summary>
/// ガウスフィルタを掛けるクラス
/// </summary>
public static class GaussianClass
{
	private const int DIRECTIONS = 16;
	private const int MAXX = 8;
	private const int MAXY = 8;

	private static readonly double[,] Gaussian =
	{
		{ 0.00, 0.09, 0.17, 0.09, 0.00 },
		{ 0.09, 0.57, 1.05, 0.57, 0.09 },
		{ 0.17, 1.05, 1.94, 1.05, 0.17 },
		{ 0.09, 0.57, 1.05, 0.57, 0.09 },
		{ 0.00, 0.09, 0.17, 0.09, 0.00 },
	};

	/// <summary>
	///  ガウスフィルタプロセス
	///  元のデータに人マスおきにガウスフィルタを掛ける.
	/// </summary>
	/// <param name="srcArray">元データ(16方向 x 16 x 16)</param>
	/// <returns>フィルタ後のデータ(16方向 x 8 x 8)</returns>
	public static double[,,] GaussianFilterProcess(byte[,,] srcArray)
	{
		double[,,] filteredArray = new double[DIRECTIONS, MAXY, MAXX];

		for (int d = 0; d < srcArray.GetLength(0); d++)
		{
			for (int j = 0; j < MAXY; j++)
			{
				for (int i = 0; i < MAXX; i++)
				{
					filteredArray[d, j, i] = GetFilteredSum(srcArray, d, i, j);
				}
			}
		}

		return filteredArray;
	}

	/// <summary>
	/// ガウスフィルタ用補助関数
	/// </summary>
	/// <param name="src">元データ</param>
	/// <param name="d">方向</param>
	/// <param name="x">x座標</param>
	/// <param name="y">y座標</param>
	/// <returns>フィルタリング合計値</returns>
	private static double GetFilteredSum(byte[,,] src, int d, int x, int y)
	{
		double result = 0.0;
		for (int j = 0; j < Gaussian.GetLength(0); j++)
		{
			for (int i = 0; i < Gaussian.GetLength(1); i++)
			{
				int pointX = (x * 2) + i;
				int pointY = (y * 2) + j;
				if (pointX < 0 || pointX > src.GetLength(2))
				{
					continue;
				}

				if (pointY < 0 || pointY > src.GetLength(1))
				{
					continue;
				}

				result += src[d, pointY, pointX] * Gaussian[j, i];
			}
		}

		return result;
	}
}