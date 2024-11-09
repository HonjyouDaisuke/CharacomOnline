namespace CharacomOnline.ImageProcessing;

public class DoublePosition
{
	public DoublePosition()
	{
		X = 0.0;
		Y = 0.0;
	}

	public DoublePosition(double x, double y)
	{
		X = x;
		Y = y;
	}

	public double X { get; set; }
	public double Y { get; set; }

	public override string ToString()
	{
		return $"( {X}, {Y})";
	}
}