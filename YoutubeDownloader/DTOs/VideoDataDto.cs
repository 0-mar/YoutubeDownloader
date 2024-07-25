using YoutubeExplode.Common;

namespace YoutubeDownloader.DTOs;

public record VideoDataDto(string Url, string Author, string Title, string ThumbnailUrl, TimeSpan Duration);