using CharacomOnline.Entity;
using Supabase;

namespace CharacomOnline.Service.TableService;

public class UsersTableService(Client supabaseClient)
{
  private readonly Client _supabaseClient = supabaseClient;

  public async Task UpdateUserAsync(UsersTable user)
  {
    try
    {
      var response = await _supabaseClient
        .From<UsersTable>() // 更新するテーブル
        .Where(x => x.Id == user.Id) // 条件を LINQ の形で指定
        .Set(x => x.Name, user.Name)
        .Update();

      // レスポンスを確認
      if (response != null && response.Models.Count > 0)
      {
        Console.WriteLine("ユーザー情報を更新しました。");
      }
      else
      {
        Console.WriteLine("更新に失敗しました。");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  public async Task UpdateUserRoleAsync(Guid userId, string newRole)
  {
    try
    {
      var response = await _supabaseClient
        .From<UsersTable>() // 更新するテーブル
        .Where(x => x.Id == userId) // 条件を LINQ の形で指定
        .Set(x => x.UserRole, newRole)
        .Update();

      // レスポンスを確認
      if (response != null && response.Models.Count > 0)
      {
        Console.WriteLine("ユーザー情報を更新しました。");
      }
      else
      {
        Console.WriteLine("更新に失敗しました。");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  public async Task<bool> SetEmailIfNull(Guid userId, string email)
  {
    var response = await _supabaseClient.Rpc(
      "update_email_if_null",
      new { user_id = userId, new_email = email }
    );

    return response != null;
  }

  public async Task<bool> SetNameIfNull(Guid userId, string name)
  {
    var response = await _supabaseClient.Rpc(
      "update_user_name_if_null",
      new { user_id = userId, new_name = name }
    );

    return response != null;
  }

  public async Task<UsersTable?> GetUserAsync(Guid userId)
  {
    try
    {
      // Supabaseクエリで条件を指定してデータを取得
      var response = await _supabaseClient
        .From<UsersTable>() // テーブルを指定
        .Select("id, user_name, email, picture_url, user_role") // 必要なカラムのみ選択
        .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString()) // フィルターを適用
        .Single();

      //UsersTable user = new UsersTable();
      if (response == null)
      {
        Console.WriteLine($"characomユーザー情報取り込み失敗 userId:{userId}");
        return null;
      }

      return response; // 取得したデータの UsersTable を返す
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error fetching project title: {ex.Message}");
      return null; // エラー時は null を返す
    }
  }

  public async Task<ProjectUsers?> GetProjectUserAsync(Guid userId)
  {
    try
    {
      // Supabaseクエリで条件を指定してデータを取得
      var response = await _supabaseClient
        .From<UsersTable>() // テーブルを指定
        .Select("id, user_name, email, picture_url, user_role") // 必要なカラムのみ選択
        .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString()) // フィルターを適用
        .Single();

      //UsersTable user = new UsersTable();
      if (response == null)
      {
        Console.WriteLine($"characomユーザー情報取り込み失敗 userId:{userId}");
        return null;
      }
      ProjectUsers projectUser = new ProjectUsers();
      projectUser.Id = userId;
      projectUser.PictureUrl = response.PictureUrl;
      projectUser.Name = response.Name;
      projectUser.Email = response.Email;

      return projectUser; // 取得したデータの ProjectTitle を返す
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error fetching project title: {ex.Message}");
      return null; // エラー時は null を返す
    }
  }

  public async Task<List<UsersTable>> FetchAllUsersAsync()
  {
    try
    {
      var response = await _supabaseClient
        .From<UsersTable>()
        .Select("id, user_name, email, picture_url, user_role")
        .Get();
      foreach (var item in response.Models)
      {
        Console.WriteLine($"user: {item.ToString()}");
      }
      return response.Models;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"エラーが発生しました: {ex.Message}");
      return new List<UsersTable>();
    }
  }

  public async Task<bool> IsExistEmail(string email)
  {
    var response = await _supabaseClient.Rpc(
      "check_email_exists",
      new { email_to_check = email.ToLower() }
    );
    Console.WriteLine($"response = {response.Content} email={email} ");
    // Content は string なので、"true" の場合に true を返す
    return bool.TryParse(response.Content, out var result) && result;
  }

  public async Task<bool> SendPasswordResetEmail(string email)
  {
    try
    {
      Console.WriteLine("送信!");
      // var options = new ResetPasswordForEmailOptions(email)
      // {
      //   RedirectTo = $"https://localhost:7241/reset-password",
      // };
      // Console.WriteLine($"redirectTo = {options.RedirectTo}");
      // //await _supabaseClient.Auth.ResetPasswordForEmail(email, new SignInOptions { RedirectTo = "https://localhost:7421/reset-password" });
      // await _supabaseClient.Auth.ResetPasswordForEmail(options);
      return true; // 成功
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error: {ex.Message}");
      return false; // 失敗
    }
  }
}
