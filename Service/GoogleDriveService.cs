using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace CharacomOnline.Service
{
  public class GoogleDriveService
  {
    private readonly string[] _scopes = { DriveService.Scope.DriveFile };
    private readonly string _applicationName = "Characom Imager Online";
    private readonly string _clientId = "95523868868-jsjmgk7m3rggf2r9dinkrl5vpb7g0uui.apps.googleusercontent.com";
    private readonly string _clientSecret = "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";

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
      // カスタムリダイレクトURIを使用する CustomCodeReceiver を作成
      var customReceiver = new CustomCodeReceiver("https://localhost:5056/authentication/login-callback/");

      // トークンを保存するデータストア (例: ファイルベース)
      var dataStore = new FileDataStore("Google.Apis.Auth");

      // Google の認証フローを設定
      var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
          new ClientSecrets
          {
            ClientId = _clientId,
            ClientSecret = _clientSecret
          },
          _scopes,
          "user",
          CancellationToken.None,
          dataStore,
          customReceiver); // CustomCodeReceiver を指定

      using var service = new DriveService(new BaseClientService.Initializer
      {
        HttpClientInitializer = credential,
        ApplicationName = _applicationName,
      });

      var fileMetadata = new Google.Apis.Drive.v3.Data.File
      {
        Name = fileName
      };

      var request = service.Files.Create(fileMetadata, fileStream, contentType);
      request.Fields = "id";
      await request.UploadAsync();

      return request.ResponseBody?.Id;
    }
  }
}
