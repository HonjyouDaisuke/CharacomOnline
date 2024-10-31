namespace CharacomOnline.Service;

public static class RandomStringMaker
{
	public static string MakeString(int stringLength)
	{
		if (stringLength < 1 || stringLength > 20)
		{
			return string.Empty;
		}

		Guid g = Guid.NewGuid();
		return g.ToString("N")[..stringLength];
	}
}
