using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Controllers
{
    public class SearchController : Controller
    {
        private readonly YoutubeService _youtubeService;

        public SearchController(YoutubeService youtubeService)
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
            var data = await _youtubeService.GetVideoData(searchModel);
            return Json(data);
        }
    }
}
