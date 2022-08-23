using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Anvyl.JsonLocalizer
{
    public static class JsonLocalizerServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLocalizer(this IServiceCollection services) => AddJsonLocalizer(services, (o) => { });

        public static IServiceCollection AddJsonLocalizer(this IServiceCollection services, Action<JsonLocalizerOptions> options)
        {
            services.AddOptions<JsonLocalizerOptions>()
                .BindConfiguration(nameof(JsonLocalizerOptions))
                .Configure(options);

            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.AddTransient<IStringLocalizer, JsonStringLocalizer>();

            return services;
        }
    }
}
