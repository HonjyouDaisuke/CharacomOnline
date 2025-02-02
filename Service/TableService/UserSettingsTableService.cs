using CharacomOnline.Entity;
using Supabase;
using System.Linq.Expressions;
namespace CharacomOnline.Service.TableService;

public class UserSettingsTableService(Client supabaseClient)
{
	private readonly Client _supabaseClient = supabaseClient;

	public async Task<UserSettings?> GetUserSettingsAsync(Guid userId)
	{
		try
		{
			var response = await _supabaseClient
					.From<UserSettings>()
					.Select("is_standard, is_center_line, is_grid_line, r, is_noize")
					.Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
					.Single();

			if (response == null)
			{
				Console.WriteLine($"characomユーザー情報取り込み失敗 userId:{userId}");
			}
			return response;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching userSetting: {ex.Message}");
			return null;
		}
	}

	private async Task<bool> CheckIfUserExists(Guid userId)
	{
		var checkExistence = await _supabaseClient
				.From<UserSettings>()
				.Where(x => x.UserId == userId)
				.Single();

		if (checkExistence == null)
		{
			Console.WriteLine($"該当するデータが見つかりませんでした: userId={userId}");
			return false;
		}
		return true;
	}

	private async Task UpdateSetting<T>(Guid userId, Expression<Func<UserSettings, T>> selector, T value)
	{
		try
		{
			if (!await CheckIfUserExists(userId)) return;

			var response = await _supabaseClient
					.From<UserSettings>()
					.Where(x => x.UserId == userId)
					.Set(selector.Body is MemberExpression ?
							 Expression.Lambda<Func<UserSettings, object>>(Expression.Convert(selector.Body, typeof(object)), selector.Parameters) :
							 throw new InvalidOperationException("Invalid property selector"),
							 value)
					.Update();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"更新エラー: {ex.Message}");
		}
	}


	public Task UpdateIsStandard(Guid userId, bool isSelect) => UpdateSetting(userId, x => x.IsStandard, isSelect);
	public Task UpdateIsCenterLine(Guid userId, bool isSelect) => UpdateSetting(userId, x => x.IsCenterLine, isSelect);
	public Task UpdateIsGridLine(Guid userId, bool isSelect) => UpdateSetting(userId, x => x.IsGridLine, isSelect);
	public Task UpdateIsNoize(Guid userId, bool isSelect) => UpdateSetting(userId, x => x.IsNoize, isSelect);
	public Task UpdateR(Guid userId, float r) => UpdateSetting(userId, x => x.R, r);
}
