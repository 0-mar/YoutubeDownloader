using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services;

public interface IYoutubeService
{
    public Task<VideoDataBatchDto> GetVideoInfo(string searchQuery, string nextPageToken);

    public Task<string> DownloadAudio(string url, string directory="");

    public Task<PlaylistDataDto> GetPlaylistInfo(string url);

}