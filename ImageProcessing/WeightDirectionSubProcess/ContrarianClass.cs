namespace CharacomOnline.ImageProcessing.WeightDirectionSubProcess;

/// <summary>
/// 反対方向同一視クラス
/// </summary>
public static class ContrarianClass
{
	private const int DIRECTIONS = 8;
	private const int MAXX = 8;
	private const int MAXY = 8;

	/// <summary>
	/// 	反対方向同一視.
	/// </summary>
	/// <param name="srcArray">入力側配列</param>
	/// <returns>同一視後の配列</returns>
	public static double[,,] ContrarianIdentification(double[,,] srcArray)
	{
		double[,,] contraViewArray = new double[DIRECTIONS / 2, MAXY, MAXX];

		for (int direction = 0; direction < DIRECTIONS; direction++)
		{
			for (int y = 0; y < MAXY; y++)
			{
				for (int x = 0; x < MAXX; x++)
				{
					contraViewArray[direction - ((int)(direction / 4) * 4), y, x] += srcArray[
						direction,
						y,
						x
					];
				}
			}
		}

		return contraViewArray;
	}
}