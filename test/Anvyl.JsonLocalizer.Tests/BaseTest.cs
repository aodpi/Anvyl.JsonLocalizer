using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Anvyl.JsonLocalizer.Tests
{
    public abstract class BaseTest
    {
        protected readonly IStringLocalizer _localizer;
        protected const string CacheKeyPrefix = "__loc__";
        protected const string ResourcesPath = "Localization";
        protected readonly IMemoryCache _cache;

        public BaseTest()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.Configure<JsonLocalizerOptions>(opts =>
            {
                opts.CacheKeyPrefix = CacheKeyPrefix;
                opts.ResourcesPath = ResourcesPath;
            });
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            
            var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IStringLocalizerFactory>();
            
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            
            _localizer = factory.Create(null);
            _cache = provider.GetRequiredService<IMemoryCache>();
        }
    }
}