using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;

namespace Anvyl.JsonLocalizer
{
    /// <summary>
    /// IStringLocalizer factory used to create instances
    /// of <see cref="IStringLocalizer"/>
    /// </summary>
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IOptions<JsonLocalizerOptions> _options;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Creates a new instance of the <see cref="IStringLocalizerFactory"/>
        /// implementation
        /// </summary>
        /// <param name="cache">The <see cref="IDistributedCache"/> implementation to use</param>
        /// <param name="options">The configuration options for the json localizer</param>
        public JsonStringLocalizerFactory(IOptions<JsonLocalizerOptions> options, IMemoryCache cache)
        {
            _options = options;
            _cache = cache;
        }

        /// <summary>
        /// Create a new instance of <see cref="IStringLocalizer"/>
        /// implementation
        /// </summary>
        /// <param name="resourceSource">Type of the resource</param>
        /// <returns></returns>
        public IStringLocalizer Create(Type resourceSource) =>
            new JsonStringLocalizer(_options, _cache);

        /// <summary>
        /// Creates a new instace of the <see cref="IStringLocalizer"/>
        /// implementation with the specified baseName and location
        /// </summary>
        /// <param name="baseName">The Base Name</param>
        /// <param name="location">The location</param>
        /// <returns></returns>
        public IStringLocalizer Create(string baseName, string location) =>
            new JsonStringLocalizer(_options, _cache);
    }
}
