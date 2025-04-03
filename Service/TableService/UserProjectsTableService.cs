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

  public async Task<Guid?> FetchUserProjectRoleAsync(Guid projectId, Guid userId)
  {
    try
    {
      var response = await _supabaseClient
        .From<UserProjectsTable>() // 更新するテーブル
        .Where(x => x.ProjectId == projectId && x.UserId == userId) // 条件を LINQ の形で指定
        .Single();
      if (response == null)
      {
        Console.WriteLine("responseがnullでした");
        return null;
      }
      return response.RoleId;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return null;
    }
  }

  public async Task<List<Guid>?> FetchProjectUsers(Guid projectId)
  {
    try
    {
      var response = await _supabaseClient
        .From<UserProjectsTable>() // 更新するテーブル
        .Where(x => x.ProjectId == projectId) // 条件を LINQ の形で指定
        .Select("user_id") // 必要なフィールドを指定
        .Get();

      var result = response.Models.Select(x => x.UserId).ToList();
      return result;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return null;
    }
  }

  public async Task DeleteProject(Guid projectId)
  {
    try
    {
      await _supabaseClient.From<UserProjectsTable>().Where(x => x.ProjectId == projectId).Delete();
      Console.WriteLine("user_projectsテーブル、削除成功");
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }
}
