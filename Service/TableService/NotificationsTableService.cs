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
}
