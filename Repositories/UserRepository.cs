using CharacomOnline.Entity;

namespace CharacomOnline.Repositories;

public class UserRepository
{
  UsersTable currentUser = new();

  public UsersTable CurrentUser()
  {
    return currentUser;
  }

  public void SetCurrentUser(UsersTable user)
  {
    currentUser = user;
  }
}
