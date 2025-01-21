using CharacomOnline.Entity;
using Supabase;

namespace CharacomOnline.Service.TableService;

public class UserProjectsTableService(Client supabaseClient)
{
  private readonly Client _supabaseClient = supabaseClient;

  public async Task CreateUserProjects(Guid userId, Guid projectId, Guid roleId)
  {
    var newUserProjects = new UserProjectsTable
    {
      UserId = userId,
      ProjectId = projectId,
      RoleId = roleId,
    };

    // データベースに挿入
    // Console.WriteLine($"user name = {client.Auth.CurrentUser.Email}");
    var response = await _supabaseClient.From<UserProjectsTable>().Insert(newUserProjects);
    if (response != null)
    {
      Console.WriteLine($"ProjectTitle {newUserProjects.UserId} added successfully.");
    }
  }
}
