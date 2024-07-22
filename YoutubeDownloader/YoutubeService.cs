using Newtonsoft.Json.Linq;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;
using YoutubeExplode.Common;
using System.Net.Http;
using DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace YoutubeDownloader;

public class YoutubeService
{
    private readonly HttpClient _httpClient;
    private const string API_KEY = "AIzaSyAb6WPNNtjoVTFAuFq2TiukDCCbQtyLJqU";

    public YoutubeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<VideoDataBatchDto> GetVideoData(SearchModel searchModel)
    {
        string url = "https://www.googleapis.com/youtube/v3/search";
        var parameters = new Dictionary<string, string>
        {
            { "q", searchModel.SearchQuery },
            { "part", "snippet" },
            { "type", "video" },
            { "key", API_KEY },
            { "maxResults", "15" }
        };
        if (searchModel.NextPageToken != "")
        {
            parameters["pageToken"] = searchModel.NextPageToken;
        }
        var newUrl = new Uri(QueryHelpers.AddQueryString(url, parameters));
        Console.WriteLine(newUrl.ToString());
        
        using var request = new HttpRequestMessage(HttpMethod.Get, newUrl);
        
        var response = await _httpClient.SendAsync(request);
        var stringResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine(stringResponse);
        /*var data = JObject.Parse(await response.Content.ReadAsStringAsync());

        if (data["items"] != null)
        {
            foreach (var item in data["items"])
            {
                var videoId = item["id"]?["videoId"]?.ToString();
                if (!string.IsNullOrEmpty(videoId))
                {
                    var videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                    var snippet = item["snippet"];
                    var title = snippet?["title"]?.ToString();
                    var authorName = snippet?["channelTitle"]?.ToString();
                    var thumbnailUrl = snippet?["thumbnails"]?["high"]?["url"]?.ToString();

                    var videoData = new VideoDataDto(videoUrl, authorName, title, thumbnailUrl);  // Duration is not available in the search response
                    videos.Add(videoData);
                }
            }
        }*/
        Root? root = JsonConvert.DeserializeObject<Root>(stringResponse);
        if (root is null)
        {
            return new VideoDataBatchDto(null, null);
        }

        List<VideoDataDto> videoDataDtos = new List<VideoDataDto>();
        foreach (var item in root.items)
        {
            var videoUrl = $"https://www.youtube.com/watch?v={item.id.videoId}";
            var title = item.snippet.title;
            var authorName = item.snippet.channelTitle;
            var thumbnailsObj = item.snippet.thumbnails;
            var thumbnailUrl = thumbnailsObj.high != null ? thumbnailsObj.high.url :
                thumbnailsObj.medium != null ? thumbnailsObj.medium.url : thumbnailsObj.@default.url;
            
            videoDataDtos.Add(new VideoDataDto(videoUrl, authorName, title, thumbnailUrl));
        }

        return new VideoDataBatchDto(root.nextPageToken, videoDataDtos);
    }


}