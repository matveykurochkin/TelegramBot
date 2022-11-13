using Microsoft.Extensions.Caching.Memory;
using NLog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace TelegramBot.Internal;

internal static class ImageCache
{
    // ReSharper disable InconsistentNaming
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private static readonly IMemoryCache _memCache = new MemoryCache(new MemoryCacheOptions
    {
        ExpirationScanFrequency = TimeSpan.FromSeconds(10)
    });

    internal static async Task<byte[]?> GetImage(string url, CancellationToken ct)
    {
        var data = await _memCache.GetOrCreateAsync(url, async entry =>
        {
            _logger.Info("Start downloading image to cache: {url}", url);
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(5)); //вынести в конфиг
            using var hc = new HttpClient();
            await using var stream = await hc.GetStreamAsync(entry.Key.ToString(), ct);

            _logger.Info("Resizing and compress to jpeg", url);
            using Image image = await Image.LoadAsync(stream, ct);
            image.Mutate(x => x
                .Resize(image.Width / 2, image.Height / 2)
            );
            using var ms = new MemoryStream();
            await image.SaveAsJpegAsync(ms, cancellationToken: ct);
            return ms.ToArray();
        });

        return data;
    }
}