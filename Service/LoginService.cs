using System.Threading.Tasks;

namespace CharacomOnline.Service;

public class LoginService
{
  private AppState appState;
  private LocalStorageService localStorageService;
  private SupabaseService supabaseService;

  public LoginService(
    AppState _appState,
    SupabaseService _supabaseService,
    LocalStorageService _localStorageService
  )
  {
    appState = _appState;
    supabaseService = _supabaseService;
    localStorageService = _localStorageService;
  }

  public async Task LoadLocalStorage()
  {
    if (localStorageService == null)
      return;
    var state = await localStorageService.LoadAppStateAsync();
    if (state == null)
      return;
    appState.AppStateData = state;
  }

  public bool IsSupabaseLoggedIn()
  {
    return supabaseService.IsSupabaseLogin();
  }

  public async Task<bool> TryRestoreLoginAsync(string accessToken, string refreshToken)
  {
    try
    {
      if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        return false;

      // セッション復元を試みる
      var isRefresh = await supabaseService.RefreshSessionAsync(accessToken, refreshToken);
      return isRefresh;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"[Login Restore Error] {ex.Message}");
    }
    return false;
  }

  public async Task<bool> SupabaseLoginTest()
  {
    if (supabaseService.IsSupabaseLogin())
    {
      Console.WriteLine($"すでにログイン済みです・・・・ {appState.AppStateData.ToString()}");
      return true;
    }

    if (
      appState.AppStateData.SupabaseAccessToken == null
      || appState.AppStateData.SupabaseRefreshToken == null
    )
    {
      await LoadLocalStorage();
      Console.WriteLine("＞＞＞ローカルストレージを読み込みました。");
      Console.WriteLine($"appState = {appState.AppStateData.ToString()}");
    }

    if (
      appState.AppStateData.SupabaseAccessToken == null
      || appState.AppStateData.SupabaseRefreshToken == null
    )
    {
      Console.WriteLine("accessToken or refeshTokenが空っぽです。");
      return false;
    }

    await supabaseService.GetClientAsync(
      appState.AppStateData.SupabaseAccessToken,
      appState.AppStateData.SupabaseRefreshToken
    );
    Console.WriteLine("supabase二ログインしました？");
    return true;
  }

  public async Task<string?> LoginProc()
  {
    if (!await SupabaseLoginTest())
    {
      return "/login";
    }
    return null;
  }
}
