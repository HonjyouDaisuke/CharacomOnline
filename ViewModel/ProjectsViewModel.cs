using CharacomOnline.Entity;
using CharacomOnline.Repositories;
using CharacomOnline.Service.TableService;

namespace CharacomOnline.ViewModel;

public class ProjectsViewModel
{
	private readonly ProjectsTableService projectsTableService;
	private readonly UserProjectsTableService userProjectsTableService;
	//private readonly BoxFileService boxFileService;
	private readonly ProjectsRepository projectRepository;
	public string? ProjectTitle { get; set; }

	// DI コンストラクタ
	public ProjectsViewModel(ProjectsTableService _projectsTableService, UserProjectsTableService _userProjectsTableService, ProjectsRepository _projectRepository)
	{
		projectsTableService = _projectsTableService ?? throw new ArgumentNullException(nameof(_projectsTableService));
		userProjectsTableService = _userProjectsTableService ?? throw new ArgumentNullException(nameof(_userProjectsTableService));
		projectRepository = _projectRepository ?? throw new ArgumentNullException(nameof(_projectRepository));
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
		projectRepository.SetProjectId(projectId);
		projectRepository.SetProjectName(projectName);
	}

	public Guid? GetCurrentProjectId()
	{
		return projectRepository.GetCurrentProjectId();
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
}
