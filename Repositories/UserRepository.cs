using System.Collections.ObjectModel;
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
}
