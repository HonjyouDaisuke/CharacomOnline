using CharacomOnline.Service;
using SkiaSharp;
using System.Drawing;

namespace CharacomOnline.ImageProcessing.WeightDirectionSubProcess;

/// <summary>
/// 正規化クラス
/// </summary>
public static class NormalizeClass
{
	/// <summary>
	/// 大きさの正規化を行う
	/// </summary>
	/// <param name="input">入力画像</param>
	/// <param name="r">R値 初期値:38.0</param>
	/// <returns>大きさの正規化後の画像</returns>
	public static SKBitmap Normalize(SKBitmap input, double r)
	{
		double r_ave, rRave;
		DoublePosition gravity;

		//// キャンバスが真っ白だったらすぐ終了
		if (ImageEffectService.IsWhiteCanvas(input))
		{
			return input;
		}

		SKBitmap bmp = new(input.Width, input.Height);

		DoublePosition center = GetCenter(input);

		gravity = GetGravityPosition(input);
		r_ave = SecondaryMomentCalculation(input, gravity);

		//// r_aveが得られなかったら終了
		if (double.IsNaN(r_ave))
		{
			return input;
		}

		rRave = r_ave / r;

		for (int j = 0; j < input.Height; j++)
		{
			for (int i = 0; i < input.Width; i++)
			{
				Point current = new(i, j);
				bmp.SetPixel(i, j, SetPoint(input, current, center, gravity, rRave));
			}
		}

		return bmp;
	}

	private static DoublePosition GetCenter(SKBitmap input)
	{
		return new DoublePosition(input.Width / 2.0, input.Height / 2.0);
	}

	/// <summary>
	/// 重心を求める
	/// </summary>
	/// <param name="srcBitmap">入力画像</param>
	/// <returns>重心(DoublePosition)</returns>
	private static DoublePosition GetGravityPosition(SKBitmap srcBitmap)
	{
		// 重心を求める
		int sum_aX, sum_aY, sum_b;
		DoublePosition gravity = new();

		sum_aX = 0;
		sum_aY = 0;
		sum_b = 0;
		for (int j = 0; j < srcBitmap.Height; j++)
		{
			for (int i = 0; i < srcBitmap.Width; i++)
			{
				if (srcBitmap.GetPixel(i, j) != SKColors.White)
				{
					sum_aX += i;
					sum_aY += j;
					sum_b++;
				}
			}
		}

		gravity.X = (sum_b != 0) ? (double)((double)sum_aX / (double)sum_b) : 0.0;
		gravity.Y = (sum_b != 0) ? (double)((double)sum_aY / (double)sum_b) : 0.0;

		return gravity;
	}

	/// <summary>
	/// 2次モーメントの算出
	/// </summary>
	/// <param name="srcBitmap">入力画像</param>
	/// <param name="gravity">重心座標(DoublePosition)</param>
	/// <returns>2次モーメント</returns>
	private static double SecondaryMomentCalculation(SKBitmap srcBitmap, DoublePosition gravity)
	{
		int sum;
		double r_ave;
		double sX, sY;

		sum = 0;
		r_ave = 0.0;

		for (int j = 0; j < srcBitmap.Height; j++)
		{
			for (int i = 0; i < srcBitmap.Width; i++)
			{
				if (srcBitmap.GetPixel(i, j) != SKColors.White)
				{
					sum++;
					sX = (double)i - gravity.X;
					sY = (double)j - gravity.Y;
					r_ave += Math.Sqrt(Math.Pow(sX, 2) + Math.Pow(sY, 2));
				}
			}
		}

		r_ave /= (double)sum;
		return r_ave;
	}

	private static SKColor SetPoint(SKBitmap srcBitmap, Point current, DoublePosition center, DoublePosition gravity, double rRave)
	{
		int x = (int)((rRave * ((double)current.X - center.X)) + gravity.X);
		int y = (int)((rRave * ((double)current.Y - center.Y)) + gravity.Y);
		if ((x >= 0) && (x < srcBitmap.Width) && (y >= 0) && (y < srcBitmap.Height))
		{
			if (srcBitmap.GetPixel(current.X, current.Y) != SKColors.White)
			{
				return SKColors.Black;
			}
		}

		return SKColors.White;
	}
}
