using CharacomOnline.Entity;
using CharacomOnline.Repositories;
using CharacomOnline.Service.TableService;

namespace CharacomOnline.ViewModel;

public class UsersViewModel
{
	private readonly UserRepository userRepository;
	private readonly UsersTableService usersTableService;

	public UsersViewModel(UserRepository _userRepository, UsersTableService _usersTableService)
	{
		userRepository = _userRepository ?? throw new ArgumentNullException(nameof(_userRepository));
		usersTableService = _usersTableService ?? throw new ArgumentNullException(nameof(_usersTableService));
	}

	public async Task SetCurrentUser(Guid userId)
	{
		UsersTable? user = await usersTableService.GetUserAsync(userId);
		if (user == null) return;
		userRepository.SetCurrentUser(user);
	}

	public string? GetCurrentUserRole()
	{
		Console.WriteLine($"Getします : data->{userRepository.CurrentUser().ToString()}");
		return userRepository.CurrentUser().UserRole;
	}

	public string? GetUserPicture()
	{
		return userRepository.CurrentUser().PictureUrl;
	}

	public string? GetUserName()
	{
		return userRepository.CurrentUser().Name;
	}

	public UsersTable? GetUsersTable()
	{
		return userRepository.CurrentUser();
	}

	public async Task UpdateUserAsync(UsersTable user)
	{
		if (user == null) return;
		userRepository.SetCurrentUser(user);
		await usersTableService.UpdateUserAsync(user);
	}
}
