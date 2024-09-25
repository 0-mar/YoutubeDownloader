namespace YoutubeDownloader.Areas.Admin.DTOs;

public class UserFilterDto
{
    public string? UserName { get; set; } = null;
    public string? Email { get; set; } = null;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
    public string SortField { get; set; } = "username";
    public string SortOrder { get; set; } = "asc";
}