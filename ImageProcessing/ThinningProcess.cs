using CharacomOnline.Service;
using Microsoft.AspNetCore.SignalR;
using SixLabors.ImageSharp;
using SkiaSharp;

namespace CharacomOnline.ImageProcessing;

public class ThinningProcess(SKBitmap srcBitmap)
{
  private SKBitmap _srcBitmap = srcBitmap;
  private byte[,] srcArray = ImageEffectService.ImageArrayFromBitmap(srcBitmap);
  private int[,] b;
  private int[,] be;

  // 8連結データ配列
  private readonly int[] py = [0, -1, -1, -1, 0, 1, 1, 1, 0];
  private readonly int[] px = [1, 1, 0, -1, -1, -1, 0, 1, 1];

  public async Task<SKBitmap?> ThinBinaryImageAsync(SKBitmap inputBitmap)
  {
    Console.WriteLine("Start--->");
    return await Task.Run(() => ThinBinaryImage());
  }

  public SKBitmap ThinBinaryImage()
  {
    if (srcArray == null)
    {
      return _srcBitmap;
    }

    CloneToWorkArray();

    do
    {
      ProcessThinning();
    } while (UpdateCheck());

    return DrawBitmap();
  }

  private void CloneToWorkArray()
  {
    if (srcArray == null)
    {
      return;
    }

    b = new int[srcArray.GetLength(0) + 2, srcArray.GetLength(1) + 2];
    be = new int[srcArray.GetLength(0) + 2, srcArray.GetLength(1) + 2];

    for (int i = 0; i < srcArray.GetLength(0); i++)
    {
      for (int j = 0; j < srcArray.GetLength(1); j++)
      {
        b[i + 1, j + 1] = srcArray[i, j];
        be[i + 1, j + 1] = srcArray[i, j];
      }
    }
  }

  private void ProcessThinning()
  {
    for (int j = 0; j < b.GetLength(0) - 1; j++)
    {
      for (int i = 0; i < b.GetLength(0) - 1; i++)
      {
        if (b[j + 1, i + 1] == 0)
        {
          continue;
        }

        SSearch(i, j);
      }
    }
  }

  private bool UpdateCheck()
  {
    bool isUpdated = false;

    for (int i = 0; i < b.GetLength(0) + 1; i++)
    {
      for (int j = 0; j < b.GetLength(1) + 1; j++)
      {
        if (b[i + 1, j + 1] != -1)
        {
          continue;
        }

        b[i + 1, j + 1] = 0;
        isUpdated = true;
      }
    }

    return isUpdated;
  }

  private SKBitmap DrawBitmap()
  {
    SKBitmap workBitmap = new(_srcBitmap.Width, _srcBitmap.Height);

    for (int j = 0; j < workBitmap.Height; j++)
    {
      for (int i = 0; i < workBitmap.Width; i++)
      {
        if (b[j + 1, i + 1] == 0)
        {
          continue;
        }

        workBitmap.SetPixel(i, j, SKColors.Black);
      }
    }

    return workBitmap;
  }

  public static SKBitmap? ThinBinaryImage_old(SKBitmap srcBitmap)
  {
    byte[,] src = ImageEffectService.ImageArrayFromBitmap(srcBitmap);
    int[,] b = new int[srcBitmap.Height + 2, srcBitmap.Width + 2];
    int[,] be = new int[srcBitmap.Height + 2, srcBitmap.Width + 2];
    int check,
      m;
    SKBitmap workBitmap = new(srcBitmap.Width, srcBitmap.Height);
    workBitmap = ImageEffectService.WhiteFilledBitmap(workBitmap);
    Console.WriteLine("Start--->");

    for (int i = 0; i < srcBitmap.Height; i++)
    {
      for (int j = 0; j < srcBitmap.Width; j++)
      {
        b[i + 1, j + 1] = src[i, j];
        be[i + 1, j + 1] = src[i, j];
      }
    }

    do
    {
      check = 0;
      for (int i = 0; i < srcBitmap.Height + 1; i++)
      {
        for (int j = 0; j < srcBitmap.Width + 1; j++)
        {
          if (b[i + 1, j + 1] == 0)
          {
            continue;
          }

          SSearch2(b, j + 1, i + 1);
        }
      }

      // 手順2
      m = 0;
      for (int i = 0; i < srcBitmap.Height + 1; i++)
      {
        for (int j = 0; j < srcBitmap.Width + 1; j++)
        {
          if (b[i + 1, j + 1] == -1)
          {
            b[i + 1, j + 1] = 0;
            m++;
            check = 1;
            be[i + 1, j + 1] = 2; // 変更した画素を保存しておく
          }
          else
          {
            be[i + 1, j + 1] = b[i + 1, j + 1];
          }
        }
      }
    } while (check != 0);

    for (int i = 0; i < srcBitmap.Height; i++)
    {
      for (int j = 0; j < srcBitmap.Width; j++)
      {
        if (b[i + 1, j + 1] != 1)
        {
          continue;
        }

        workBitmap.SetPixel(j, i, SKColors.Black);
        if (j + 1 < workBitmap.Width)
        {
          workBitmap.SetPixel(j + 1, i, SKColors.Black);
        }

        if (i + 1 < workBitmap.Height)
        {
          workBitmap.SetPixel(j, i + 1, SKColors.Black);
        }

        if (j + 1 < workBitmap.Width && i + 1 < workBitmap.Height)
        {
          workBitmap.SetPixel(j + 1, i + 1, SKColors.Black);
        }
      }
    }

    return workBitmap;
  }

