using System.Text;
using CharacomOnline.Entity;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Supabase;

namespace CharacomOnline.Service.TableService;

public class NotificationsTableService(Client supabaseClient)
{
  private readonly Client _supabaseClient = supabaseClient;

  public async Task AddNotificationAsync(string title, string message, string type, Guid toUser)
  {
    var notification = new Notification
    {
      Title = title,
      Message = message,
      Type = type,
      UserId = toUser,
    };

    // データベースに挿入
    // Console.WriteLine($"user name = {client.Auth.CurrentUser.Email}");
    var response = await _supabaseClient.From<Notification>().Insert(notification);
    if (response != null)
    {
      Console.WriteLine($"NotificationTitle {notification.Title} added successfully.");
    }
  }

  public async Task<List<Notification>?> FetchNotificationsFromUserIdAsync(
    Guid userId,
    string accessToken,
    string refreshToken
  )
  {
    List<Notification>? projectViewData = new();
    if (_supabaseClient.Auth.CurrentSession == null)
    {
      Console.WriteLine("CurrentSessionがnullでござる");
      var session = await _supabaseClient.Auth.SetSession(accessToken, refreshToken);
    }
    // Supabase から RPC を呼び出す
    var response = await _supabaseClient.Rpc("get_user_notifications", new { p_user_id = userId });
    Console.WriteLine(response.Content);

    var json = response.Content;
    if (json == null)
      return null;
    var notifications = JsonConvert.DeserializeObject<List<Notification>>(json);

    return notifications ?? new List<Notification>();
  }
}
