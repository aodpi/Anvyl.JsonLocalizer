using Microsoft.Extensions.Caching.Distributed;

namespace Anvyl.JsonLocalizer
{
    /// <summary>
    /// Configuration options for the json localizer
    /// </summary>
    public class JsonLocalizerOptions
    {
        /// <summary>
        /// The path of the json files where all the localization 
        /// is stored
        /// </summary>
        public string ResourcesPath { get; set; }

        /// <summary>
        /// The cache key prefix to use when creating cache
        /// entries in the <see cref="IDistributedCache"/> implementation
        /// </summary>
        public string CacheKeyPrefix { get; set; }
    }
}
