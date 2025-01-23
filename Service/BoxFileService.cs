using System.Net;
using System.Net.Http.Headers;
using CharacomOnline.Entity;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SkiaSharp;

namespace CharacomOnline.Service;

public class BoxFileService
{
  private readonly HttpClient _httpClient;
  private readonly NavigationManager _navigationManager;
  private readonly AppState _appState;

  public BoxFileService(
    HttpClient httpClient,
    NavigationManager navigationManager,
    AppState appState
  )
  {
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _navigationManager =
      navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
    _appState = appState ?? throw new ArgumentNullException(nameof(appState));
    Console.WriteLine($"AppState.UserId = {_appState.UserId}");
    Console.WriteLine($"BoxFileService initialized with HttpClient: {_httpClient}");
  }

  public async Task<byte[]?> DownloadFileAsync(string fileId, string accessToken)
  {
    // Box APIのダウンロードエンドポイント
    var request = new HttpRequestMessage(
      HttpMethod.Get,
      $"https://api.box.com/2.0/files/{fileId}/content"
    );
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await _httpClient.SendAsync(request);
    if (
      response.StatusCode == HttpStatusCode.Unauthorized
      || response.StatusCode == HttpStatusCode.Forbidden
    )
    {
      _navigationManager.NavigateTo("/auth", false);
      return null;
    }
    response.EnsureSuccessStatusCode();

    // ファイルデータを取得
    return await response.Content.ReadAsByteArrayAsync();
  }

  public async Task<SKBitmap?> DownloadFileAsSKBitmapAsync(string fileId, string accessToken)
  {
    // Box APIのダウンロードエンドポイント
    var request = new HttpRequestMessage(
      HttpMethod.Get,
      $"https://api.box.com/2.0/files/{fileId}/content"
    );
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await _httpClient.SendAsync(request);
    Console.WriteLine($"responseStatus = {response.StatusCode}");
    if (
      response.StatusCode == HttpStatusCode.Unauthorized
      || response.StatusCode == HttpStatusCode.Forbidden
    )
    {
      Console.WriteLine("リダイレクトします");
      _navigationManager.NavigateTo("/auth", false);
      return null;
    }

    response.EnsureSuccessStatusCode();

    // ファイルデータを取得
    byte[] fileData = await response.Content.ReadAsByteArrayAsync();

    // byte[] データを SKBitmap にデコード
    using (var stream = new MemoryStream(fileData))
    {
      return SKBitmap.Decode(stream);
    }
  }

  public async Task<List<BoxItem>?> GetFolderItemsAsync(
    string folderId,
    int limit,
    int offset,
    string accessToken
  )
  {
    Console.WriteLine($"folderId = {folderId}");
    var url = $"https://api.box.com/2.0/folders/{folderId}/items?limit={limit}&offset={offset}";
    var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add("Authorization", $"Bearer {accessToken}");

    var response = await _httpClient.SendAsync(request);
    if (
      response.StatusCode == HttpStatusCode.Unauthorized
      || response.StatusCode == HttpStatusCode.Forbidden
    )
    {
      _navigationManager.NavigateTo("/auth", false);
      return null;
    }
    response.EnsureSuccessStatusCode();

    var responseContent = await response.Content.ReadAsStringAsync();
    var folderResponse = JsonConvert.DeserializeObject<BoxFolderResponse>(responseContent);
    if (folderResponse == null)
      return null;
    return folderResponse.Entries;
  }

  public async Task<int> GetFolderFileCountAsync(string folderId, string accessToken)
  {
    var url = $"https://api.box.com/2.0/folders/{folderId}";
    var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add("Authorization", $"Bearer {accessToken}");

    var response = await _httpClient.SendAsync(request);
    if (
      response.StatusCode == HttpStatusCode.Unauthorized
      || response.StatusCode == HttpStatusCode.Forbidden
    )
    {
      _navigationManager.NavigateTo("/auth", false);
      return 0;
    }
    response.EnsureSuccessStatusCode();

    var responseContent = await response.Content.ReadAsStringAsync();
    var folderResponse = JsonConvert.DeserializeObject<BoxFolderDetails>(responseContent);
    if (folderResponse == null)
      return 0;
    // フォルダ内のファイルとサブフォルダの総数を返す
    return folderResponse.ItemCollections.TotalCount;
  }
}
