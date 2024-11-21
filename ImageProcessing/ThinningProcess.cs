using System.Security.Cryptography.Xml;
using CharacomOnline.Service;
using SkiaSharp;

namespace CharacomOnline.ImageProcessing;

public class ThinningProcess(SKBitmap srcBitmap)
{
  // 8連結データ配列
  private readonly int[] py = [0, -1, -1, -1, 0, 1, 1, 1, 0];
  private readonly int[] px = [1, 1, 0, -1, -1, -1, 0, 1, 1];
  private readonly SKBitmap _srcBitmap = srcBitmap;
  private readonly byte[,] srcArray = ImageEffectService.ImageArrayFromBitmap(srcBitmap);
  private int[,] b = new int[1, 1];

  /// <summary>
  /// 細線化処理
  /// </summary>
  /// <returns>処理画像</returns>
  public async Task<SKBitmap?> ThinBinaryImageAsync()
  {
    return await Task.Run(() => ThinBinaryImage());
  }

  /// <summary>
  /// 画像を細線化する
  /// </summary>
  /// <returns>細線化後の画像(SKBitmap)</returns>
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

  /// <summary>
  /// 画像配列のクローンを作成
  /// </summary>
  private void CloneToWorkArray()
  {
    if (srcArray == null)
    {
      return;
    }

    b = new int[srcArray.GetLength(0) + 2, srcArray.GetLength(1) + 2];

    for (int i = 0; i < srcArray.GetLength(0); i++)
    {
      for (int j = 0; j < srcArray.GetLength(1); j++)
      {
        b[i + 1, j + 1] = srcArray[i, j];
      }
    }
  }

  /// <summary>
  /// 細線化メインルーチン
  /// </summary>
  private void ProcessThinning()
  {
    for (int j = 0; j < _srcBitmap.Height + 1; j++)
    {
      for (int i = 0; i < _srcBitmap.Width + 1; i++)
      {
        if (b[j + 1, i + 1] == 0)
        {
          continue;
        }

        SSearch(i + 1, j + 1);
      }
    }
  }

  /// <summary>
  /// 画素の削除
  /// </summary>
  /// <returns>削除したかどうか</returns>
  private bool UpdateCheck()
  {
    bool isUpdated = false;

    for (int i = 0; i < _srcBitmap.Height + 1; i++)
    {
      for (int j = 0; j < _srcBitmap.Width + 1; j++)
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

  /// <summary>
  /// 画像配列をビットマップ(SKBitmap)に変換
  /// </summary>
  /// <returns>変換後の画像(SKBitmap)</returns>
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

  /// <summary>
  /// 細線化する場所のサーチ
  /// </summary>
  /// <param name="x">x座標</param>
  /// <param name="y">y座標</param>
  private void SSearch(int x, int y)
  {
    bool c2 = Condition2EdgePixel(x, y);
    bool c3 = Condition3Endpoint(x, y);
    bool c4 = Condition4Isolation(x, y);
    bool c5 = Condition5Connectivity(x, y);
    bool c6 = Condition6Linewidth(x, y);

    if (b[y, x] == 1 & c2 & c3 & c4 & c5 & c6)
    {
      b[y, x] = -1;
    }
  }

  /// <summary>
  /// 条件2 境界画素である条件
  /// </summary>
  /// <param name="x">x座標</param>
  /// <param name="y">y座標</param>
  /// <returns>削除可能かどうか</returns>
  private bool Condition2EdgePixel(int x, int y)
  {
    int result = 0;
    for (int i = 0; i < 8; i += 2)
    {
      result += 1 - Math.Abs(b[y + py[i], x + px[i]]);
    }

    return result >= 1;
  }

  /// <summary>
  /// 条件3 端点を削除しない条件
  /// </summary>
  /// <param name="x">x座標</param>
  /// <param name="y">y座標</param>
  /// <returns>削除可能かどうか</returns>
  private bool Condition3Endpoint(int x, int y)
  {
    int result = 0;
    for (int i = 0; i < 8; i++)
    {
      result += Math.Abs(b[y + py[i], x + px[i]]);
    }

    return result >= 2;
  }

  /// <summary>
  /// 条件4 孤立点を保存する条件
  /// </summary>
  /// <param name="x">x座標</param>
  /// <param name="y">y座標</param>
  /// <returns>削除可能かどうか</returns>
  private bool Condition4Isolation(int x, int y)
  {
    int result = 0;
    byte[] c = new byte[8];

    for (int i = 0; i < 8; i++)
    {
      c[i] = (b[y + py[i], x + px[i]] == 1) ? (byte)1 : (byte)0;
    }

    for (int i = 0; i < 8; i++)
    {
      result += c[i];
    }

    return result >= 1;
  }

  /// <summary>
  /// 条件5 連結性を保存する条件
  /// </summary>
  /// <param name="x">x座標</param>
  /// <param name="y">y座標</param>
  /// <returns>削除可能かどうか</returns>
  private bool Condition5Connectivity(int x, int y)
  {
    int result = 0;
    int[] cp = new int[9];

    for (int i = 0; i < 9; i++)
    {
      cp[i] = (Math.Abs(b[y + py[i], x + px[i]]) == 1) ? (byte)1 : (byte)0;
    }

    // Nc8(p0)を求める
    for (int i = 0; i < 8; i += 2)
    {
      int a = 1 - Math.Abs(cp[i]);
      int b = 1 - Math.Abs(cp[i + 1]);
      int c = 1 - Math.Abs(cp[i + 2]);
      result += a - (a * b * c);
    }

    return result == 1;
  }

  /// <summary>
  /// 条件6 線幅2に対する片側削除条件
  /// </summary>
  /// <param name="x">x座標</param>
  /// <param name="y">y座標</param>
  /// <returns>削除可能かどうか</returns>
  private bool Condition6Linewidth(int x, int y)
  {
    int result = 0;
    int[] xi = new int[9];

    // 格納
    for (int i = 0; i < 9; i++)
    {
      xi[i] = Math.Abs(b[y + py[i], x + px[i]]);
    }

    for (int i = 0; i < 8; i++)
    {
      int n2 = 0;
      int bx = xi[i];
      xi[i] = 0; // 0とする

      // Nc8(p0)を求める
      for (int k = 0; k < 8; k += 2)
      {
        int a = 1 - Math.Abs(xi[k]);
        int b = 1 - Math.Abs(xi[k + 1]);
        int c = 1 - Math.Abs(xi[k + 2]);
        n2 += a - (a * b * c);
      }

      // もとにもどす
      xi[i] = bx;

      if (n2 == 1 || b[y + py[i], x + px[i]] != -1)
      {
        result++;
      }
    }

    return result == 8;
  }
}
