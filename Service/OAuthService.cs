using System.Text.Json;
using System.Text.Json.Serialization;

namespace CharacomOnline.Service;

public class OAuthService
{
  private readonly HttpClient _httpClient;
  private readonly string _clientId;
  private readonly string _clientSecret;
  private bool _isLogin = false;

  public OAuthService(HttpClient httpClient, string clientId, string clientSecret)
  {
    _httpClient = httpClient;
    _clientId = clientId;
    _clientSecret = clientSecret;
  }

  public string GetBoxClientId()
  {
    return _clientId;
  }
  // 1. 初回のアクセストークン取得
  public async Task<OAuthTokenResponse> GetAccessTokenAsync(string code)
  {
    var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "redirect_uri", "https://localhost:7241/box-callback" } // 必ず設定されているリダイレクトURIを使用
        };

    var requestContent = new FormUrlEncodedContent(requestBody);
    // var response = await _httpClient.PostAsync("token", requestContent);  // BaseAddress + "token" で完全なURLになる
    var response = await _httpClient.PostAsync("https://api.box.com/oauth2/token", requestContent);


    if (response.IsSuccessStatusCode)
    {
      Console.WriteLine("Success!!");
      var responseContent = await response.Content.ReadAsStringAsync();
      Console.WriteLine($"responseContent = [{responseContent}]");
      var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(responseContent);

      if (tokenResponse != null)
      {
        _isLogin = true;
        return tokenResponse;
      }

      throw new Exception("Failed to deserialize token response.");
      // return JsonSerializer.Deserialize<OAuthTokenResponse>(responseContent);
    }
    else
    {
      Console.WriteLine("failed...!!");
      var errorContent = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Error during token request: {response.StatusCode}, {errorContent}");
    }
  }

  public bool IsLogin() { return _isLogin; }

  // 2. アクセストークンのリフレッシュ
  public async Task<OAuthTokenResponse> RefreshAccessTokenAsync(string refreshToken)
  {
    var content = new FormUrlEncodedContent(new[]
    {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret)
        });

    var response = await _httpClient.PostAsync("https://api.box.com/oauth2/token", content);
    response.EnsureSuccessStatusCode();

    var jsonResponse = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<OAuthTokenResponse>(jsonResponse)
           ?? throw new Exception("Failed to deserialize OAuth response.");
  }
}

// OAuthトークンレスポンスのモデルクラス
public class OAuthTokenResponse
{
  [JsonPropertyName("access_token")]
  public string AccessToken { get; set; } = string.Empty;

  [JsonPropertyName("refresh_token")]
  public string RefreshToken { get; set; } = string.Empty;

  [JsonPropertyName("expires_in")]
  public int ExpiresIn { get; set; } // 有効期限（秒）
}

