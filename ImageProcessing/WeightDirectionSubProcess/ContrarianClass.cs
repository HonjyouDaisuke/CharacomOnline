/// <summary>
/// 反対方向同一視クラス
/// </summary>
public static class ContrarainClass {
  private const int DIRECTIONS = 8;
  private const int MAX_X = 8;
  private const int MAX_Y = 8;
  /// <summary>
  /// 	反対方向同一視.
  /// </summary>
  /// <param name="srcArray">入力側配列</param>
  public static double[,,] ContrarianIdentification(double[,,] srcArray)
  {
    double[,,] contraViewArray = new double[DIRECTIONS / 2, MAX_Y, MAX_X];

    for (int direction = 0; direction < DIRECTIONS; direction++)
    {
      for (int y = 0; y < MAX_Y; y++)
      {
        for (int x = 0; x < MAX_X; x++)
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