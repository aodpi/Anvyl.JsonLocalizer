﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Caching.Distributed;

namespace Anvyl.JsonLocalizer
{
    public class JsonLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;

        public JsonLocalizer(IDistributedCache cache) => _cache = cache;

        public LocalizedString this[string name] => throw new NotImplementedException();

        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => throw new NotImplementedException();
        public IStringLocalizer WithCulture(CultureInfo culture) => throw new NotImplementedException();
    }
}
