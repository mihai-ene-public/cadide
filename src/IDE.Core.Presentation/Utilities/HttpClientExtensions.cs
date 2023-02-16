using System.IO;
using System.Net.Http;

namespace IDE.Core;

public static class HttpClientExtensions
{
    public static async Task DownloadFile(this HttpClient thisClient, string downloadUrl, string downloadPath)
    {
        using (var downloadStream = await thisClient.GetStreamAsync(downloadUrl))
        {
            using (var fs = new FileStream(downloadPath, FileMode.Create))
            {
                await downloadStream.CopyToAsync(fs);
            }
        }
    }
}
