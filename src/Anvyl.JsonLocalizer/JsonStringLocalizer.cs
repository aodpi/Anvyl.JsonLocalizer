using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Caching.Distributed;

namespace Anvyl.JsonLocalizer
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;

        public JsonStringLocalizer(IDistributedCache cache) => _cache = cache;

        public LocalizedString this[string name]
        {
            get
            {
                return new LocalizedString(name, $"[{name}]", true);
            }
        }

        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => throw new NotImplementedException();
        public IStringLocalizer WithCulture(CultureInfo culture) => throw new NotImplementedException();
    }
}
