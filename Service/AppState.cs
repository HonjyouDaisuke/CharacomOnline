﻿using System.ComponentModel;

namespace CharacomOnline.Service;

public class AppState : INotifyPropertyChanged
{
  private readonly SessionStorageService sessionStorage;
  private string? _currentProjectName;
  private Guid? _currentProjectId;
  private string? _userName;
  private string? _userPictureUrl;
  private string? _userRole;
  private Guid? _userId;

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

  public event PropertyChangedEventHandler? PropertyChanged;

  private async void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    if (sessionStorage != null) // sessionStorage が null かどうかを確認
    {
      await sessionStorage.SaveAppStateAsync(this);
    }
  }
}
