using CharacomOnline.Entity;

namespace CharacomOnline.Service.TableService;

public class UsersTableService(Supabase.Client supabaseClient)
{
	private readonly Supabase.Client _supabaseClient = supabaseClient;

	public async Task UpdateUserNameAndEmailAsync(Guid userId, string userName, string email)
	{
		if (string.IsNullOrEmpty(userName))
		{
			userName = email;
		}

		try
		{
			var response = await _supabaseClient
				.From<UsersTable>() // 更新するテーブル
				.Where(x => x.Id == userId) // 条件を LINQ の形で指定
				.Set(x => x.Name!, userName)
				.Set(x => x.Email!, email)
				.Update();
			Console.WriteLine($"Response: {response}");
			Console.WriteLine($"Response Models Count: {response?.Models.Count}");
			// レスポンスを確認
			if (response != null && response.Models.Count > 0)
			{
				Console.WriteLine("ユーザー情報を更新しました。");
			}
			else
			{
				Console.WriteLine($"更新に失敗しました。userId = {userId}");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
	public async Task UpdateUserAsync(UsersTable user)
	{
		try
		{
			var response = await _supabaseClient
				.From<UsersTable>() // 更新するテーブル
				.Where(x => x.Id == user.Id) // 条件を LINQ の形で指定
				.Set(x => x.Name ?? "", user.Name ?? "")
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
				.Set(x => x.UserRole ?? "", newRole)
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
			projectUser.PictureUrl = response.PictureUrl ?? "";
			projectUser.Name = response.Name ?? "";
			projectUser.Email = response.Email ?? "";

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
}
