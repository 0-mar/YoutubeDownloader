using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly IYoutubeService _youtubeService;
        
        public PlaylistController(IYoutubeService youtubeService)
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
        public async Task<IActionResult> GetPlaylistInfo(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return StatusCode(404, "Empty playlist URL");
            }

            try
            {
                var playlistDataDto = await _youtubeService.GetPlaylistInfo(url);

                return Json(playlistDataDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Playlist data fetching went wrong");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAudioData(List<string> urlList)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            
            List<string> audioFiles = new List<string>();
            foreach (var videoUrl in urlList)
            {
                try
                {
                    var filePath = await _youtubeService.DownloadAudio(videoUrl, tempDir);
                    audioFiles.Add(filePath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Audio downloading went wrong");
                }
            }
            
            // Create a ZIP file
            var zipFileName = "playlist";
            var zipFilePath = Path.Combine(tempDir, $"{zipFileName}.zip");

            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var audioFile in audioFiles)
                {
                    zip.CreateEntryFromFile(audioFile, Path.GetFileName(audioFile));
                }
            }

            // Return the ZIP file to the user
            var fileBytes = await System.IO.File.ReadAllBytesAsync(zipFilePath);
            Directory.Delete(tempDir, true);

            return File(fileBytes, "application/zip", zipFileName);
        }

    }
}
