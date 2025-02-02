namespace CharacomOnline.Enums;

public enum ProfileMenuItemType
{
  EditProfile,
  SelectProject,
  DBLogout,
  DBLogin,
  BoxLogout,
  BoxLogin,
  UserRole,
  Exit,
}

public class ProfileMenuItemTypeHelper
{
  private static readonly Dictionary<ProfileMenuItemType, string> _menuItemTypeNames =
    new()
    {
      { ProfileMenuItemType.EditProfile, "プロフィールの編集" },
      { ProfileMenuItemType.SelectProject, "選択中のプロジェクト" },
      { ProfileMenuItemType.DBLogout, "Characomからログアウト" },
      { ProfileMenuItemType.DBLogin, "CharacomDBにログイン" },
      { ProfileMenuItemType.BoxLogout, "Boxからログアウト" },
      { ProfileMenuItemType.BoxLogin, "Boxにログイン" },
      { ProfileMenuItemType.UserRole, "ユーザー権限" },
      { ProfileMenuItemType.Exit, "アプリを終了" },
    };

  public static string GetString(ProfileMenuItemType itemType)
  {
    return _menuItemTypeNames.TryGetValue(itemType, out var name) ? name : string.Empty;
  }

  // 文字列からEnumを取得する逆辞書
  private static readonly Dictionary<string, ProfileMenuItemType> _stringToEnumMap =
    _menuItemTypeNames.ToDictionary(kv => kv.Value, kv => kv.Key);

  // 文字列からEnumを取得するメソッド
  public static ProfileMenuItemType GetEnum(string value)
  {
    return _stringToEnumMap.TryGetValue(value, out var itemType)
      ? itemType
      : throw new ArgumentException("無効な文字列です", nameof(value));
  }
}