  private int GetEdgePixels4(int x, int y)
  {
    const int NEIGHBORHOODS = 8;

    int sum = 0;
    for (int i = 0; i < NEIGHBORHOODS; i += 2)
    {
      sum += 1 - Math.Abs(b[y + py[i], x + px[i]]);
    }

    return sum;
  }

  private int GetEdgePixels8(int x, int y, byte[] pixcels)
  {
    const int NEIGHBORHOODS = 8;

    int sum = 0;
    for (int i = 0; i < NEIGHBORHOODS; i++)
    {
      sum += 1 - Math.Abs(b[y + py[i], x + px[i]]);
      pixcels[i] = (b[y + py[i], x + px[i]] == 1) ? (byte)1 : (byte)0;
    }

    return sum;
  }

  private int GetIsolation(byte[] arroundPixels)
  {
    const int NEIGHBORHOODS = 8;

    int sum = 0;
    for (int i = 0; i < NEIGHBORHOODS; i++)
    {
      sum += arroundPixels[i];
    }

    return sum;
  }

  private void GetConnectingPixels(int x, int y, byte[] connectingPixels)
  {
    for (int i = 0; i < connectingPixels.GetLength(0); i++)
    {
      connectingPixels[i] = (Math.Abs(b[y + py[i], x + px[i]]) == 1) ? (byte)1 : (byte)0;
    }
  }

  private int GetConnection(byte[] connectingPixels)
  {
    const int NEIGHBORHOODS = 8;
    int sum = 0;

    for (int i = 0; i < NEIGHBORHOODS; i += 2)
    {
      int a = 1 - Math.Abs(connectingPixels[i]);
      int b = 1 - Math.Abs(connectingPixels[i + 1]);
      int c = 1 - Math.Abs(connectingPixels[i + 2]);
      sum += a - (a * b * c);
    }

    return sum;
  }

	private void DeleteDecision(int x, int y, ....)
	{
		byte[] deleteTry = new byte[9];

		for (int i = 0; i < )
	}
  private void SSearch(int x, int y)
  {
    byte[] arroundPixels = new byte[9];
    byte[] connectingPixels = new byte[9];
    

    int edgePixels4 = 0; // s1
    int edgePixels8 = 0; // s2
    int isoletion = 0; // s3
    int connection = 0; // n1
    int deleteDecision = 0; // n2

    // 境界画素を確認(4近傍)
    edgePixels4 = GetEdgePixels4(x, y);
    // 周囲画素数を計算
    edgePixels8 = GetEdgePixels8(x, y, arroundPixels);
    // 孤立点かどうか
    isoletion = GetIsolation(arroundPixels);
    // 連結性を確認
    GetConnectingPixels(x, y, connectingPixels);
    // Nc8 (p0)を求める
    connection = GetConnection(connectingPixels);
		// 削除判定

  }

  public static void SSearch2(int[,] b, int x, int y)
  {
    byte[] c = new byte[9];
    byte[] cp = new byte[9];
    byte[] xi = new byte[9];

    // 8連結データ配列
    int[] py = [0, -1, -1, -1, 0, 1, 1, 1, 0];
    int[] px = [1, 1, 0, -1, -1, -1, 0, 1, 1];

    int i,
      n2,
      n1,
      s1,
      s2,
      s3,
      k,
      m,
      bx;

    n1 = s1 = s2 = s3 = 0;

    // 条件2：境界画素である条件
    for (i = 0; i < 8; i += 2)
    {
      Console.WriteLine($"i={i} x={x} px[i]={px[i]}  y={y} py[i]={py[i]}");
      s1 += 1 - Math.Abs(b[y + py[i], x + px[i]]);
    }

    // 条件3：端点を削除しない条件
    for (i = 0; i < 8; i++)
    {
      s2 += Math.Abs(b[y + py[i], x + px[i]]);
    }

    // 条件4：孤立点を保存する条件
    // Ckの決定
    for (i = 0; i < 8; i++)
    {
      c[i] = (b[y + py[i], x + px[i]] == 1) ? (byte)1 : (byte)0;
    }

    for (i = 0; i < 8; i++)
    {
      s3 += c[i];
    }

    // 条件5：連結性を保存する条件
    for (i = 0; i < 9; i++)
    {
      cp[i] = (Math.Abs(b[y + py[i], x + px[i]]) == 1) ? (byte)1 : (byte)0;
    }

    // Nc8(p0)を求める
    for (i = 0; i < 8; i += 2)
    {
      n1 +=
        (1 - Math.Abs(cp[i]))
        - ((1 - Math.Abs(cp[i])) * (1 - Math.Abs(cp[i + 1])) * (1 - Math.Abs(cp[i + 2])));
    }

    // 条件6：線幅2に対する片側削除条件

    // 格納
    for (i = 0; i < 9; i++)
    {
      xi[i] = (byte)Math.Abs(b[y + py[i], x + px[i]]);
    }

    m = 0;
    for (i = 0; i < 8; i++)
    {
      n2 = 0;
      bx = xi[i];
      xi[i] = 0; // 0とする

      // Nc8(p0)を求める
      for (k = 0; k < 8; k += 2)
      {
        n2 +=
          1
          - Math.Abs(xi[k])
          - ((1 - Math.Abs(xi[k])) * (1 - Math.Abs(xi[k + 1])) * (1 - Math.Abs(xi[k + 2])));
      }

      // もとにもどす
      xi[i] = (byte)bx;

      if (n2 == 1 || b[y + py[i], x + px[i]] != -1)
      {
        m++;
      }
    }

    // B(i,j)を-1にする(条件1から条件6をすべて満たすもの）
    if (b[y, x] == 1 && s1 >= 1 && s2 >= 2 && s3 >= 1 && n1 == 1 && m == 8)
    {
      b[y, x] = -1;
    }
  }
}
