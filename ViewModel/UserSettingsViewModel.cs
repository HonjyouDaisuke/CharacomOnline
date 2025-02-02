using CharacomOnline.Entity;
using CharacomOnline.Service.TableService;

namespace CharacomOnline.ViewModel;

public class UserSettingsViewModel
{
	private readonly UserSettingsTableService userSettingTableService;
	private UserSettings userSettings = new();

	public UserSettings UserSettings { get => userSettings; set => userSettings = value; }

	public UserSettingsViewModel(UserSettingsTableService _userSettingsTableService)
	{
		userSettingTableService = _userSettingsTableService ?? throw new ArgumentNullException(nameof(_userSettingsTableService));
	}

	public async Task UpdateIsCenterLine(Guid userId, bool isCenterLine)
	{
		userSettings.IsCenterLine = isCenterLine;
		await userSettingTableService.UpdateIsCenterLine(userId, isCenterLine);
	}

	public async Task UpdateIsStandard(Guid userId, bool isStandard)
	{
		userSettings.IsStandard = isStandard;
		await userSettingTableService.UpdateIsStandard(userId, isStandard);
	}

	public async Task UpdateIsGridLine(Guid userId, bool isGridLine)
	{
		userSettings.IsGridLine = isGridLine;
		await userSettingTableService.UpdateIsGridLine(userId, isGridLine);
	}

	public async Task UpdateIsNoize(Guid userId, bool isNoize)
	{
		userSettings.IsNoize = isNoize;
		await userSettingTableService.UpdateIsNoize(userId, isNoize);
	}

	public async Task GetUserSettings(Guid userId)
	{
		var userSetting = await userSettingTableService.GetUserSettingsAsync(userId);
		if (userSetting == null) return;
		userSettings = userSetting;
	}
}
