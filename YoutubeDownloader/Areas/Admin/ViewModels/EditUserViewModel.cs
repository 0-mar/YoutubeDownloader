using System.ComponentModel.DataAnnotations;
using YoutubeDownloader.Areas.Admin.Enums;

namespace YoutubeDownloader.Areas.Admin.ViewModels;

public class EditUserViewModel
{
    [Required]
    public string Id { get; set; }
    public string UserName { get; set; }
    public string? Password { get; set; }
    [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid role.")]
    public string Role { get; set; }
}