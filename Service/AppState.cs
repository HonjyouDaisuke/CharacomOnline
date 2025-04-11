using System.ComponentModel;
using CharacomOnline.Entity;

namespace CharacomOnline.Service;

public class AppState : INotifyPropertyChanged
{
  private readonly SessionStorageService sessionStorage;
  private readonly LocalStorageService localStorage;
  private bool _isLoggedIn = false;
  private string? _currentProjectName;
  private Guid? _currentProjectId;
  private string? _userName;
  private string? _userPictureUrl;
  private string? _userRole;
  private Guid? _userId;
  public AppStateData AppStateData = new();

  public AppState(LocalStorageService _localStorage, SessionStorageService _sessionStorage)
  {
    this.sessionStorage = _sessionStorage;
    this.localStorage = _localStorage;
  }

  public async Task SaveAppStateDataToLocalStorage()
  {
    await localStorage.SaveAppStateAsync(AppStateData);
  }

  public bool IsLoggedIn
  {
    get => _isLoggedIn;
    set
    {
      if (_isLoggedIn != value)
      {
        _isLoggedIn = value;
        OnPropertyChanged(nameof(_isLoggedIn));
      }
    }
  }

  public string? CurrentProjectName
  {
    get => _currentProjectName;
    set
    {
      if (_currentProjectName != value)
      {
        _currentProjectName = value;
        OnPropertyChanged(nameof(CurrentProjectName));
      }
    }
  }

  public Guid? CurrentProjectId
  {
    get => _currentProjectId;
    set
    {
      if (_currentProjectId != value)
      {
        _currentProjectId = value;
        OnPropertyChanged(nameof(CurrentProjectId));
      }
    }
  }

  public Guid? UserId
  {
    get => _userId;
    set
    {
      if (_userId != value)
      {
        _userId = value;
        OnPropertyChanged(nameof(_userId));
      }
    }
  }

  public string? UserName
  {
    get => _userName;
    set
    {
      if (_userName != value)
      {
        _userName = value;
        OnPropertyChanged(nameof(UserName));
      }
    }
  }

  public string? UserRole
  {
    get => _userRole;
    set
    {
      if (_userRole == value)
        return;
      _userRole = value;
      OnPropertyChanged(nameof(UserRole));
    }
  }

  public string? UserPictureUrl
  {
    get => _userPictureUrl;
    set
    {
      if (_userPictureUrl != value)
      {
        _userPictureUrl = value;
        OnPropertyChanged(nameof(UserPictureUrl));
      }
    }
  }

  public void SetUserInfoFromUserTable(UsersTable user)
  {
    _userId = user.Id;
    _userName = user.Name;
    _userPictureUrl = user.PictureUrl;
    _userRole = user.UserRole;
    if (string.IsNullOrEmpty(_userId.ToString()))
      return;
    _isLoggedIn = true;
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void OnPropertyChanged(string propertyName)
  {
    // LocalStorage にも保存
    // if (propertyName == nameof(CurrentProjectId))
    // {
    //   await localStorage.SetCurrentProjectIdAsync(CurrentProjectId ?? Guid.Empty);
    //   Console.WriteLine($"LocalStorageにprojectIdを保存 projectId={CurrentProjectId}");
    // }

    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    if (sessionStorage != null) // sessionStorage が null かどうかを確認
    {
      sessionStorage.SaveAppStateAsync(this);
    }
  }
}
