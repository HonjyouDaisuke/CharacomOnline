using SkiaSharp;

namespace CharacomOnline.ImageProcessing.WeightDirectionSubProcess;

public class NoiseReductionClass
{
	/// <summary>
	/// ノイズ除去をします。
	/// </summary>
	/// <param name="srcBitmap">入力画像（2値画像）</param>
	/// <returns>ノイズ除去後の画像</returns>
	public static SKBitmap NoiseReduction(SKBitmap srcBitmap)
	{
		SKBitmap outBmp = new(srcBitmap.Width, srcBitmap.Height);
		srcBitmap.CopyTo(outBmp);

		for (int j = 1; j < outBmp.Height - 1; j++)
		{
			for (int i = 1; i < outBmp.Width - 1; i++)
			{
				if (IsWhitePixel(outBmp, i, j))
				{
					continue;
				}

				int up = IsWhitePixel(outBmp, i, j - 1) ? 0 : 1;
				int down = IsWhitePixel(outBmp, i, j + 1) ? 0 : 1;
				int left = IsWhitePixel(outBmp, i - 1, j) ? 0 : 1;
				int right = IsWhitePixel(outBmp, i + 1, j) ? 0 : 1;
				if (up + down + left + right == 0)
				{
					outBmp.SetPixel(i, j, SKColors.White);
				}
			}
		}

		for (int j = 2; j < outBmp.Height - 2; j += 5)
		{
			for (int i = 2; i < outBmp.Width - 2; i += 5)
			{
				if (IsWhitePixel(outBmp, i, j))
				{
					continue;
				}

				int sum = CountNonWhitePixels(outBmp, i, j);
				int a = CountNonWhitePixelsInLine(outBmp, i, j, offset: -2, vertical: false);
				int b = CountNonWhitePixelsInLine(outBmp, i, j, offset: 2, vertical: false);
				int c = CountNonWhitePixelsInLine(outBmp, i, j, offset: -2, vertical: true);
				int d = CountNonWhitePixelsInLine(outBmp, i, j, offset: 2, vertical: true);

				if (sum < 10 && a + b + c + d == 0)
				{
					SetAreaWhite(outBmp, i, j);
				}
			}
		}

		return outBmp;
	}

	/// <summary>
	/// 指定したピクセルが白かどうかをチェックします。
	/// </summary>
	private static bool IsWhitePixel(SKBitmap bmp, int x, int y) => bmp.GetPixel(x, y) == SKColors.White;

	/// <summary>
	/// 指定したピクセルの周囲で白以外のピクセル数をカウントします。
	/// </summary>
	private static int CountNonWhitePixels(SKBitmap bmp, int x, int y)
	{
		int count = 0;
		for (int l = -2; l <= 2; l++)
		{
			for (int m = -2; m <= 2; m++)
			{
				if (!IsWhitePixel(bmp, x + m, y + l))
				{
					count++;
				}
			}
		}

		return count;
	}

	/// <summary>
	/// 指定した行または列にある白以外のピクセル数をカウントします。
	/// </summary>
	private static int CountNonWhitePixelsInLine(SKBitmap bmp, int x, int y, int offset, bool vertical)
	{
		int count = 0;
		for (int i = -2; i <= 2; i++)
		{
			int posX = vertical ? x + offset : x + i;
			int posY = vertical ? y + i : y + offset;

			if (!IsWhitePixel(bmp, posX, posY))
			{
				count++;
			}
		}

		return count;
	}

	/// <summary>
	/// 指定した範囲を白に設定します。
	/// </summary>
	private static void SetAreaWhite(SKBitmap bmp, int x, int y)
	{
		for (int l = -2; l <= 2; l++)
		{
			for (int m = -2; m <= 2; m++)
			{
				bmp.SetPixel(x + m, y + l, SKColors.White);
			}
		}
	}
}
