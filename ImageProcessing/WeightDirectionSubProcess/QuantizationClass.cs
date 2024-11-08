/// <summary>
/// 量子化を行うクラス
/// </summary>
public static class QuantizationClass
{
  private const int COUNT_SIZE = 10;
  private const int DIRECTIONS = 16;
  private const int MAX_X = 16;
  private const int MAX_Y = 16;
    
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

    for (int m = 0; m < COUNT_SIZE; m++)
    {
      for (int n = 0; n < COUNT_SIZE; n++)
      {
        sum += src[d, (COUNT_SIZE * y) + m, (COUNT_SIZE * x) + n];
      }
    }

    return sum;
  }
  /// <summary>
  ///  量子化
  ///  160 x 160 →　10 x 10マスを合計して16x16に圧縮
  /// </summary>
  /// <param name="srcArray">方向線素データ</param>
  /// <returns>量子化後のデータ.</returns>
  public static byte[,,] Quantization(byte[,,] srcArray)
  {
    byte[,,] quantizationData = new byte[DIRECTIONS, MAX_Y, MAX_X];

    for (int d = 0; d < DIRECTIONS; d++)
    {
      for (int y = 0; y < MAX_Y; y++)
      {
        for (int x = 0; x < MAX_X; x++)
        {
          quantizationData[d, y, x] = CountQuantizationFeature(srcArray, d, x, y);
        }
      }
    }

    return quantizationData;
  }
}