using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Anvyl.JsonLocalizer.Tests
{
    public abstract class BaseTest
    {
        protected readonly IStringLocalizer _localizer;
        protected const string CacheKeyPrefix = "__loc__";
        protected const string ResourcesPath = "Localization";
        protected readonly IDistributedCache _cache;

        public BaseTest()
        {
            var services = new ServiceCollection();
            services.AddDistributedMemoryCache();
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
            _cache = provider.GetRequiredService<IDistributedCache>();
        }
    }
}