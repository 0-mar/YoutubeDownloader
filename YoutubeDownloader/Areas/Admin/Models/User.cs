using Microsoft.AspNetCore.Identity;
using YoutubeDownloader.Models.Database;

namespace YoutubeDownloader.Areas.Admin.Models;

public class User : IdentityUser
{
    public DateTime CreatedOn { get; set; }

    public string ProfilePicSource { get; set; }
    
    public ICollection<HistoryRecord> HistoryRecords { get; set; } = new List<HistoryRecord>();
}