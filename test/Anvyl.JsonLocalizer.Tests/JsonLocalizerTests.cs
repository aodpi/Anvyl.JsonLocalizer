using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Anvyl.JsonLocalizer.Tests
{
    public class JsonLocalizerTests
    {
        private readonly IStringLocalizer _localizer;
        private const string CacheKeyPrefix = "__loc__";
        private const string ResourcesPath = "Localization";
        private readonly IDistributedCache _cache;

        public JsonLocalizerTests()
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
            
            _localizer = factory.Create(null);
            _cache = provider.GetRequiredService<IDistributedCache>();
        }

        [Fact(DisplayName = "Show [<key>] whenever the <key> does not exist in json file")]
        public void Returns_Resource_NotFound_With_Square_Brackets()
        {
            const string locKey = "Hello Traveller";
            Assert.NotNull(_localizer);
            Assert.NotNull(_localizer[locKey]);
            Assert.True(_localizer[locKey].ResourceNotFound);
            Assert.Equal($"[{locKey}]", _localizer[locKey].Value);
        }

        [Fact(DisplayName = "Returns a resource from CurrentCulture named json file")]
        public void ReturnsResource()
        {
            const string locKey = "Hello";
            Assert.NotNull(_localizer);
            Assert.NotNull(_localizer[locKey]);
            Assert.True(!_localizer[locKey].ResourceNotFound);
            Assert.Equal("Noroc", _localizer[locKey].Value);
        }

        [Fact(DisplayName = "Returns a resource that should be cached afterwards")]
        public void Returns_Resource_And_Is_Saved_In_Cache()
        {
            const string locKey = "Hello";
            Assert.NotNull(_localizer);
            Assert.NotNull(_localizer[locKey]);
            Assert.True(!_localizer[locKey].ResourceNotFound);
            Assert.Equal("Noroc", _localizer[locKey].Value);
            Assert.Equal("Noroc", _cache.GetString($"{CacheKeyPrefix}_{locKey}"));

            // Cleanup
            _cache.Remove($"{CacheKeyPrefix}_{locKey}");
        }

        [Fact(DisplayName = "If the key does not exist it should not be saved in cache")]
        public void Unexistent_Value_Is_Not_Saved_In_Cache()
        {
            const string locKey = "Hello Traveller";
            Assert.NotNull(_localizer);
            Assert.NotNull(_localizer[locKey]);
            Assert.True(_localizer[locKey].ResourceNotFound);
            Assert.Equal($"[{locKey}]", _localizer[locKey].Value);
            Assert.Null(_cache.GetString($"{CacheKeyPrefix}_{locKey}"));
        }

        [Fact(DisplayName = "Creates new file for new culture with null values")]
        public void ThrowsIOExceptionForMissingJson()
        {
            var localizerRO = _localizer.WithCulture(new CultureInfo("ro-RO"));
            const string locKey = "Hello";
            Assert.NotNull(localizerRO);
            Assert.True(localizerRO[locKey].ResourceNotFound);
            Assert.Equal($"[{locKey}]", localizerRO[locKey]);
            // Cleanup
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        }

        [Fact(DisplayName = "Test GetAllStrings")]
        public void GetAllStrings()
        {
            var localizedStrings = _localizer.GetAllStrings().ToArray();
            Assert.All(localizedStrings, ls => Assert.False(ls.ResourceNotFound));
            var names = localizedStrings.Select(ls => ls.Name).ToArray();
            var values = localizedStrings.Select(ls => ls.Value).ToArray();
            Assert.Contains("Hello", names);
            Assert.Contains("Hello {0}", names);
            Assert.Contains("Noroc", values);
            Assert.Contains("Noroc {0}", values);
        }
    }
}
