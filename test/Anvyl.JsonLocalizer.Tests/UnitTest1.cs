using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using Xunit;

namespace Anvyl.JsonLocalizer.Tests
{
    public class UnitTest1
    {
        private IServiceProvider _provider;
        public UnitTest1()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDistributedMemoryCache();
            services.Configure<JsonLocalizerOptions>(opts =>
            {
                opts.CacheKeyPrefix = "__loc__";
                opts.ResourcesPath = "Localization";
            });
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            _provider = services.BuildServiceProvider();
        }
        [Fact]
        public void Test1()
        {
            const string locKey = "Hello";
            var srv = _provider.GetRequiredService<IStringLocalizerFactory>();
            var localizer = srv.Create(null);
            Assert.NotNull(localizer);
            Assert.NotNull(localizer[locKey]);
            Assert.True(localizer[locKey].ResourceNotFound);
        }
    }
}
