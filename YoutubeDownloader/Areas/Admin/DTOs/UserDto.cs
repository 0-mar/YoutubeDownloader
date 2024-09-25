namespace YoutubeDownloader.Areas.Admin.DTOs;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ProfilePicSource { get; set; }
    public string Role { get; set; }
}