using System.Text.RegularExpressions;

namespace CharacomOnline.Service;

public struct FileInformation
{
  public string CharaName { get; set; }

  public string MaterialName { get; set; }

  public string TimesName { get; set; }
}

public static class FileNameService
{
  public static string? GetExtension(string fileName)
  {
    return Path.GetExtension(fileName);
  }

  public static FileInformation? GetDataInfo(string fileName)
  {
    FileInformation resData = new();

    // 正規表現で分割
    var match = Regex.Match(fileName, @"^(.*?)_(.*?)-(.*?)\.(jpg|jpeg)$", RegexOptions.IgnoreCase);
    if (match.Success)
    {
      resData.CharaName = match.Groups[1].Value;
      resData.MaterialName = match.Groups[2].Value;
      resData.TimesName = match.Groups[3].Value;
    }
    else
    {
      Console.WriteLine("ファイル名の形式が正しくありません。");
      return null;
    }

    if (string.IsNullOrEmpty(resData.CharaName))
      return null;
    if (string.IsNullOrEmpty(resData.MaterialName))
      return null;
    if (string.IsNullOrEmpty(resData.TimesName))
      return null;
    return resData;
  }
}
