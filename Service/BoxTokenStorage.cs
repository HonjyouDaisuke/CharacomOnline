using Microsoft.JSInterop;

namespace CharacomOnline.Service;
public class TokenStorage
{
  private readonly IJSRuntime _jsRuntime;

  public TokenStorage(IJSRuntime jsRuntime)
  {
    _jsRuntime = jsRuntime;
  }

  public async Task SaveTokenAsync(OAuthTokenResponse tokenResponse)
  {
    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", tokenResponse.AccessToken);
    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", tokenResponse.RefreshToken);
  }

  public async Task<string?> GetAccessTokenAsync()
  {
    return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
  }
}
