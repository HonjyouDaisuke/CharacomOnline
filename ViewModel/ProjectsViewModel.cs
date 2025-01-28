﻿using System.Threading.Tasks;
using ApexCharts;
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
    Dictionary<string, Dictionary<string, (int? SelectedCount, int? CharaCount)>> charaData = new();

    Console.WriteLine("始まるよー");
    foreach (var item in charaCount)
    {
      if (!charaData.ContainsKey(item.CharaName))
      {
        charaData[item.CharaName] = new Dictionary<string, (int?, int?)>();
      }

      charaData[item.CharaName][item.MaterialName] = (item.SelectedCount, item.CharaCount);

      //Console.WriteLine(
      //  $"CharaName = {item.CharaName} material = {item.MaterialName} count = {item.SelectedCount} / {item.CharaCount}"
      //);
    }
    Console.WriteLine("配列を取得するよ");
    var charaNames = charaCount.Select(item => item.CharaName).Distinct().ToList();
    var materialNames = charaCount.Select(item => item.MaterialName).Distinct().ToList();

    Dictionary<string, Dictionary<string, (int SelectedCount, int CharaCount)>> result = new();
    Console.WriteLine("変換するよー");
    foreach (var charaEntry in charaData)
    {
      // チャラネームごとに結果を初期化
      if (!result.ContainsKey(charaEntry.Key))
      {
        result[charaEntry.Key] = new Dictionary<string, (int SelectedCount, int CharaCount)>();
      }

      foreach (var materialName in materialNames)
      {
        // 各マテリアル名について
        var counts = charaEntry.Value.ContainsKey(materialName)
          ? charaEntry.Value[materialName]
          : (SelectedCount: 0, CharaCount: 0); // 存在しない場合は (0, 0) を設定

        // result に追加
        if (!result[charaEntry.Key].ContainsKey(materialName))
        {
          result[charaEntry.Key][materialName] = (
            counts.SelectedCount ?? 0,
            counts.CharaCount ?? 0
          );
        }
      }
    }
    Console.WriteLine("nullチェックだよ");
    foreach (var c in result.Keys)
    {
      foreach (var m in result[c].Keys)
      {
        if (!result[c].ContainsKey(m))
          Console.WriteLine($"c:{c} m:{m}");
      }
    }
    return result;
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
}
