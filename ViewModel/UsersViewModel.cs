using CharacomOnline.Entity;
using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;

namespace CharacomOnline.ViewModel;

public class UsersViewModel
{
  private readonly UserRepository userRepository;
  private readonly UsersTableService usersTableService;
  private readonly AppState appState;
  private readonly Supabase.Client supabaseClient;

  public UsersViewModel(
    UserRepository _userRepository,
    UsersTableService _usersTableService,
    AppState _appState,
    Supabase.Client _supabaseClient
  )
  {
    userRepository = _userRepository ?? throw new ArgumentNullException(nameof(_userRepository));
    usersTableService =
      _usersTableService ?? throw new ArgumentNullException(nameof(_usersTableService));
    appState = _appState ?? throw new ArgumentNullException(nameof(_appState));
    supabaseClient = _supabaseClient ?? throw new ArgumentNullException(nameof(_supabaseClient));
  }

  public async Task SetCurrentUser(Guid userId)
  {
    UsersTable? user = await usersTableService.GetUserAsync(userId);
    if (user == null)
      return;
    Console.WriteLine($"to repository Url={user.PictureUrl}");
    userRepository.SetCurrentUser(user);
  }

  public string? GetCurrentUserRole()
  {
    Console.WriteLine($"Getします : data->{userRepository.CurrentUser().ToString()}");
    return userRepository.CurrentUser().UserRole;
  }

  public string? GetUserPicture()
  {
    return userRepository.CurrentUser().PictureUrl;
  }

  public string? GetUserName()
  {
    return userRepository.CurrentUser().Name;
  }

  public UsersTable? GetCurrentUserInfo()
  {
    return userRepository.CurrentUser();
  }

  public async Task UpdateCurrentUserAsync(UsersTable user)
  {
    if (user == null)
      return;
    userRepository.SetCurrentUser(user);
    await usersTableService.UpdateUserAsync(user);
  }

  public async Task FetchAllUsersAsync()
  {
    var users = await usersTableService.FetchAllUsersAsync();
    if (users == null)
      return;

    userRepository.ClearAllUser();

    foreach (var user in users)
    {
      if (string.IsNullOrEmpty(user.PictureUrl))
      {
        user.PictureUrl = "/images/no_user_icon.png";
      }
      userRepository.AddUser(user);
    }
  }

  public List<UsersTable> GetAllUserList()
  {
    return userRepository.GetAllUser();
  }

  public async Task UpdateUserRoleAsync(Guid userId, string role)
  {
    var response = userRepository.UpdateUserRole(userId, role);
    if (response == false)
      return;
    await usersTableService.UpdateUserRoleAsync(userId, role);
  }

  public async Task SetUserInfoToAppState()
  {
    var session = supabaseClient.Auth.CurrentSession;
    if (session?.User != null)
    {
      if (Guid.TryParse(session.User.Id, out Guid guid))
      {
        await SetCurrentUser(guid);
        var user = GetCurrentUserInfo();
        appState.UserId = user!.Id;
        appState.UserRole = user.UserRole;
        appState.UserName = user.Name;
        appState.UserPictureUrl = user.PictureUrl;
        appState.IsLoggedIn = true;
        Console.WriteLine($" set AppState UserName = {user.Name}");
      }
      else
      {
        Console.WriteLine("Invalid GUID format.");
      }
    }
  }

  public async Task SetUserNameAndEmailIfNull(Guid userId, string userName, string email)
  {
    var isNameUpdate = await usersTableService.SetNameIfNull(userId, email);
    var isEmailUpdate = await usersTableService.SetEmailIfNull(userId, email);
    if (isNameUpdate)
      Console.WriteLine($"ユーザ名をemailでアップデートしました。{email}");
    if (isEmailUpdate)
      Console.WriteLine($"emailアドレスをアップデートしました。{email}");
  }

  public async Task<bool> ResetPassword(string email)
  {
    Console.WriteLine("リセットするよ");
    if (!await usersTableService.IsExistEmail(email))
      return false;
    if (!await usersTableService.SendPasswordResetEmail(email))
      return false;
    return true;
  }
}
