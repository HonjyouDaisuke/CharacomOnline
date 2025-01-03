using CharacomOnline.Entity;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CharacomOnline.Service;

public class BoxFileService
{
  private readonly HttpClient _httpClient;

  public BoxFileService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<byte[]> DownloadFileAsync(string fileId, string accessToken)
  {
    // Box APIのダウンロードエンドポイント
    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.box.com/2.0/files/{fileId}/content");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await _httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    // ファイルデータを取得
    return await response.Content.ReadAsByteArrayAsync();
  }

  public async Task<List<BoxItem>> GetFolderItemsAsync(string folderId, int limit, int offset, string accessToken)
  {
    Console.WriteLine($"folderId = {folderId}");
    var url = $"https://api.box.com/2.0/folders/{folderId}/items?limit={limit}&offset={offset}";
    var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add("Authorization", $"Bearer {accessToken}");

    var response = await _httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    var responseContent = await response.Content.ReadAsStringAsync();
    var folderResponse = JsonConvert.DeserializeObject<BoxFolderResponse>(responseContent);

    return folderResponse.Entries;
  }

  public async Task<int> GetFolderFileCountAsync(string folderId, string accessToken)
  {
    var url = $"https://api.box.com/2.0/folders/{folderId}";
    var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add("Authorization", $"Bearer {accessToken}");

    var response = await _httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    var responseContent = await response.Content.ReadAsStringAsync();
    var folderResponse = JsonConvert.DeserializeObject<BoxFolderDetails>(responseContent);

    // フォルダ内のファイルとサブフォルダの総数を返す
    return folderResponse.ItemCollections.TotalCount;
  }
}
