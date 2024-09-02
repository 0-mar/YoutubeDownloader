using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace YoutubeDownloader.Models.Database;

public class User : IdentityUser
{
    public DateTime CreatedOn { get; set; }

    public string ProfilePicSource { get; set; }
    
    public ICollection<HistoryRecord> HistoryRecords { get; set; } = new List<HistoryRecord>();
}