using YoutubeExplode;
using static System.Net.WebRequestMethods;

namespace YouTubeVideoDownloader
{
    
    internal class Program
    {

        public static async Task DownloadYoutubeVideos(string videoUrl, string outputDirectory)
        {
            await Console.Out.WriteLineAsync("Download Starting...");
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);
            string sanitizeTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var MuxedStream = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();
            if (MuxedStream.Any())
            {
                await Console.Out.WriteLineAsync("Download Started...");
                var streamInfo = MuxedStream.First();
                using var httpClient = new HttpClient();

                var stream = await httpClient.GetStreamAsync(streamInfo.Url);
                var datetime = DateTime.Now;

                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizeTitle}.{streamInfo.Container}");
                using var outputStream = System.IO.File.Create(outputFilePath);
                await stream.CopyToAsync(outputStream);

                Console.WriteLine("Download completed!");
                Console.WriteLine($"Video saved as: {outputFilePath}{datetime}");
            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
            }
            static async Task Main(string[] args)
            {
            string outputDictory = "C:\\YoutubeDownloads";
            bool folderExists = Directory.Exists(outputDictory);
            if(!folderExists)
            {
                Console.WriteLine($"Creating Directory {outputDictory}");
                Directory.CreateDirectory(outputDictory);
            }
            Console.WriteLine("Enter Video Link here. if Many Separate By ,");
            string urls = Console.ReadLine() ?? "";

            var VideoUrls = urls.Split(",");
            foreach ( var videoUrl in VideoUrls )
            {
                await DownloadYoutubeVideos(videoUrl, outputDictory);
            }
            await Console.Out.WriteLineAsync("Press Any Key to exit");
            Console.ReadLine();
    }
    }
}
