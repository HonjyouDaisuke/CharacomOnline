namespace CharacomOnline.Repositories;

public class ProjectsRepository
{
	private string? _projectName;
	private Guid? _projectId;

	public void SetProjectId(Guid projectId)
	{
		_projectId = projectId;
	}

	public void SetProjectName(string projectName)
	{
		_projectName = projectName;
	}

	public Guid? GetCurrentProjectId()
	{
		return _projectId;
	}

	public string? GetCurrentProjectName()
	{
		return _projectName;
	}
}
