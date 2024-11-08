using CharacomOnline.Service;
using SkiaSharp;

namespace CharacomOnline.ImageProcessing;

public static class WeightedDirectionHistogramAnalyzer
{
  

  public static async Task<double?[]> GenerateWeightedDirectionHistogram(SKBitmap srcBitmap)
  {
    SKBitmap workBitmap = ImageEffectService.ResizeBitmap(srcBitmap, 160, 160);
    if (workBitmap == null)
    {
      return [];
    }

    SKBitmap MonoBitmap = await ImageEffectService.GetBinaryBitmap(workBitmap);
    byte[,] pixels = ImageEffectService.ImageArrayFromBitmap(workBitmap);

    return [];
  }

  private static byte[] DataInitialize(byte[] data)
  {
    int i;

    for (i = 0; i < 160 * 20; i++)
    {
      data[i] = 0;
    }

    return data;
  }

  private static byte[,,] Data2Initialize(byte[,,] data2)
  {
    int i,
      j,
      k;

    for (k = 0; k < 16; k++)
    {
      for (j = 0; j < 160; j++)
      {
        for (i = 0; i < 160; i++)
        {
          data2[k, j, i] = 0;
        }
      }
    }

    return data2;
  }

  private static byte[,,] Data3Initialize(byte[,,] data3)
  {
    int i,
      j,
      k;

    for (k = 0; k < 16; k++)
    {
      for (j = 0; j < 16; j++)
      {
        for (i = 0; i < 16; i++)
        {
          data3[k, j, i] = 0;
        }
      }
    }

    return data3;
  }

  static void data4Syoki(double[,,] data4)
  {
    int i,
      j,
      k;

    for (k = 0; k < 16; k++)
    {
      for (j = 0; j < 8; j++)
      {
        for (i = 0; i < 8; i++)
        {
          data4[k, j, i] = 0.0;
        }
      }
    }
  }

  static void data5Syoki(double[,,] data5)
  {
    int i,
      j,
      k;

    for (k = 0; k < 8; k++)
    {
      for (j = 0; j < 8; j++)
      {
        for (i = 0; i < 8; i++)
        {
          data5[k, j, i] = 0.0;
        }
      }
    }
  }

  static void data6Syoki(double[,,] data6)
  {
    int i,
      j,
      k;

    for (k = 0; k < 4; k++)
    {
      for (j = 0; j < 8; j++)
      {
        for (i = 0; i < 8; i++)
        {
          data6[k, j, i] = 0.0;
        }
      }
    }
  }

  static void syokika(byte[,] dat3, byte[,] dat2)
  {
    int i,
      j;

    for (i = 0; i < 162; i++)
    {
      for (j = 0; j < 162; j++)
      {
        dat3[i, j] = 0;
      }
    }
    for (i = 0; i < 160; i++)
    {
      for (j = 0; j < 160; j++)
      {
        dat3[i + 1, j + 1] = dat2[i, j];
      }
    }
  }

  static void tuiseki2(
    byte[,] dat3,
    byte[,,] data2,
    int ap,
    int bp,
    int x,
    int y,
    int bx,
    int by,
    int sc,
    int n
  )
  {
    int i,
      j,
      data,
      sx,
      sy;
    int[] px = { -1, -1, 0, 1, 1, 1, 0, -1 };
    int[] py = { 0, 1, 1, 1, 0, -1, -1, -1 };
    sx = x;
    sy = y;
    bp = 0;
    ap = 0;
    for (i = 0; i < 8; i++)
    {
      if (px[i] == bx - x && py[i] == by - y)
      {
        sc = i + 1;
      }
    }
    for (i = 0; i < 8; i++)
    {
      if (px[i] == x - bx && py[i] == y - by)
      {
        bp = (i + 4) % 8;
      }
    }
    for (i = 0; i < 8; i++)
    {
      if (0 != dat3[y + py[(sc + i) % 8], x + px[(sc + i) % 8]])
      {
        dat3[y + py[(sc + i) % 8], x + px[(sc + i) % 8]] = 7;
        by = y;
        bx = x;
        y += py[(sc + i) % 8];
        x += px[(sc + i) % 8];
        for (j = 0; j < 8; j++)
        {
          if (px[j] == bx - x && py[j] == by - y)
          {
            ap = j;
            data = bp + ap;
            data2[data, by - 1, bx - 1]++;
          }
        }
        break;
      }
    }
  }

