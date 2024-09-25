using YoutubeDownloader.Models.Database;

namespace YoutubeDownloader.DTOs;

public class PageDto<T>
{
    public List<T>? Page { get; set; }
    public int Length { get; set; }
}