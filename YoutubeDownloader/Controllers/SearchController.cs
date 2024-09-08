using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.Controllers
{
    
    [Authorize(Roles = "Regular")]
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
        public async Task<ActionResult<IEnumerable<VideoDataDto>>> GetQueryResults(SearchViewModel searchViewModel)
        {
            var data = await _youtubeService.GetVideoSearchData(searchViewModel.SearchQuery, searchViewModel.NextPageToken);
            return Json(data);
        }
    }
}
