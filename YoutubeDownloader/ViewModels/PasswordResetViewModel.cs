using System.ComponentModel.DataAnnotations;

namespace YoutubeDownloader.ViewModels;

public record PasswordResetViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}