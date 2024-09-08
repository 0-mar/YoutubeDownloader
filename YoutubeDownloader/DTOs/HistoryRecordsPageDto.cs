using YoutubeDownloader.Models.Database;

namespace YoutubeDownloader.DTOs;

public class HistoryRecordsPageDto
{
    public List<HistoryRecordDto>? Page { get; set; }
    public int Length { get; set; }
}