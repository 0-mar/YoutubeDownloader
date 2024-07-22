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

        [HttpPost]
        public async Task<ActionResult<IEnumerable<VideoDataDto>>> GetQueryResults([FromBody] SearchModel searchModel)
        {
            /*var youtube = new YoutubeClient();
            var videos = await youtube.Search.GetVideosAsync(searchModel.SearchQuery);
            var result = videos.Select(v => new VideoDataDto(v.Url, v.Author, v.Title, v.Duration, v.Thumbnails.GetWithHighestResolution().Url));

            return Json(result);*/
            
            var data = await _youtubeService.GetVideoData(searchModel);
            return Json(data);
        }
    }
}
