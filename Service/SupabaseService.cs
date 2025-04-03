using System.Runtime.CompilerServices;
using Blazored.LocalStorage;
using Supabase;

namespace CharacomOnline.Service;

public class SupabaseService
{
  private readonly Supabase.Client _supabaseClient;
  private readonly AppSettings _appSettings;
  private readonly ILocalStorageService _localStorage;

  public SupabaseService(AppSettings appSettings, ILocalStorageService localStorage)
  {
    // SupabaseのURLとanonキーを使ってクライアントを初期化
    _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));

    // SupabaseのURLとanonキーを使ってクライアントを初期化
    var supabaseUrl = _appSettings.SUPABASE_URL;
    var anonKey = _appSettings.ANON_KEY;

    var options = new SupabaseOptions { AutoConnectRealtime = false };

    _supabaseClient = new Supabase.Client(supabaseUrl, anonKey, options);
  }

  public string SupabaseUrl => _appSettings.SUPABASE_URL;
  public string AnonKey => _appSettings.ANON_KEY;

  // public Supabase.Client GetClient() => _supabaseClient;
  public async Task<Supabase.Client> GetClientAsync()
  {
    var accessToken = await _localStorage.GetItemAsync<string>("access_token");
    var refreshToken = await _localStorage.GetItemAsync<string>("refresh_token");

    if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
    {
      try
      {
        await _supabaseClient.Auth.SetSession(accessToken, refreshToken);
        var session = _supabaseClient.Auth.CurrentSession;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error during SetSession: {ex.Message}");
      }
    }

    return _supabaseClient;
  }

  public string? GetUserProfilePicture()
  {
    // 現在の認証済みユーザーを取得
    var user = _supabaseClient.Auth.CurrentUser;
    Console.WriteLine($"ユーザ情報取得 {user?.Email}");

    if (user == null)
      return null;
    // ユーザーのメタデータからプロフィール画像を取得
    Console.WriteLine($"meta = {user.UserMetadata.ToString()}");
    var pictureUrl = user.UserMetadata?["picture"]?.ToString();
    return pictureUrl;
  }
}
