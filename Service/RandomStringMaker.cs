public static class RandomStringMaker
{
    public static string makeString(int stringLength)
    {
        if (stringLength < 1 || stringLength > 20)
            return "";
        Guid g = System.Guid.NewGuid();
        return g.ToString("N").Substring(0, stringLength);
    }
}
