using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anvyl.JsonLocalizer
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly IOptions<JsonLocalizerOptions> _options;
        private readonly JsonSerializer _serializer = new JsonSerializer();

        public JsonStringLocalizer(IDistributedCache cache, IOptions<JsonLocalizerOptions> options)
        {
            _cache = cache;
            _options = options;
        }

        public LocalizedString this[string name]
        {
            get
            {
                string value = GetString(name);
                return new LocalizedString(name, value ?? $"[{name}]", value == null);
            }
        }

        public string GetString(string key)
        {
            string filePath = $"{_options.Value.ResourcesPath}/{CultureInfo.CurrentCulture.Name}.json";
            string cacheKey = $"{_options.Value.CacheKeyPrefix}_{key}";
            string cacheValue = _cache.GetString(cacheKey);
            if (string.IsNullOrEmpty(cacheValue))
            {
                string result = PullDeserialize<string>(key, Path.GetFullPath(filePath));
                if (!string.IsNullOrEmpty(result))
                    _cache.SetString(cacheKey, result);

                return result;
            }
            return cacheValue;
        }

        /// <summary>
        /// This is used to deserialize only one specific value from
        /// the json without loading the entire object.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize</typeparam>
        /// <param name="propertyName">Name of the property to get from json</param>
        /// <param name="str"><see cref="Stream"/> from where to read the json</param>
        /// <returns>Deserialized propert from the json</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        private T PullDeserialize<T>(string propertyName, string filePath)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            
            using (Stream str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader sReader = new StreamReader(str))
            using (JsonTextReader reader = new JsonTextReader(sReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName
                        && (string)reader.Value == propertyName)
                    {
                        reader.Read();
                        return _serializer.Deserialize<T>(reader);
                    }
                }
                return default(T);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                LocalizedString actualValue = this[name];
                if (!actualValue.ResourceNotFound)
                    return new LocalizedString(name, string.Format(actualValue.Value, arguments), false);
                return actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            string filePath = $"{_options.Value.ResourcesPath}/{CultureInfo.CurrentCulture.Name}.json";
            using (Stream str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader sReader = new StreamReader(str))
            using (JsonTextReader reader = new JsonTextReader(sReader))
            {
                while(reader.Read())
                {
                    if(reader.TokenType == JsonToken.PropertyName)
                    {
                        string key = (string)reader.Value;
                        reader.Read();
                        string value = _serializer.Deserialize<string>(reader);
                        yield return new LocalizedString(key, value, false);
                    }
                }
                yield break;
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new JsonStringLocalizer(_cache, _options);
        }
    }
}
