namespace YoutubeDownloader.DTOs;

public record VideoDataBatchDto(string? NextPageToken, IEnumerable<VideoDataDto>? Videos);