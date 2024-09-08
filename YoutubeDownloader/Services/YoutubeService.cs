using System.Xml;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using YoutubeDownloader.Database;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models.Database;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;

using SearchRoot = YoutubeDownloader.Json.Search.Root;
using VideoRoot = YoutubeDownloader.Json.Video.Root;

namespace YoutubeDownloader.Services;

public class YoutubeService : IYoutubeService
{
    private readonly HttpClient _httpClient;
    private readonly YoutubeClient _youtube;
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public YoutubeService(HttpClient httpClient, AppDbContext context, 
        UserManager<User> userManager,IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        
        _youtube = new YoutubeClient();
    }

    public async Task<List<VideoDataDto>> GetVideosData(IEnumerable<string> videoIds)
    {
        string baseUrl = "https://www.googleapis.com/youtube/v3/videos";
        var parameters = new Dictionary<string, string>
        {
            { "id", String.Join(",", videoIds) },
            { "part", "snippet,contentDetails" },
            { "key", Environment.GetEnvironmentVariable("YOUTUBE_API_KEY") },
        };

        var url = new Uri(QueryHelpers.AddQueryString(baseUrl, parameters));

        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        var response = await _httpClient.SendAsync(request);
        var stringResponse = await response.Content.ReadAsStringAsync();

        List<VideoDataDto> result = new List<VideoDataDto>();
        
        VideoRoot? root = JsonConvert.DeserializeObject<VideoRoot>(stringResponse);
        foreach (var video in root.items)
        {
            TimeSpan parsedDuration = XmlConvert.ToTimeSpan(video.contentDetails.duration);
            string videoUrl = $"https://www.youtube.com/watch?v={video.id}";
            var title = video.snippet.title;
            var authorName = video.snippet.channelTitle;
            var thumbnailsObj = video.snippet.thumbnails;
            var thumbnailUrl = thumbnailsObj.high != null ? thumbnailsObj.high.url :
                thumbnailsObj.medium != null ? thumbnailsObj.medium.url : thumbnailsObj.@default.url;
            
            result.Add(new VideoDataDto(videoUrl, authorName, title, thumbnailUrl, parsedDuration));
        }

        return result;
    }

    public async Task<VideoDataBatchDto> GetVideoSearchData(string searchQuery, string nextPageToken)
    {
        string url = "https://www.googleapis.com/youtube/v3/search";
        var parameters = new Dictionary<string, string>
        {
            { "q", searchQuery },
            { "part", "snippet" },
            { "type", "video" },
            { "key", Environment.GetEnvironmentVariable("YOUTUBE_API_KEY") },
            { "maxResults", "10" }
        };
        if (nextPageToken != "")
        {
            parameters["pageToken"] = nextPageToken;
        }

        var newUrl = new Uri(QueryHelpers.AddQueryString(url, parameters));

        using var request = new HttpRequestMessage(HttpMethod.Get, newUrl);

        var response = await _httpClient.SendAsync(request);
        var stringResponse = await response.Content.ReadAsStringAsync();

        SearchRoot? root = JsonConvert.DeserializeObject<SearchRoot>(stringResponse);
        if (root is null)
        {
            return new VideoDataBatchDto(null, null);
        }

        IEnumerable<string> videoIds = root.items.Select(item => item.id.videoId);
        List<VideoDataDto> videoDataDtos = await GetVideosData(videoIds);

        return new VideoDataBatchDto(root.nextPageToken, videoDataDtos);
    }

    public async Task<string> DownloadAudio(string url, string directory="", string extension="mp3")
    {
        var filePath = "";
        
        var video = await _youtube.Videos.GetAsync(url);
        filePath = Path.Combine(directory, $"{video.Title}.{extension}");
        await _youtube.Videos.DownloadAsync(url, filePath);

        await CreateHistoryRecord(url);
        
        return filePath;
    }

    private async Task CreateHistoryRecord(string url)
    {
        var startIdx = url.IndexOf("v=") + 2;
        var endIdx = url.IndexOf('&', startIdx);

        var videoId = url.Substring(startIdx, endIdx - startIdx);
                
        var data = (await GetVideosData(new []{videoId}))[0];
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
                
        _context.HistoryRecords.Add(new HistoryRecord
        {
            DownloadedOn = DateTime.UtcNow,
            Id = new Guid(),
            User = user,
            ThumbnailUrl = data.ThumbnailUrl,
            UserId = user.Id,
            VideoTitle = data.Title,
            VideoUrl = data.Url
        });
        await _context.SaveChangesAsync();
    }

    public async Task<PlaylistDataDto> GetPlaylistInfo(string url)
    {
        var playlist = await _youtube.Playlists.GetAsync(url);

        var playlistTitle = playlist.Title;
        var playlistAuthor = "Author not available";
        if (playlist.Author != null)
        {
            playlistAuthor = playlist.Author.ChannelTitle;
        }
        var playlistThumbnailUrl = playlist.Thumbnails.GetWithHighestResolution().Url;

        List<VideoDataDto> videoData = new List<VideoDataDto>();

        await foreach (var video in _youtube.Playlists.GetVideosAsync(url))
        {
            var title = video.Title;
            var author = video.Author.ChannelTitle;
            var thumbnail = video.Thumbnails.GetWithHighestResolution().Url;
            var duration = video.Duration;
            var videoUrl = video.Url;

            videoData.Add(new VideoDataDto(videoUrl, author, title, thumbnail, duration ?? TimeSpan.Zero));
        }

        return new PlaylistDataDto(playlistTitle, playlistAuthor, playlistThumbnailUrl, videoData);
    }
}