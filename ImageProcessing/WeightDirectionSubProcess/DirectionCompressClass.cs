/// <summary>
/// 方向圧縮クラス
/// </summary>
public static class DirectionCompressClass {
  const int DIRECTIONS = 8;
  const int SRC_DIRECTIONS = 16;
  const int MAX_X = 8;
  const int MAX_Y = 8;
  
  /// <summary>
  ///  方向圧縮用補助関数.
  /// </summary>
  /// <param name="src">元データ</param>
  /// <param name="d">方向</param>
  /// <param name="x">x座標</param>
  /// <param name="y">y座標</param>
  /// <returns>圧縮値.</returns>
  private static double GetDirCompress(double[,,] src, int d, int x, int y)
  {
    double sum;

    if (d == 0)
    {
      sum = src[15, y, x] + (src[0, y, x] * 2) + src[1, y, x];
    }
    else
    {
      sum = src[d - 1, y, x] + (src[d, y, x] * 2) + src[d + 1, y, x];
    }

    return sum;
  }
  
  /// <summary>
  ///  方向圧縮（16方向→8方向）
  ///  偶数方向＊２＋両脇方向.
  /// </summary>
  /// <param name="srcArray">追跡後の方向データ</param>
  /// <returns>圧縮された配列</returns>
  public static double[,,] DirectionalCompression(double[,,] srcArray)
  {
    double[,,] compressedArray = new double[DIRECTIONS, MAX_Y, MAX_X];
    
    for (int y = 0; y < MAX_Y; y++)
    {
      for (int x = 0; x < MAX_X; x++)
      {
        for (int d = 0; d < SRC_DIRECTIONS; d += 2)
        {
          compressedArray[d / 2, x, y] = GetDirCompress(srcArray, d, x, y);
        }
      }
    }

    return compressedArray;
  }
}