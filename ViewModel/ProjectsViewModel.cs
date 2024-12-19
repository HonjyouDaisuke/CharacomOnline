using CharacomOnline.Entity;
using CharacomOnline.Service.TableService;

namespace CharacomOnline.ViewModel;

public class ProjectsViewModel
{
	private readonly ProjectsTableService projectsTableService;
	private readonly UserProjectsTableService userProjectsTableService;
	public string? ProjectTitle { get; set; }

	// DI コンストラクタ
	public ProjectsViewModel(ProjectsTableService _projectsTableService, UserProjectsTableService _userProjectsTableService)
	{
		projectsTableService = _projectsTableService ?? throw new ArgumentNullException(nameof(_projectsTableService));
		userProjectsTableService = _userProjectsTableService ?? throw new ArgumentNullException(nameof(_userProjectsTableService)); ;
	}

	public async Task<Guid?> CreateNewProject(string projectName, string description, Guid userId)
	{
		Guid? result = await projectsTableService.CreateProject(projectName, description);
		if (result != null)
		{
			await userProjectsTableService.CreateUserProjects(userId, (Guid)result, (Guid)result);
		}
		ProjectTitle = projectName;
		return result;
	}

	public async Task SetCurrentProject(Guid projectId)
	{
		ProjectTitle = await projectsTableService.GetProjectTitle(projectId);
	}

	public async Task<List<ProjectViewData>?> FetchProjectsFromUserAsync(Guid userId)
	{
		var projects = await projectsTableService.FetchProjectsFromUserIdAsync(userId);

		return projects;
	}
}
