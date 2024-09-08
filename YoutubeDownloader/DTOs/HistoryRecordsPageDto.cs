using YoutubeDownloader.Models.Database;

namespace YoutubeDownloader.DTOs;

public class HistoryRecordsPageDto
{
    public List<HistoryRecord>? Page { get; set; }
    public int Length { get; set; }
}