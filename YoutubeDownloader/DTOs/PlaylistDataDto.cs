namespace YoutubeDownloader.DTOs;

public record PlaylistDataDto(string Title, string Author, string ThumbnailUrl, IEnumerable<VideoDataDto> Videos);