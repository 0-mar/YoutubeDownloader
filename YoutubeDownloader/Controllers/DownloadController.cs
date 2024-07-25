using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Controllers
{
    public class DownloadController : Controller
    {
        [HttpGet]
        public IActionResult Index(string url)
        {
            var urlModel = new UrlModel(url);
            return View(urlModel);

        }
        
        [HttpGet]
        public async Task<IActionResult> GetResource(string url)
        {
            var youtube = new YoutubeClient();
            string filePath = "";
            
            try
            {
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                var video = await youtube.Videos.GetAsync(url);
                filePath = $"{video.Title}.{streamInfo.Container.Name}";

                await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
                
                // when I put 'using' in front, it gives me System.ObjectDisposedException: Cannot access a closed file.
                FileStream fileStream = new FileStream(filePath, FileMode.Open,FileAccess.Read);
                
                MemoryStream memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                await fileStream.DisposeAsync();
                //System.IO.File.Delete(filePath);
                
                memoryStream.Position = 0;
                // Return the file to the client
                return File(memoryStream, "audio/mpeg", filePath);
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
