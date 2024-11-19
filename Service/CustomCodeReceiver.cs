using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using System.Net;

public class CustomCodeReceiver : ICodeReceiver
{
  private readonly string _redirectUri;

  public CustomCodeReceiver(string redirectUri)
  {
    _redirectUri = redirectUri;
  }

  public string RedirectUri => _redirectUri;

  public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(AuthorizationCodeRequestUrl url, CancellationToken cancellationToken)
  {
    // HttpListenerを初期化
    using var listener = new HttpListener();
    listener.Prefixes.Add(_redirectUri);
    listener.Start();

    Console.WriteLine($"Listening on {_redirectUri}");

    // 認証リクエストを受信
    var context = await listener.GetContextAsync();

    // 認証コードを取得
    var code = context.Request.QueryString["code"];
    if (code == null)
    {
      throw new InvalidOperationException("Authorization code not found.");
    }

    // ユーザーへの応答
    var response = context.Response;
    var responseString = "Authentication successful! You can close this window.";
    var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
    response.ContentLength64 = buffer.Length;
    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
    response.OutputStream.Close();

    return new AuthorizationCodeResponseUrl
    {
      Code = code
    };
  }
}
