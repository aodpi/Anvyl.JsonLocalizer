using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Anvyl.JsonLocalizer.Tests
{
    public class JsonLocalizerTests
    {
        private readonly IStringLocalizer _localizer;
        private const string cacheKeyPrefix = "__loc__";
        private const string resourcesPath = "Localization";
        private readonly IDistributedCache _cache;

        public JsonLocalizerTests()
        {
            ServiceProvider provider;
            ServiceCollection services = new ServiceCollection();
            services.AddDistributedMemoryCache();
            services.Configure<JsonLocalizerOptions>(opts =>
            {
                opts.CacheKeyPrefix = cacheKeyPrefix;
                opts.ResourcesPath = resourcesPath;
            });
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IStringLocalizerFactory>();
            _localizer = factory.Create(null);
            _cache = provider.GetRequiredService<IDistributedCache>();
        }

        [Fact(DisplayName = "Show [<key>] whenever the <key> does not exist in json file")]
        public void Returns_Resource_NotFound_With_Square_Brackets()
        {
            string locKey = "Hello Traveller";
            Assert.NotNull(_localizer);
            Assert.NotNull(_localizer[locKey]);
            Assert.True(_localizer[locKey].ResourceNotFound);
            Assert.Equal($"[{locKey}]", _localizer[locKey].Value);
        }

        [Fact(DisplayName = "Returns a resource from CurrentCulture named json file")]
        public void ReturnsResource()
        {
            string locKey = "Hello";
            Assert.NotNull(_localizer);
            Assert.NotNull(_localizer[locKey]);
            Assert.True(!_localizer[locKey].ResourceNotFound);
            Assert.Equal("Noroc", _localizer[locKey].Value);
        }

        [Fact(DisplayName = "Returns a resource that should be cached afterwards")]
        public void Returns_Resource_And_Is_Saved_In_Cache()
        {
            string locKey = "Hello";
            Assert.NotNull(_localizer);
            Assert.NotNull(_localizer[locKey]);
            Assert.True(!_localizer[locKey].ResourceNotFound);
            Assert.Equal("Noroc", _localizer[locKey].Value);
            Assert.Equal("Noroc", _cache.GetString($"{cacheKeyPrefix}_{locKey}"));

            // Cleanup
            _cache.Remove($"{cacheKeyPrefix}_{locKey}");
        }

        [Fact(DisplayName = "If the key does not exist it should not be saved in cache")]
        public void Unexistent_Value_Is_Not_Saved_In_Cache()
        {
            string locKey = "Hello Traveller";
            Assert.NotNull(_localizer);
            Assert.NotNull(_localizer[locKey]);
            Assert.True(_localizer[locKey].ResourceNotFound);
            Assert.Equal($"[{locKey}]", _localizer[locKey].Value);
            Assert.Null(_cache.GetString($"{cacheKeyPrefix}_{locKey}"));
        }

        [Fact(DisplayName = "Throw exception when json file for culture does not exist")]
        public void ThrowsIOExceptionForMissingJson()
        {
            var localizerRO = _localizer.WithCulture(new CultureInfo("ro-RO"));
            string locKey = "Hello";
            Assert.NotNull(localizerRO);
            Assert.Throws<System.IO.FileNotFoundException>(() =>
            {
                var roString = localizerRO[locKey];
            });
            // Cleanup
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        }

        [Fact(DisplayName = "Test GetAllStrings")]
        public void GetAllStrings()
        {
            var allStrings = _localizer.GetAllStrings();
            Assert.All(allStrings, (ls) => Assert.False(ls.ResourceNotFound));
            string[] names = allStrings.Select(ls => ls.Name).ToArray();
            string[] values = allStrings.Select(ls => ls.Value).ToArray();
            Assert.Contains("Hello", names);
            Assert.Contains("Hello {0}", names);
            Assert.Contains("Noroc", values);
            Assert.Contains("Noroc {0}", values);
        }
    }
}
