using System.Xml;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using YoutubeDownloader.DTOs;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;

using SearchRoot = YoutubeDownloader.Json.Search.Root;
using VideoRoot = YoutubeDownloader.Json.Video.Root;

namespace YoutubeDownloader.Services;

public class YoutubeService : IYoutubeService
{
    private readonly HttpClient _httpClient;
    private YoutubeClient Youtube;

    public YoutubeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        Youtube = new YoutubeClient();
    }

    private async Task<List<TimeSpan>> GetVideosDuration(IEnumerable<string> videoIds)
    {
        string baseUrl = "https://www.googleapis.com/youtube/v3/videos";
        var parameters = new Dictionary<string, string>
        {
            { "id", String.Join(",", videoIds) },
            { "part", "contentDetails" },
            { "key", Environment.GetEnvironmentVariable("YOUTUBE_API_KEY") },
        };

        var url = new Uri(QueryHelpers.AddQueryString(baseUrl, parameters));

        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        var response = await _httpClient.SendAsync(request);
        var stringResponse = await response.Content.ReadAsStringAsync();

        VideoRoot? root = JsonConvert.DeserializeObject<VideoRoot>(stringResponse);
        List<TimeSpan> durations = new List<TimeSpan>();

        foreach (var video in root.items)
        {
            TimeSpan parsedDuration = XmlConvert.ToTimeSpan(video.contentDetails.duration);
            durations.Add(parsedDuration);
        }

        return durations;
    }

    public async Task<VideoDataBatchDto> GetVideoInfo(string searchQuery, string nextPageToken)
    {
        string url = "https://www.googleapis.com/youtube/v3/search";
        var parameters = new Dictionary<string, string>
        {
            { "q", searchQuery },
            { "part", "snippet" },
            { "type", "video" },
            { "key", Environment.GetEnvironmentVariable("YOUTUBE_API_KEY") },
            { "maxResults", "15" }
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
        List<TimeSpan> videoDurations = await GetVideosDuration(videoIds);

        List<VideoDataDto> videoDataDtos = new List<VideoDataDto>();
        foreach ((var item, var duration) in root.items.Zip(videoDurations, (i1, i2) => (i1, i2)))
        {
            var videoUrl = $"https://www.youtube.com/watch?v={item.id.videoId}";
            var title = item.snippet.title;
            var authorName = item.snippet.channelTitle;
            var thumbnailsObj = item.snippet.thumbnails;
            var thumbnailUrl = thumbnailsObj.high != null ? thumbnailsObj.high.url :
                thumbnailsObj.medium != null ? thumbnailsObj.medium.url : thumbnailsObj.@default.url;

            videoDataDtos.Add(new VideoDataDto(videoUrl, authorName, title, thumbnailUrl, duration));
        }

        return new VideoDataBatchDto(root.nextPageToken, videoDataDtos);
    }

    public async Task<string> DownloadAudio(string url, string directory="", string extension="mp3")
    {
        var filePath = "";

        /*var streamManifest = await Youtube.Videos.Streams.GetManifestAsync(url);
        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        var video = await Youtube.Videos.GetAsync(url);*/
        //filePath = Path.Combine(directory, $"{video.Title}.{streamInfo.Container.Name}");
        //filePath = Path.Combine(directory, $"{video.Title}.mp3");

        //await Youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
        
        var video = await Youtube.Videos.GetAsync(url);
        filePath = Path.Combine(directory, $"{video.Title}.{extension}");
        await Youtube.Videos.DownloadAsync(url, filePath);

        return filePath;
    }

    public async Task<PlaylistDataDto> GetPlaylistInfo(string url)
    {
        var playlist = await Youtube.Playlists.GetAsync(url);

        var playlistTitle = playlist.Title;
        var playlistAuthor = "Author not available";
        if (playlist.Author != null)
        {
            playlistAuthor = playlist.Author.ChannelTitle;
        }
        var playlistThumbnailUrl = playlist.Thumbnails.GetWithHighestResolution().Url;

        List<VideoDataDto> videoData = new List<VideoDataDto>();

        await foreach (var video in Youtube.Playlists.GetVideosAsync(url))
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