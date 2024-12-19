using CharacomOnline.Entity;
using Supabase;
using System.Text.Json;

namespace CharacomOnline.Service.TableService;

public class ProjectsTableService(Client supabaseClient)
{
	private readonly Client _supabaseClient = supabaseClient;

	public async Task<Guid?> CreateProject(string title, string description)
	{
		Guid newUuid = Guid.NewGuid();
		var newProject = new ProjectsTable
		{
			Id = newUuid,
			Title = title,
			Description = description,
		};

		// データベースに挿入
		// Console.WriteLine($"user name = {client.Auth.CurrentUser.Email}");
		var response = await _supabaseClient.From<ProjectsTable>().Insert(newProject);

		if (response != null && response.Models.Count > 0)
		{
			var insertedProject = response.Models.First();
			Console.WriteLine($"Project '{insertedProject.Title}' added successfully with ID: {insertedProject.Id}");
			return insertedProject.Id; // ID を返す
		}
		return null;
	}

	public async Task<string?> GetProjectTitle(Guid projectId)
	{
		try
		{
			// Supabaseクエリで条件を指定してデータを取得
			var response = await _supabaseClient
					.From<ProjectsTable>() // テーブルを指定
					.Select("title") // 必要なカラムのみ選択
					.Filter("id", Supabase.Postgrest.Constants.Operator.Equals, projectId.ToString()) // フィルターを適用
					.Single();

			return response?.Title; // 取得したデータの ProjectTitle を返す
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching project title: {ex.Message}");
			return null; // エラー時は null を返す
		}
	}
	public async Task<List<ProjectViewData>> FetchProjectsFromUserIdAsync(Guid userId)
	{
		List<ProjectViewData>? projectViewData = new List<ProjectViewData>();

		// Supabase から RPC を呼び出す
		var response = await _supabaseClient.Rpc("get_projects_with_counts", new { user_id = userId });
		Console.WriteLine(response.Content);
		// レスポンスの Content は string 型なので、JSON としてデシリアライズ
		if (!string.IsNullOrEmpty(response.Content))
		{
			try
			{
				// JSON を List<ProjectViewData> に変換
				projectViewData = JsonSerializer.Deserialize<List<ProjectViewData>>(response.Content);
			}
			catch (JsonException ex)
			{
				// デシリアライズエラー処理
				Console.WriteLine("JSON Parsing Error: " + ex.Message);
			}
		}
		else
		{
			// エラーハンドリング（必要に応じて）
			Console.WriteLine("Error: No content returned");
		}

		foreach (ProjectViewData p in projectViewData)
		{
			Console.Write($"id: {p.Id} title:{p.Name} desc:{p.Description} users:{p.Users} ");
		}
		Console.Write(projectViewData.ToString());
		return projectViewData;
	}
}
