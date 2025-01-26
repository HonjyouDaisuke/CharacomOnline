using CharacomOnline.Entity;
using CharacomOnline.Repositories;
using CharacomOnline.Service;
using CharacomOnline.Service.TableService;

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

  // DI コンストラクタ
  public ProjectsViewModel(
    ProjectsTableService _projectsTableService,
    UserProjectsTableService _userProjectsTableService,
    BoxFileService _boxFileService,
    ProjectsRepository _projectRepository,
    CharaDataTableService _charaDataTableService,
    UsersTableService _usersTableService)
  {
    projectsTableService = _projectsTableService ?? throw new ArgumentNullException(nameof(_projectsTableService));
    userProjectsTableService = _userProjectsTableService ?? throw new ArgumentNullException(nameof(_userProjectsTableService));
    projectRepository = _projectRepository ?? throw new ArgumentNullException(nameof(_projectRepository));
    boxFileService = _boxFileService ?? throw new ArgumentNullException(nameof(_boxFileService));
    charaDataTableService = _charaDataTableService ?? throw new ArgumentNullException(nameof(_charaDataTableService));
    usersTableService = _usersTableService ?? throw new ArgumentNullException(nameof(_usersTableService));
  }

  public async Task GetUsers()
  {
    var projectId = projectRepository.GetCurrentProjectId();
    if (projectId == null) return;
    var users = await userProjectsTableService.FetchProjectUsers((Guid)projectId);
    Console.WriteLine($"projectId = {projectId} users = {users.Count}");
    List<UsersTable> usersList = new List<UsersTable>();
    projectRepository.ClearCurrentProjectUsers();
    foreach (var user in users)
    {
      var userInfo = await usersTableService.GetProjectUserAsync((Guid)user);
      Console.WriteLine($"userId = {userInfo.Id} Name = {userInfo.Name}");
      // if (userInfo.Role == null) continue;
      userInfo.Role = "guest";
      projectRepository.AddCurrentProjectUsers(userInfo);
    }
    return;
  }

  public List<ProjectUsers> GetProjectUsersInfo()
  {
    return projectRepository.GetCurrentProjectUsersInfo();
  }

  public async Task<bool> IsProjectTitleExists(string? projectTitle)
  {
    if (projectTitle == null) return false;
    return await projectsTableService.IsProjectTitleExists(projectTitle);
  }

  public async Task<Guid?> CreateNewProject(string projectName, string description, Guid userId, string folderId, string charaFolderId)
  {
    Guid? result = await projectsTableService.CreateProject(projectName, description, folderId, charaFolderId);
    if (result != null)
    {
      await userProjectsTableService.CreateUserProjects(userId, (Guid)result, (Guid)result);
    }
    ProjectTitle = projectName;
    return result;
  }

  public async Task SetCurrentProject(Guid projectId, string projectName)
  {
    ProjectTitle = await projectsTableService.GetProjectTitle(projectId);
    var folderId = await projectsTableService.GetProjectCharaFolderId(projectId);
    if (folderId != null) projectRepository.SetProjectFolderId(folderId);
    projectRepository.SetProjectId(projectId);
    projectRepository.SetProjectName(projectName);
  }

  public Guid? GetCurrentProjectId()
  {
    return projectRepository.GetCurrentProjectId();
  }

  public string? GetCurrentProjectFolderId()
  {
    return projectRepository.GetCurrentFolderId();
  }
  public string? GetCurrentProjectName()
  {
    return projectRepository.GetCurrentProjectName();
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
}
