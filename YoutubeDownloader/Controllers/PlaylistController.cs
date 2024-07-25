using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.DTOs;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Controllers
{
    public class PlaylistController : Controller
    {
        private string TempDir;
        private readonly IYoutubeService _youtubeService;
        
        public PlaylistController(IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
            
            TempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempDir);
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

            /*foreach (var video in playlist.Videos)
            {
                var videoUrl = video.Url;
                var audioFilePath = Path.Combine(tempDir, $"{video.Title}.mp3");

                // Download the video and extract audio
                await youtube.Videos.DownloadAsync(videoUrl, audioFilePath, o => o.SetContainer("mp3"));

                audioFiles.Add(audioFilePath);
            }

            // Create a ZIP file
            var zipFilePath = Path.Combine(Path.GetTempPath(), $"{playlist.Title}.zip");

            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var audioFile in audioFiles)
                {
                    zip.CreateEntryFromFile(audioFile, Path.GetFileName(audioFile));
                }
            }

            // Return the ZIP file to the user
            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            var fileName = $"{playlist.Title}.zip";

            return File(fileBytes, "application/zip", fileName);
        }
        finally
        {
            // Clean up temporary files
            Directory.Delete(tempDir, true);
        }*/
            
            
            
            
            
            
            /*var youtube = new YoutubeClient();
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
            }*/
            
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAudioData(List<string> urlList)
        {
            var youtube = new YoutubeClient();
            string filePath = "";
            
            try
            {
                foreach (var videoUrl in urlList)
                {
                    var title = video.Title;
                    var author = video.Author.ChannelTitle;
                    var thumbnail = video.Thumbnails.GetWithHighestResolution().Url;
                    var duration = video.Duration;
                    var videoUrl = video.Url;

                    videoData.Add(new VideoDataDto(videoUrl, author, title, thumbnail, duration ?? TimeSpan.Zero));
                }

                return Json(new PlaylistDataDto(playlistTitle, playlistAuthor, playlistThumbnailUrl, videoData));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Playlist data fetching went wrong");
            }
        }

    }
}
