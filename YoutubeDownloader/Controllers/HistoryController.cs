using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YoutubeDownloader.Database;
using YoutubeDownloader.DTOs;

namespace YoutubeDownloader.Controllers
{
    
    [Authorize(Roles = "Regular")]
    public class HistoryController : Controller
    {
        private readonly AppDbContext _context;

        public HistoryController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<HistoryRecordsPageDto> GetHistoryRecords(Guid? lastId = null, DateTime? lastDownloadDate = null)
        {
            if (lastId == null && lastDownloadDate == null)
            {
                var firstPage = await _context.HistoryRecords
                    .OrderBy(b => b.DownloadedOn)
                    .ThenBy(b => b.Id)
                    .Take(10)
                    .Select(b => new HistoryRecordDto
                    {
                        DownloadedOn = b.DownloadedOn,
                        Id = b.Id,
                        ThumbnailUrl = b.ThumbnailUrl,
                        UserId = b.UserId,
                        VideoTitle = b.VideoTitle,
                        VideoUrl = b.VideoUrl
                    })
                    .ToListAsync();

                return new HistoryRecordsPageDto
                {
                    Page = firstPage,
                    Length = firstPage.Count
                };
            }

            var nextPage = await _context.HistoryRecords
                .OrderBy(b => b.DownloadedOn)
                .ThenBy(b => b.Id)
                .Where(b => b.DownloadedOn > lastDownloadDate || (b.DownloadedOn == lastDownloadDate && b.Id > lastId))
                .Take(10)
                .Select(b => new HistoryRecordDto
                {
                    DownloadedOn = b.DownloadedOn,
                    Id = b.Id,
                    ThumbnailUrl = b.ThumbnailUrl,
                    UserId = b.UserId,
                    VideoTitle = b.VideoTitle,
                    VideoUrl = b.VideoUrl
                })
                .ToListAsync();

            return new HistoryRecordsPageDto
            {
                Page = nextPage,
                Length = nextPage.Count
            };
        }
    }
}
