using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Controllers
{
    
    [Authorize(Roles = "Regular")]
    public class DownloadController : Controller
    {

        private readonly IYoutubeService _youtubeService;
        private static readonly Dictionary<string, string> ContentTypes = new()
        {
            {"audio", "audio/mpeg"},
            {"video", "video/mpeg"},
        };
        private static readonly Dictionary<string, string> Extensions = new()
        {
            {"audio", "mp3"},
            {"video", "webm"},
        };

        public DownloadController(IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }
        
        [HttpGet]
        public IActionResult Index(string url, string type)
        {
            var downloadModel = new DownloadViewModel(url, type);
            return View(downloadModel);

        }
        
        [HttpGet]
        public async Task<IActionResult> GetResource(string url, string type)
        {
            var filePath = "";
            var contentType = ContentTypes[type];
            var ext = Extensions[type];
            
            try
            {
                filePath = await _youtubeService.DownloadAudio(url, extension: ext);
                
                // when I put 'using' in front, it gives me System.ObjectDisposedException: Cannot access a closed file.
                FileStream fileStream = new FileStream(filePath, FileMode.Open,FileAccess.Read);
                
                return File(fileStream, contentType, filePath);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while downloading audio");
            }
            finally
            {
                if (filePath != "" && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            
        }

    }
}
