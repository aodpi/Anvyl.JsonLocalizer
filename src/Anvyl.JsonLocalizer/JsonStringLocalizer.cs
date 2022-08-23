using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Anvyl.JsonLocalizer
{
    /// <summary>
    /// Service that contains LocalizedStrings with json files as localization
    /// resources and a DistributedCache to cache out the values read from 
    /// json resource files
    /// </summary>
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IOptions<JsonLocalizerOptions> _options;
        private readonly IMemoryCache _cache;

        private static ReadOnlySpan<byte> Utf8Bom => new byte[] { 0xEF, 0xBB, 0xBF };

        public JsonStringLocalizer(IOptions<JsonLocalizerOptions> options, IMemoryCache cache)
        {
            _options = options;
            _cache = cache;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var str = GetString(name);
                return new LocalizedString(name, string.IsNullOrEmpty(str) ? $"[{name}]" : str, string.IsNullOrEmpty(str));
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var value = this[name];

                return value.ResourceNotFound
                    ? value
                    : new LocalizedString(name, string.Format(CultureInfo.InvariantCulture, value.Value, arguments), false);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            yield return new LocalizedString(string.Empty, string.Empty, true);
        }

        private string GetString(string key)
        {
            var cacheKey = $"{_options.Value.CacheKeyPrefix}_{key}";

            return _cache.GetOrCreate(cacheKey, (e) =>
            {
                var relativePath = $"{_options.Value.ResourcesPath}/{CultureInfo.CurrentCulture.Name}.json";
                var fullFilePath = Path.GetFullPath(relativePath);

                if (File.Exists(fullFilePath))
                {
                    return ReadFromJson(key, fullFilePath);
                }

                return string.Empty;
            });
        }

        private static string ReadFromJson(string key, string fullFilePath)
        {
            byte[] buffer = new byte[4096];
            using var stream = File.OpenRead(fullFilePath);

            stream.Read(buffer);

            ReadOnlySpan<byte> spanBuffer = buffer;

            // Read past the UTF-8 BOM bytes if a BOM exists.
            if (spanBuffer.StartsWith(Utf8Bom))
            {
                buffer = spanBuffer.Slice(Utf8Bom.Length).ToArray();
            }

            try
            {
                var reader = new Utf8JsonReader(buffer, false, default);

                while (reader.TokenType != JsonTokenType.PropertyName || !reader.ValueTextEquals(key))
                {
                    if (!reader.Read())
                    {
                        GetMoreBytesFromStream(stream, ref buffer, ref reader);
                    }
                }

                while (!reader.Read())
                {
                    GetMoreBytesFromStream(stream, ref buffer, ref reader);
                }

                return reader.GetString();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private static void GetMoreBytesFromStream(Stream stream, ref byte[] buffer, ref Utf8JsonReader reader)
        {
            int bytesRead;
            if (reader.BytesConsumed < buffer.Length)
            {
                ReadOnlySpan<byte> leftOver = buffer.AsSpan((int)reader.BytesConsumed);
                if (leftOver.Length == buffer.Length)
                {
                    Array.Resize(ref buffer, buffer.Length * 2);
                }

                leftOver.CopyTo(buffer);
                bytesRead = stream.Read(buffer.AsSpan(leftOver.Length));
            }
            else
            {
                bytesRead = stream.Read(buffer);
            }

            reader = new Utf8JsonReader(buffer, isFinalBlock: bytesRead == 0, reader.CurrentState);
        }
    }
}