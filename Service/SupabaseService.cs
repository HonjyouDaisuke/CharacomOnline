﻿using Blazored.LocalStorage;
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

    var options = new SupabaseOptions
    {
      AutoConnectRealtime = false,
    };

    _supabaseClient = new Supabase.Client(supabaseUrl, anonKey, options);
  }

  public string SupabaseUrl => _appSettings.SUPABASE_URL;
  public string AnonKey => _appSettings.ANON_KEY;

  //public Supabase.Client GetClient() => _supabaseClient;
  public async Task<Supabase.Client> GetClientAsync()
  {
    var accessToken = await _localStorage.GetItemAsync<string>("access_token");
    var refreshToken = await _localStorage.GetItemAsync<string>("refresh_token");

    if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
    {
      Console.WriteLine($"Access Token: {accessToken}");
      Console.WriteLine($"Refresh Token: {refreshToken}");

      try
      {
        //_supabaseClient.Auth.SetSession(accessToken, refreshToken);
        var session = _supabaseClient.Auth.CurrentSession;
        Console.WriteLine($"Session after SetSession: {session}");
        if (session?.User != null)
        {
          Console.WriteLine($"User Email: {session.User.Email}");
        }
        else
        {
          Console.WriteLine("Session or User is null after SetSession.");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error during SetSession: {ex.Message}");
      }

    }

    return _supabaseClient;
  }

}
