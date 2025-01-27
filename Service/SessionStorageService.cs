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
    //Console.WriteLine($"SessionStorage UserId = {result.Value}");
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

  public async Task SetUserPicture(string? pictureUrl)
  {
    if (pictureUrl == null)
      return;
    await _sessionStorage.SetAsync("PictureUrl", pictureUrl);
  }

  public async Task<string?> GetUserPicture()
  {
    var result = await _sessionStorage.GetAsync<string?>("PictureUrl");
    return result.Success ? result.Value : null;
  }

  public async Task SetTopFolderId(string folderId)
  {
    await _sessionStorage.SetAsync("TopFolderID", folderId);
  }

  public async Task<string?> GetTopFolderId()
  {
    var result = await _sessionStorage.GetAsync<string>("TopFolderID");
    return result.Success ? result.Value : null;
  }

  public async Task SetStandardFolderId(string folderId)
  {
    await _sessionStorage.SetAsync("StandardFolderID", folderId);
  }

  public async Task<string?> GetStandardFolderId()
  {
    var result = await _sessionStorage.GetAsync<string>("StandardFolderID");
    return result.Success ? result.Value : null;
  }

  public async Task SetStrokeFolderId(string folderId)
  {
    await _sessionStorage.SetAsync("StrokeFolderID", folderId);
  }

  public async Task<string?> GetStrokeFolderId()
  {
    var result = await _sessionStorage.GetAsync<string>("StrokeFolderID");
    return result.Success ? result.Value : null;
  }

  public async Task ClearAsync()
  {
    await _sessionStorage.DeleteAsync("UserID");
    await _sessionStorage.DeleteAsync("ProjectID");
    await _sessionStorage.DeleteAsync("TopFolderID");
    await _sessionStorage.DeleteAsync("StandardFolderID");
    await _sessionStorage.DeleteAsync("StrokeFolderID");
  }

  public async Task<bool> IsLoggedInAsync()
  {
    var userId = await GetUserIDAsync();
    if (userId == null)
      return false;
    return true;
  }

  public async Task<bool> IsSelectedProjectAsync()
  {
    var projectId = await GetProjectIDAsync();
    if (projectId == null)
      return false;
    return true;
  }

  public async Task<bool> IsLoginAsync()
  {
    return await GetUserIDAsync() != null;
  }

  public async Task SaveAppStateAsync(AppState appState)
  {
    await _sessionStorage.SetAsync("CurrentProjectName", appState.CurrentProjectName);
    await _sessionStorage.SetAsync("CurrentProjectId", appState.CurrentProjectId);
    await _sessionStorage.SetAsync("UserId", appState.UserId);
    await _sessionStorage.SetAsync("UserName", appState.UserName);
    await _sessionStorage.SetAsync("UserRole", appState.UserRole);
    await _sessionStorage.SetAsync("UserPictureUrl", appState.UserPictureUrl);
  }

  public async Task<AppState> LoadAppStateAsync()
  {
    var appState = new AppState
    {
      CurrentProjectName = (await _sessionStorage.GetAsync<string>("CurrentProjectName")).Value,
      CurrentProjectId = (await _sessionStorage.GetAsync<Guid?>("CurrentProjectId")).Value,
      UserId = (await _sessionStorage.GetAsync<Guid?>("UserId")).Value,
      UserName = (await _sessionStorage.GetAsync<string>("UserName")).Value,
      UserRole = (await _sessionStorage.GetAsync<string>("UserRole")).Value,
      UserPictureUrl = (await _sessionStorage.GetAsync<string>("UserPictureUrl")).Value,
    };
    return appState;
  }
}
