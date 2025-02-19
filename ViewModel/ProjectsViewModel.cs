using System.Reflection;
using CharacomOnline.Components;
using CharacomOnline.Entity;
using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;
using Radzen;

namespace CharacomOnline.ViewModel;

public class ProjectsViewModel
{
  private readonly ProjectsTableService projectsTableService;
  private readonly UserProjectsTableService userProjectsTableService;
  private readonly BoxFileService boxFileService;
  private readonly ProjectsRepository projectRepository;
  private readonly UsersTableService usersTableService;
  private readonly CharaDataTableService charaDataTableService;

  public string? ProjectTitle { get; set; }

  public event Action<int>? OnProgressUpdated;
  private CancellationTokenSource? _cts = new();
  public int Progress { get; private set; } = 0;
  public bool IsCompleted { get; private set; } = false;

  // DI コンストラクタ
  public ProjectsViewModel(
    ProjectsTableService _projectsTableService,
    UserProjectsTableService _userProjectsTableService,
    BoxFileService _boxFileService,
    ProjectsRepository _projectRepository,
    CharaDataTableService _charaDataTableService,
    UsersTableService _usersTableService
  )
  {
    projectsTableService =
      _projectsTableService ?? throw new ArgumentNullException(nameof(_projectsTableService));
    userProjectsTableService =
      _userProjectsTableService
      ?? throw new ArgumentNullException(nameof(_userProjectsTableService));
    projectRepository =
      _projectRepository ?? throw new ArgumentNullException(nameof(_projectRepository));
    boxFileService = _boxFileService ?? throw new ArgumentNullException(nameof(_boxFileService));
    charaDataTableService =
      _charaDataTableService ?? throw new ArgumentNullException(nameof(_charaDataTableService));
    usersTableService =
      _usersTableService ?? throw new ArgumentNullException(nameof(_usersTableService));
  }

  public async Task GetUsers()
  {
    var projectId = projectRepository.GetCurrentProjectId();
    if (projectId == null)
      return;
    var users = await userProjectsTableService.FetchProjectUsers((Guid)projectId);
    Console.WriteLine($"projectId = {projectId} users = {users?.Count}");
    List<UsersTable> usersList = new List<UsersTable>();
    projectRepository.ClearCurrentProjectUsers();
    if (users == null)
      return;
    foreach (var user in users)
    {
      var userInfo = await usersTableService.GetProjectUserAsync((Guid)user);
      Console.WriteLine($"userId = {userInfo?.Id} Name = {userInfo?.Name}");
      if (userInfo == null)
        continue;
      if (string.IsNullOrEmpty(userInfo?.PictureUrl))
      {
        if (userInfo != null)
          userInfo.PictureUrl = "/images/no_user_icon.png";
      }
      // if (userInfo.Role == null) continue;
      if (userInfo != null)
      {
        userInfo.Role = "guest";
        projectRepository.AddCurrentProjectUsers(userInfo);
      }
    }
    return;
  }

  public async Task<string?> GetProjectNameFromProjectId(Guid projectId)
  {
    var project = await projectsTableService.GetProjectInfoAsync((Guid)projectId);
    return project?.Title;
  }

  public List<ProjectUsers> GetProjectUsersInfo()
  {
    return projectRepository.GetCurrentProjectUsersInfo();
  }

  public async Task<bool> IsProjectTitleExists(string? projectTitle)
  {
    if (projectTitle == null)
      return false;
    return await projectsTableService.IsProjectTitleExists(projectTitle);
  }

  public async Task<Guid?> CreateNewProject(
    string projectName,
    string description,
    Guid userId,
    string folderId,
    string charaFolderId
  )
  {
    Guid? result = await projectsTableService.CreateProject(
      projectName,
      description,
      folderId,
      charaFolderId
    );
    if (result != null)
    {
      await userProjectsTableService.CreateUserProjects(userId, (Guid)result, (Guid)result);
    }
    ProjectTitle = projectName;
    return result;
  }

  public async Task SetCurrentProject(Guid projectId, string projectName)
  {
    var projectInfo = await projectsTableService.GetProjectInfoAsync(projectId);
    if (projectInfo == null)
      return;
    projectRepository.SetCurrentProjectInfo(projectInfo);
  }

  public async Task<Dictionary<
    string,
    Dictionary<string, (int SelectCount, int CharaCount)>
  >?> GetCurrentProjectCharaCount(Guid projectId)
  {
    var charaCount = await charaDataTableService.GetCharaDataSelectedCountAsync(projectId);
    if (charaCount == null)
      return null;
    Dictionary<string, Dictionary<string, (int SelectedCount, int CharaCount)>> charaData = new();

    foreach (var item in charaCount)
    {
      if (!charaData.ContainsKey(item.CharaName))
      {
        charaData[item.CharaName] = new Dictionary<string, (int, int)>();
      }

      charaData[item.CharaName][item.MaterialName] = (item.SelectedCount, item.CharaCount);

      // Console.WriteLine(
      //  $"CharaName = {item.CharaName} material = {item.MaterialName} count = {item.SelectedCount} / {item.CharaCount}"
      //);
    }
    return charaData;
  }

  public Guid? GetCurrentProjectId()
  {
    return projectRepository.GetCurrentProjectId();
  }

  public string? GetCurrentProjectFolderId()
  {
    return projectRepository.GetCurrentFolderId();
  }

  public string? GetCurrentProjectCharaFolderId()
  {
    return projectRepository.GetCurrentCharaFolderId();
  }

