using CharacomOnline.Entity;

namespace CharacomOnline.Repositories;

public class ProjectsRepository
{
  private ProjectsTable? currentProject;

  // private string? _projectName;
  // private string? _projectDescription;
  // private Guid? _projectId;

  // private string? _folderId;
  private List<ProjectUsers> _currentUsers = new();
  private int _charaDataCount = 0;
  private int _folderFiles = 0;
  private int _selectedDataCount = 0;

  public int CharaDataCount
  {
    get => _charaDataCount;
    set => _charaDataCount = value;
  }
  public int SelectedDataCount
  {
    get => _selectedDataCount;
    set => _selectedDataCount = value;
  }
  public int FolderFiles
  {
    get => _folderFiles;
    set => _folderFiles = value;
  }

  public void SetCurrentProjectInfo(ProjectsTable info)
  {
    currentProject = info;
  }

  // public void SetProjectId(Guid projectId)
  // {
  //   _projectId = projectId;
  // }

  // public void SetProjectFolderId(string projectFolderId)
  // {
  //   _folderId = projectFolderId;
  // }

  // public void SetProjectName(string projectName)
  // {
  //   _projectName = projectName;
  // }

  // public void SetProjectDescription(string projectDescription)
  // {
  //   _projectDescription = projectDescription;
  // }

  public Guid? GetCurrentProjectId()
  {
    return currentProject?.Id;
  }

  public string? GetCurrentFolderId()
  {
    return currentProject?.FolderId;
  }

  public string? GetCurrentCharaFolderId()
  {
    return currentProject?.CharaFolderId;
  }

  public string? GetCurrentProjectName()
  {
    return currentProject?.Title;
  }

  public string? GetCurrentProjectDescription()
  {
    return currentProject?.Description;
  }

  public Guid? GetCurrentProjectCreatedBy()
  {
    return currentProject?.CreatedBy;
  }

  public DateTime? GetCurrentProjectCreatedAt()
  {
    return currentProject?.CreatedAt;
  }

  public void AddCurrentProjectUsers(ProjectUsers newUser)
  {
    _currentUsers.Add(newUser);
  }

  public void ClearCurrentProjectUsers()
  {
    _currentUsers.Clear();
  }

  public List<ProjectUsers> GetCurrentProjectUsersInfo()
  {
    return _currentUsers;
  }
}
