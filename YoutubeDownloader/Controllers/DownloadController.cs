using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Controllers
{
    public class DownloadController : Controller
    {

        private readonly IYoutubeService _youtubeService;

        public DownloadController(IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }
        
        [HttpGet]
        public IActionResult Index(string url)
        {
            var urlModel = new UrlModel(url);
            return View(urlModel);

        }
        
        [HttpGet]
        public async Task<IActionResult> GetResource(string url)
        {
            var filePath = "";
            
            try
            {
                filePath = await _youtubeService.DownloadAudio(url);
                
                // when I put 'using' in front, it gives me System.ObjectDisposedException: Cannot access a closed file.
                FileStream fileStream = new FileStream(filePath, FileMode.Open,FileAccess.Read);
                
                // Return the file to the client
                return File(fileStream, "audio/mpeg", filePath);
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
