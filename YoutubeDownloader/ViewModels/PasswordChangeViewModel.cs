using System.ComponentModel.DataAnnotations;

namespace YoutubeDownloader.ViewModels;

public class PasswordChangeViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }
}