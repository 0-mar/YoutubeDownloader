using YoutubeDownloader.Models.Database;

namespace YoutubeDownloader.DTOs;

public class HistoryRecordDto : BaseEntity
{
    public DateTime DownloadedOn { get; set; }
    public string VideoTitle { get; set; }
    public string VideoUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    public string UserId { get; set; }
}