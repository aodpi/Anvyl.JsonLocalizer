using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anvyl.JsonLocalizer
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IDistributedCache _cache;
        private readonly IOptions<JsonLocalizerOptions> _options;

        public JsonStringLocalizerFactory(IDistributedCache cache, IOptions<JsonLocalizerOptions> options)
        {
            _cache = cache;
            _options = options;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_cache, _options);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_cache, _options);
        }
    }
}
