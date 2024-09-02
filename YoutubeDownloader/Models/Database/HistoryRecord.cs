namespace YoutubeDownloader.Models.Database;

public class HistoryRecord : BaseEntity
{
    public DateTime DownloadedOn { get; set; }
    public string VideoTitle { get; set; }
    public string VideoUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}