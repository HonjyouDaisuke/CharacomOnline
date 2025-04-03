using CharacomOnline.Entity;

namespace CharacomOnline.Repositories;

public class UserRepository
{
  UsersTable currentUser = new();
  private List<UsersTable> users = new();

  public UsersTable CurrentUser()
  {
    return currentUser;
  }

  public void SetCurrentUser(UsersTable user)
  {
    currentUser = user;
  }

  public void AddUser(UsersTable user)
  {
    users.Add(user);
  }

  public List<UsersTable> GetAllUser()
  {
    return users;
  }

  public void ClearAllUser()
  {
    users.Clear();
  }

  public bool UpdateUserRole(Guid userId, string newRole)
  {
    // 指定した userId を持つユーザーを検索
    var user = users.FirstOrDefault(u => u.Id == userId);
    if (user?.Id == currentUser.Id)
    {
      currentUser.UserRole = newRole;
    }

    if (user != null)
    {
      user.UserRole = newRole; // Role を更新
      return true; // 更新成功
    }

    return false; // 更新失敗 (該当ユーザーが見つからない)
  }
}
