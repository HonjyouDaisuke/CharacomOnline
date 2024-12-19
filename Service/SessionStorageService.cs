namespace CharacomOnline.Service;

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public class SessionStorageService(ProtectedSessionStorage sessionStorage)
{
  private readonly ProtectedSessionStorage _sessionStorage = sessionStorage;

  public async Task SetUserIDAsync(Guid userID)
  {
    await _sessionStorage.SetAsync("UserID", userID);
  }

  public async Task<Guid?> GetUserIDAsync()
  {
    var result = await _sessionStorage.GetAsync<Guid>("UserID");
    return result.Success ? result.Value : null;
  }

  public async Task SetProjectIDAsync(Guid projectID)
  {
    await _sessionStorage.SetAsync("ProjectID", projectID);
  }

  public async Task<Guid?> GetProjectIDAsync()
  {
    var result = await _sessionStorage.GetAsync<Guid>("ProjectID");
    return result.Success ? result.Value : null;
  }

  public async Task ClearAsync()
  {
    await _sessionStorage.DeleteAsync("UserID");
    await _sessionStorage.DeleteAsync("ProjectID");
  }
}
