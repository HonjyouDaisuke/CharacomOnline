using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;

public class GoogleService
{
  private readonly string _clientId;
  private readonly string _clientSecret;
  private DriveService _driveService;

  public GoogleService(string clientId, string clientSecret)
  {
    _clientId = clientId;
    _clientSecret = clientSecret;
  }

  /// <summary>
  /// Google Driveにログインするメソッド
  /// </summary>
  public async Task LoginAsync()
  {
    var secrets = new ClientSecrets { ClientId = _clientId, ClientSecret = _clientSecret };

    var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
      secrets,
      new[] { DriveService.Scope.DriveFile }, // 必要なスコープを指定
      "user",
      CancellationToken.None
    );

    _driveService = new DriveService(
      new BaseClientService.Initializer
      {
        HttpClientInitializer = credential,
        ApplicationName = "BlazorGoogleDriveApp",
      }
    );
  }

  /// <summary>
  /// ファイルをGoogle Driveにアップロードするメソッド
  /// </summary>
  /// <param name="filePath">アップロードするローカルファイルのパス</param>
  /// <param name="folderId">アップロード先のフォルダID。nullの場合はルートフォルダ</param>
  public async Task<string> UploadFileAsync(string filePath, string folderId = null)
  {
    if (_driveService == null)
      throw new InvalidOperationException("Login is required before uploading files.");

    var fileMetadata = new Google.Apis.Drive.v3.Data.File
    {
      Name = Path.GetFileName(filePath),
      Parents = folderId != null ? new[] { folderId } : null,
    };

    await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    var request = _driveService.Files.Create(fileMetadata, fileStream, GetMimeType(filePath));
    request.Fields = "id";

    var progress = await request.UploadAsync();

    if (progress.Status == UploadStatus.Completed)
    {
      return request.ResponseBody.Id;
    }
    else
    {
      throw new Exception($"File upload failed: {progress.Exception?.Message}");
    }
  }

  /// <summary>
  /// ファイルのMIMEタイプを取得するヘルパーメソッド
  /// </summary>
  private string GetMimeType(string filePath)
  {
    var mimeType = "application/octet-stream";
    var extension = Path.GetExtension(filePath)?.ToLower();
    if (extension != null)
    {
      var registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);
      if (registryKey?.GetValue("Content Type") is string registryMimeType)
      {
        mimeType = registryMimeType;
      }
    }
    return mimeType;
  }
}
