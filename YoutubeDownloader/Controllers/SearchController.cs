using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Controllers
{
    public class SearchController : Controller
    {
        private readonly IYoutubeService _youtubeService;

        public SearchController(IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideoDataDto>>> GetQueryResults(SearchModel searchModel)
        {
            var data = await _youtubeService.GetVideoInfo(searchModel.SearchQuery, searchModel.NextPageToken);
            return Json(data);
        }
    }
}
