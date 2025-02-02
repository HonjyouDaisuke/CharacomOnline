namespace CharacomOnline.Enums;

public enum UserRoleType
{
  Administrator,
  StandardUser,
  Guest,
  Developer,
  InvalidUser,
}

public class UserRoleTypeHelper
{
  private static readonly Dictionary<UserRoleType, string> _userRoleTypes =
    new()
    {
      { UserRoleType.Administrator, "admin" },
      { UserRoleType.StandardUser, "user" },
      { UserRoleType.Guest, "guest" },
      { UserRoleType.Developer, "develop" },
      { UserRoleType.InvalidUser, "invalid" },
    };

  public static string GetString(UserRoleType itemType)
  {
    return _userRoleTypes.TryGetValue(itemType, out var name) ? name : string.Empty;
  }

  // 文字列からEnumを取得する逆辞書
  private static readonly Dictionary<string, UserRoleType> _stringToEnumMap =
    _userRoleTypes.ToDictionary(kv => kv.Value, kv => kv.Key);

  // 文字列からEnumを取得するメソッド
  public static UserRoleType GetEnum(string value)
  {
    return _stringToEnumMap.TryGetValue(value, out var itemType)
      ? itemType
      : throw new ArgumentException("無効な文字列です", nameof(value));
  }

  public static IEnumerable<string> GetAllStrings()
  {
    return _userRoleTypes.Values;
  }
}