  public string? GetCurrentProjectName()
  {
    return projectRepository.GetCurrentProjectName();
  }

  public string? GetCurrentProjectDescription()
  {
    return projectRepository.GetCurrentProjectDescription();
  }

  public async Task<string?> GetCurrentProjectCreatedBy()
  {
    var createdUserId = projectRepository.GetCurrentProjectCreatedBy();
    if (createdUserId == null)
      return "";
    var createUser = await usersTableService.GetUserAsync((Guid)createdUserId);
    return createUser?.Name;
  }

  public DateTime? GetCurrentProjectCreatedAt()
  {
    return projectRepository.GetCurrentProjectCreatedAt();
  }

  public async Task<List<ProjectViewData>?> FetchProjectsFromUserAsync(Guid userId)
  {
    var projects = await projectsTableService.FetchProjectsFromUserIdAsync(userId);

    return projects;
  }

  public async Task<int> GetCharaDataCount(Guid projectId)
  {
    var dataCount = await charaDataTableService.GetCharaDataCountAsync(projectId);
    projectRepository.CharaDataCount = dataCount;
    return dataCount;
  }

  public async Task<int> GetBoxFileCount(string folderId, string accessToken)
  {
    var fileCount = await boxFileService.GetFolderFileCountAsync(folderId, accessToken);
    projectRepository.FolderFiles = fileCount;
    return fileCount;
  }

  public async Task<int> GetSelectedCharaDataCount(Guid projectId)
  {
    var selectedCount = await charaDataTableService.GetSelectedCharaCountAsync(projectId);
    projectRepository.SelectedDataCount = selectedCount;
    return selectedCount;
  }

  public bool isSameFileInfo(FileInformation boxInfo, FileInformation dbInfo)
  {
    if (boxInfo.CharaName != dbInfo.CharaName)
      return false;
    if (boxInfo.MaterialName != dbInfo.MaterialName)
      return false;
    if (boxInfo.TimesName != dbInfo.TimesName)
      return false;
    return true;
  }

  public async Task InsertOrUpdateCharaData(
    string fileName,
    Guid projectId,
    string fileId,
    Guid userId
  )
  {
    FileInformation? fileInfo = FileNameService.GetDataInfo(fileName);
    if (fileInfo == null)
    {
      Console.WriteLine($"ファイル名が正しくありませんでした。{fileName}");
      return;
    }

    var existFileId = await charaDataTableService.IsCharaDataExists(projectId, fileId);

    if (existFileId)
    {
      var dbInfo = await charaDataTableService.GetFileInformationFromFileIdAsync(projectId, fileId);
      if (isSameFileInfo((FileInformation)fileInfo, (FileInformation)dbInfo))
        return;

      Console.WriteLine($"ファイルの情報を更新します{fileName}");
      await charaDataTableService.UpdateFileInfoAsync(
        projectId,
        fileId,
        (FileInformation)fileInfo,
        userId
      );
    }
    else
    {
      Console.WriteLine($"新規のファイルです{fileName}");
      var charaDataId = await charaDataTableService.CreateCharaData(
        projectId,
        fileId,
        (FileInformation)fileInfo
      );
    }
    return;
  }

  public async Task<int> GetFileCount(string charaFolderId, string accessToken)
  {
    var files = await boxFileService.GetFolderFileCountAsync(charaFolderId, accessToken);
    return files;
  }

  public void InitializeCancellationToken()
  {
    // 以前のトークンをキャンセルし、新しいトークンを作成
    _cts?.Cancel();
    _cts?.Dispose();
    _cts = new CancellationTokenSource();
  }

  public async Task CreateOrUpdateCharaData(
    Guid projectId,
    string charaFolderId,
    string accessToken,
    int files,
    Guid userId
  )
  {
    int offset = 0;
    int limit = 100;
    Progress = 0;
    IsCompleted = false;
    if (_cts == null)
    {
      _cts = new CancellationTokenSource();
    }

    OnProgressUpdated?.Invoke(Progress);
    while (true)
    {
      if (_cts.Token.IsCancellationRequested)
      {
        // キャンセルされた場合、ループを抜ける
        Console.WriteLine("処理がキャンセルされました。");
        break;
      }

      var fileList = await boxFileService.GetFolderItemsAsync(
        charaFolderId,
        limit,
        offset,
        accessToken
      );
      if (fileList == null || fileList.Count == 0)
        break;

      foreach (var item in fileList)
      {
        //キャンセルされている場合は例外
        _cts.Token.ThrowIfCancellationRequested();
        Progress++;

        OnProgressUpdated?.Invoke(Progress);

        if (item.Type != "file")
          continue;
        if (item.Name == "")
          continue;
        var extension = FileNameService.GetExtension(item.Name);
        if (extension != ".jpeg" && extension != ".jpg")
          continue;

        await InsertOrUpdateCharaData(item.Name, projectId, item.Id, userId);
      }
      if (Progress > files)
        break;
      offset += limit;
    }
  }

  public void CancelOperation()
  {
    Console.WriteLine("キャンセルです。");
    _cts?.Cancel();
  }

  public async Task UpdateProjectNameAsync(
    Guid projectId,
    string name,
    string description,
    Guid userId
  )
  {
    await projectsTableService.UpdateProjectNameAsync(projectId, name, description, userId);
  }

  public async Task DeleteProjectAsync(Guid projectId)
  {
    await charaDataTableService.DeleteProjectCharaData(projectId);
    await userProjectsTableService.DeleteProject(projectId);
    await projectsTableService.DeleteProjectAsync(projectId);
  }
}
