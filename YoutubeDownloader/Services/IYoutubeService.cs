using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services;

public interface IYoutubeService
{
    public Task<List<VideoDataDto>> GetVideosData(IEnumerable<string> videoIds);
    public Task<VideoDataBatchDto> GetVideoSearchData(string searchQuery, string nextPageToken);

    public Task<string> DownloadAudio(string url, string directory="", string extension="mp3");

    public Task<PlaylistDataDto> GetPlaylistInfo(string url);

}