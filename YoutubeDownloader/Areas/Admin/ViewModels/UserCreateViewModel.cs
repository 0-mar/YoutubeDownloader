using System.ComponentModel.DataAnnotations;
using YoutubeDownloader.Areas.Admin.Enums;

namespace YoutubeDownloader.Areas.Admin.ViewModels;

public class UserCreateViewModel
{
    [Required]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid role.")]
    public string Role { get; set; }
}