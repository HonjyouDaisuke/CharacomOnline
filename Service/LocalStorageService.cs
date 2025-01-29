using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CharacomOnline.Service;

public class LocalStorageService(ProtectedLocalStorage localStorage)
{
  private readonly ProtectedLocalStorage _localStorage = localStorage;

  public async Task SetCurrentProjectIdAsync(Guid projectId)
  {
    await _localStorage.SetAsync("CurrentProjectId", projectId);
  }

  public async Task<Guid?> GetCurrentProjectIdAsync()
  {
    var result = await _localStorage.GetAsync<Guid>("CurrentProjectId");
    return result.Success ? result.Value : null;
  }

  public async Task ClearAsync()
  {
    await _localStorage.DeleteAsync("CurrentProjectId");
  }
}