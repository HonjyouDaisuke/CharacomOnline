using CharacomOnline.Service;
using SkiaSharp;

namespace CharacomOnline.ImageProcessing;

public static class WeightedDirectionHistogramAnalyzer
{
	public static double[] GenerateWeightedDirectionHistogram(SKBitmap srcBitmap)
	{
		SKBitmap workBitmap = ImageEffectService.ResizeBitmap(srcBitmap, 160, 160);
		byte[,] pixels = ImageEffectService.ImageArrayFromBitmap(workBitmap);

		return [];
	}

	#region 特徴抽出(加重方向指数ヒストグラム特徴)
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
		int i, j, k;

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
		int i, j, k;

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
		int i, j, k;

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
		int i, j, k;

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
		int i, j, k;

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
		int i, j;

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

	static void tuiseki2(byte[,] dat3, byte[,,] data2, int ap, int bp, int x, int y, int bx, int by, int sc, int n)
	{
		int i, j, data, sx, sy;
		int[] px = { -1, -1, 0, 1, 1, 1, 0, -1 };
		int[] py = { 0, 1, 1, 1, 0, -1, -1, -1 };
		sx = x; sy = y;
		bp = 0; ap = 0;
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
				by = y; bx = x;
				y += py[(sc + i) % 8]; x += px[(sc + i) % 8];
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
		int i, j, data, sx, sy, sc, bx, by, check, sum;
		int[] px = { -1, -1, 0, 1, 1, 1, 0, -1 };
		int[] py = { 0, 1, 1, 1, 0, -1, -1, -1 };
		sx = x; sy = y;
		bx = 0; by = 0;
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
					by = y; bx = x;
					y += py[(sc + i) % 8]; x += px[(sc + i) % 8];
					for (j = 0; j < 8; j++)
					{
						if (px[j] == bx - x && py[j] == by - y)
						{
							ap = j;
							data = bp + ap;
							if (sum == 1) data2[data, by - 1, bx - 1]++;
							sum = 1;

						}
					}
					break;
				}
			}
		} while (x != sx || y != sy);
		x = sx; y = sy;
		tuiseki2(dat3, data2, ap, bp, x, y, bx, by, sc, n);
	}
	static void rasta(byte[,] dat3, byte[,,] data2, int ap, int bp)
	{
		int i, j, n = 3;
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

	static void ryousika(byte[,,] data2, byte[,,] data3)
	{
		int i, j, k, m, n;

		for (i = 0; i < 16; i++)
		{
			for (j = 0; j < 16; j++)
			{
				for (k = 0; k < 16; k++)
				{
					for (m = 0; m < 10; m++)
					{
						for (n = 0; n < 10; n++)
						{
							data3[i, j, k] += data2[i, (10 * j) + m, (10 * k) + n];
						}
					}
				}
			}
		}
	}

	static void gaus_fil<Type>(Type[,,] data3, double[,,] data4)
	{
		int m, n, i, j, k;
		double sum = 0.0;
		//Type work;
		double[,] gaus ={{0.00,0.09,0.17,0.09,0.00},
														 {0.09,0.57,1.05,0.57,0.09},
														 {0.17,1.05,1.94,1.05,0.17},
														 {0.09,0.57,1.05,0.57,0.09},
														 {0.00,0.09,0.17,0.09,0.00}};
		System.Diagnostics.Debug.WriteLine(data4.GetLength(0).ToString() + "," + data4.GetLength(1).ToString() + "," + data4.GetLength(2).ToString());
		for (i = 0; i < data4.GetLength(0); i++)
		{
			for (j = 0; j < 8; j++)
			{
				for (k = 0; k < 8; k++)
				{
					sum = 0.0;
					for (m = 0; m < 5; m++)
					{
						for (n = 0; n < 5; n++)
						{
							if (2 * j + m - 2 > 0 && 2 * j + m - 2 < 16 && 2 * k + n - 2 > 0 && 2 * k + n - 2 < 16)
							{
								//work = data3[i,2*j+m-2,2*k+n-2];
								//sum+=data3[i,2*j+m-2,2*k+n-2]*gaus[m,n];
								sum += Convert.ToDouble(data3[i, 2 * j + m - 2, 2 * k + n - 2]) * gaus[m, n];
							}
						}
					}
					data4[i, j, k] = sum;
				}
			}
		}
	}

	static void houkou(double[,,] data4, double[,,] data5)
	{
		int i, j, k;

		for (i = 0; i < 16; i += 2)
		{
			for (j = 0; j < 8; j++)
			{
				for (k = 0; k < 8; k++)
				{
					if (i == 0) data5[i / 2, j, k] = data4[15, j, k] + data4[0, j, k] * 2 + data4[1, j, k];
					else data5[i / 2, j, k] = data4[i - 1, j, k] + data4[i, j, k] * 2 + data4[i + 1, j, k];
				}
			}
		}
	}

	static void douitusi(double[,,] data5, double[,,] data6)
	{
		int i, j, k;

		for (i = 0; i < 8; i++)
		{
			for (j = 0; j < 8; j++)
			{
				for (k = 0; k < 8; k++)
				{
					if (i > 3) data6[i - 4, j, k] += data5[i, j, k];
					else data6[i, j, k] += data5[i, j, k];
				}
			}
		}
	}

	static void kajyu_data(double[,,] data6, double[] nyuuryoku)
	{
		int i, j, k;
		for (i = 0; i < 256; i++)
		{
			nyuuryoku[i] = 0;
		}
		for (i = 0; i < 4; i++)
		{
			for (j = 0; j < 8; j++)
			{
				for (k = 0; k < 8; k++)
				{
					nyuuryoku[i * 64 + j * 8 + k] = data6[i, j, k];
				}
			}
		}
	}

	public static void GetKajyu(Bitmap bmp, double[] kajyu, double[] kajyuView, double R)
	{
		int j, m, n;
		byte[,] data1 = new byte[bmp.Height, bmp.Height];
		byte[,] dat3 = new byte[bmp.Height + 2, bmp.Height + 2];
		byte[,,] data2 = new byte[16, bmp.Width, bmp.Height];
		byte[,,] data3 = new byte[16, 16, 16];
		double[,,] data4 = new double[16, 8, 8];
		double[,,] data5 = new double[8, 8, 8];
		double[,,] data6 = new double[4, 8, 8];
		double maxd, mind, work;
		double R1 = 10.0;

		Noize(bmp);
		Normalize(bmp, R);
		data1 = GetArrayFromBmp(bmp);
		data2Syoki(data2);
		Data3Initialize(data3);
		data4Syoki(data4);
		data5Syoki(data5);
		data6Syoki(data6);

		syokika(dat3, data1);
		rasta(dat3, data2, ap, bp);
		ryousika(data2, data3);
		gaus_fil(data3, data4);
		houkou(data4, data5);
		douitusi(data5, data6);

		kajyu_data(data6, kajyu);
		maxd = 0.0;
		mind = 10000.0;
		for (j = 0; j < 4; j++)
		{
			for (m = 0; m < 8; m++)
			{
				for (n = 0; n < 8; n++)
				{
					if (maxd < data6[j, m, n]) maxd = data6[j, m, n];
					if (mind > data6[j, m, n]) mind = data6[j, m, n];
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
	#endregion
}
