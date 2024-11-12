namespace CharacomOnline.ImageProcessing.WeightDirectionSubProcess;

public class Position(int x, int y)
{
	public int X { get; set; } = x;
	public int Y { get; set; } = y;

	public string GetString()
	{
		return $"({this.X}, {this.Y})";
	}
}

/// <summary>
/// 輪郭線追跡クラス
/// </summary>
public static class BorderlineTrackingClass
{
	private const int DIRECTIONS = 16;

	private static readonly int[,] DirectionPoint =
	{
		{ -1, -1, 0, 1, 1, 1, 0, -1 },
		{ 0, 1, 1, 1, 0, -1, -1, -1 },
	};

	private static readonly int[,] Directions16 =
	{
		{ 14, 13, 12, 11, 10 },
		{ 15, 14, 12, 10, 9 },
		{ 0, 0, 99, 8, 8 },
		{ 1, 2, 4, 6, 7 },
		{ 2, 3, 4, 5, 6 },
	};

	public static byte[,,] BorderlineTracking(byte[,] srcArray)
	{
		byte[,,] tracked = new byte[DIRECTIONS, srcArray.GetLength(0), srcArray.GetLength(1)];
		//OutlineTracking(srcArray, tracked, new Position(2, 1), 0);
		RasterScanProc(srcArray, tracked);
		return tracked;
	}

	private static int GetPrevToNextDirection(Position currentPos, Position prevPos)
	{
		int dx = prevPos.X - currentPos.X;
		int dy = prevPos.Y - currentPos.Y;

		for (int i = 0; i < DirectionPoint.GetLength(1); i++)
		{
			if (dx == DirectionPoint[0, i] && dy == DirectionPoint[1, i])
			{
				return i;
			}
		}

		return 9;
	}

	private static int GetNextDirection(byte[,] image, Position currentPos, int startDirection)
	{
		for (int i = 0; i < DirectionPoint.GetLength(1); i++)
		{
			int nextDirection = (i + startDirection) % 8;
			int nextX = currentPos.X + DirectionPoint[0, nextDirection];
			int nextY = currentPos.Y + DirectionPoint[1, nextDirection];

			// 配列範囲外チェック
			if (nextX < 0 || nextX >= image.GetLength(1) || nextY < 0 || nextY >= image.GetLength(0))
			{
				continue;
			}

			if (image[nextY, nextX] == 0)
			{
				continue;
			}

			return (i + startDirection) % 8;
		}

		//// Console.WriteLine("該当なし");
		return 0;
	}

	private static Position GetNextPosition(Position currentPos, int direction)
	{
		int nextX = currentPos.X + DirectionPoint[0, direction];
		int nextY = currentPos.Y + DirectionPoint[1, direction];

		return new Position(nextX, nextY);
	}

	private static bool IsSamePosition(Position a, Position b)
	{
		if (a.X == b.X && a.Y == b.Y)
		{
			return true;
		}

		return false;
	}

	private static void OutlineTracking(
		byte[,] image,
		byte[,,] track,
		Position startPos,
		int startDirection
	)
	{
		Position currentPos = startPos;
		image[currentPos.Y, currentPos.X] = 7;
		int nextDirection = GetNextDirection(image, currentPos, startDirection);
		Position nextPos = GetNextPosition(currentPos, nextDirection);
		//// Console.WriteLine($"prevPos={prevPos.getString()} currentPos={currentPos.getString()} nextPos={nextPos.getString()}");

		do
		{
			Position prevPos = currentPos;
			currentPos = nextPos;

			startDirection = (GetPrevToNextDirection(currentPos, prevPos) + 1) % 8;
			//// Console.WriteLine($"next search startDir = {startDirection} current = {currentPos.getString()}");
			nextDirection = GetNextDirection(image, currentPos, startDirection);
			nextPos = GetNextPosition(currentPos, nextDirection);
			int direction = Directions16[nextPos.Y - prevPos.Y + 2, nextPos.X - prevPos.X + 2];
			image[currentPos.Y, currentPos.X] = 7;
			if (direction > 15)
			{
				continue;
			}

			track[direction, currentPos.Y, currentPos.X]++;
			//// Console.WriteLine($"prevPos={prevPos.getString()} currentPos={currentPos.getString()} nextDir={nextDirection} nextPos={nextPos.getString()} D:{Direction}");
		} while (!IsSamePosition(currentPos, startPos));
	}

	private static void StartTracking(byte[,] image, byte[,,] track, Position startPos, int direction)
	{
		image[startPos.Y, startPos.X] = 7;
		OutlineTracking(image, track, startPos, direction);
	}

	private static void VerticalCheck(byte[,] image, byte[,,] track, Position p)
	{
		if (p.Y == 0 || p.Y == image.GetLength(0) - 1)
		{
			int direction = (p.Y == 0) ? 7 : 3;
			if (image[p.Y, p.X] == 1)
			{
				StartTracking(image, track, p, direction);
			}
			return;
		}

		if (image[p.Y, p.X] == 0 && image[p.Y - 1, p.X] == 1)
		{
			image[p.Y - 1, p.X] = 7;
			OutlineTracking(image, track, new Position(x: p.X, y: p.Y - 1), 2);
			return;
		}

		if (image[p.Y, p.X] == 1 && image[p.Y - 1, p.X] == 0)
		{
			image[p.Y, p.X] = 7;
			OutlineTracking(image, track, new Position(x: p.X, y: p.Y), 6);
			return;
		}
	}

	private static void HorizontalityCheck(byte[,] image, byte[,,] track, Position p)
	{
		if (p.X == 0 || p.X == image.GetLength(0) - 1)
		{
			int direction = (p.X == 0) ? 1 : 5;
			if (image[p.Y, p.X] == 1)
			{
				StartTracking(image, track, p, direction);
			}
			return;
		}
		if (image[p.Y, p.X] == 0 && image[p.Y, p.X + 1] == 1)
		{
			image[p.Y, p.X + 1] = 7;
			OutlineTracking(image, track, new Position(x: p.X, y: p.Y - 1), 0);
		}

		if (image[p.Y, p.X] == 1 && image[p.Y, p.X + 1] == 0)
		{
			image[p.Y, p.X] = 7;
			OutlineTracking(image, track, new Position(x: p.X, y: p.Y), 4);
		}
	}

	private static void RasterScanProc(byte[,] image, byte[,,] track)
	{
		for (int j = 0; j < image.GetLength(0); j++)
		{
			for (int i = 0; i < image.GetLength(1); i++)
			{
				HorizontalityCheck(image, track, new Position(x: i, y: j));
				VerticalCheck(image, track, new Position(x: i, y: j));
			}
		}
	}

	/**
    void rasta(byte[,] dat3, byte[,,] data2, int ap, int bp)
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
  **/
	private static void Print<T>(T[,] array)
	{
		for (int j = 0; j < array.GetLength(0); j++)
		{
			for (int i = 0; i < array.GetLength(1); i++)
			{
				// 数値型の場合に "00" フォーマットを適用
				if (array[j, i] is IFormattable formattable)
				{
					Console.Write($"{formattable.ToString("00", null)},");
				}
				else
				{
					// 数値型以外の場合は通常の ToString を使用
					Console.Write($"{array[j, i]},");
				}
			}

			Console.WriteLine(string.Empty);
		}
	}
}