  static void tuiseki(byte[,] dat3, byte[,,] data2, int ap, int bp, int x, int y, int n, int num)
  {
    int i,
      j,
      data,
      sx,
      sy,
      sc,
      bx,
      by,
      check,
      sum;
    int[] px = { -1, -1, 0, 1, 1, 1, 0, -1 };
    int[] py = { 0, 1, 1, 1, 0, -1, -1, -1 };
    sx = x;
    sy = y;
    bx = 0;
    by = 0;
    bp = 10;
    sum = 0;
    sc = num;
    check = 0;
    do
    {
      if (check != 0)
      {
        for (i = 0; i < 8; i++)
        {
          if (px[i] == bx - x && py[i] == by - y)
          {
            sc = i + 1;
          }
        }
        for (i = 0; i < 8; i++)
        {
          if (px[i] == x - bx && py[i] == y - by)
          {
            bp = (i + 4) % 8;
          }
        }
      }
      check = 1;
      for (i = 0; i < 8; i++)
      {
        if (0 != dat3[y + py[(sc + i) % 8], x + px[(sc + i) % 8]])
        {
          dat3[y + py[(sc + i) % 8], x + px[(sc + i) % 8]] = 7;
          by = y;
          bx = x;
          y += py[(sc + i) % 8];
          x += px[(sc + i) % 8];
          for (j = 0; j < 8; j++)
          {
            if (px[j] == bx - x && py[j] == by - y)
            {
              ap = j;
              data = bp + ap;
              if (sum == 1)
                data2[data, by - 1, bx - 1]++;
              sum = 1;
            }
          }
          break;
        }
      }
    } while (x != sx || y != sy);
    x = sx;
    y = sy;
    tuiseki2(dat3, data2, ap, bp, x, y, bx, by, sc, n);
  }

	private static void VerticalCheck(byte[,] data3, byte[,,] data2, int ap, int bp)
	{
		if (data3[i, j] == 0) return;
	}
  private static void RasterScan(byte[,] dat3, byte[,,] data2, int ap, int bp)
  {
    int i,
      j,
      n = 3;
    for (i = 0; i < 161; i++)
    {
      for (j = 0; j < 161; j++)
      {
        if (dat3[i, j] == 0 && dat3[i, j + 1] == 1)
        {
          dat3[i, j + 1] = 7;
          tuiseki(dat3, data2, ap, bp, j + 1, i, n, 0);
        }
        if (dat3[i, j] == 1 && dat3[i, j + 1] == 0)
        {
          dat3[i, j] = 7;
          tuiseki(dat3, data2, ap, bp, j, i, n, 4);
        }
      }
    }
    for (j = 161; j > 0; j--)
    {
      for (i = 161; i > 0; i--)
      {
        if (dat3[i, j] == 0 && dat3[i - 1, j] == 1)
        {
          dat3[i - 1, j] = 7;
          tuiseki(dat3, data2, ap, bp, j, i - 1, n, 2);
        }
        if (dat3[i, j] == 1 && dat3[i - 1, j] == 0)
        {
          dat3[i, j] = 7;
          tuiseki(dat3, data2, ap, bp, j, i, n, 6);
        }
      }
    }
  }

  /// <summary>
  ///  加重方向指数ヒストグラム特徴を1次元配列にする
  /// </summary>
  /// <param name="srcArray">元データ(4 x 8 x 8)</param>
  /// <returns>256次元データ</returns>
  private static double[] Unidimensionalization(double[,,] srcArray)
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

  public static void GetKajyu(SKBitmap bmp, double[] kajyu, double[] kajyuView, double R)
  {
    int j,
      m,
      n;
    byte[,] data1 = new byte[bmp.Height, bmp.Height];
    byte[,] dat3 = new byte[bmp.Height + 2, bmp.Height + 2];
    byte[,,] data2 = new byte[16, bmp.Width, bmp.Height];
    byte[,,] data3 = new byte[16, 16, 16];
    double[,,] data4 = new double[16, 8, 8];
    double[,,] data5 = new double[8, 8, 8];
    double[,,] data6 = new double[4, 8, 8];
    double maxd,
      mind,
      work;
    double R1 = 10.0;

    //// Noize(bmp);
    //// Normalize(bmp, R);
    // data1 = GetArrayFromBmp(bmp);
    // data2Syoki(data2);
    // Data3Initialize(data3);
    // data4Syoki(data4);
    // data5Syoki(data5);
    // data6Syoki(data6);

    // syokika(dat3, data1);
    // rasta(dat3, data2, ap, bp);
    // ryousika(data2, data3);
    // gaus_fil(data3, data4);
    // houkou(data4, data5);
    // ContrarianIdentification(data5, data6);

    // kajyu_data(data6, kajyu);
    maxd = 0.0;
    mind = 10000.0;
    for (j = 0; j < 4; j++)
    {
      for (m = 0; m < 8; m++)
      {
        for (n = 0; n < 8; n++)
        {
          if (maxd < data6[j, m, n])
            maxd = data6[j, m, n];
          if (mind > data6[j, m, n])
            mind = data6[j, m, n];
        }
      }
    }
    work = (maxd - mind) / R1;
    for (j = 0; j < 4; j++)
    {
      for (m = 0; m < 8; m++)
      {
        for (n = 0; n < 8; n++)
        {
          data6[j, m, n] = data6[j, m, n] / work;
        }
      }
    }
    kajyu_data(data6, kajyuView);
  }
}
