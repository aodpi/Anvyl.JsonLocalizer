## JsonLocalizer for ASP.NET Core 2.0 onward
 
 |Codacy|Appveyor|Travis|
 |  --- |  ---   | ---  |
 |[![Codacy Badge](https://api.codacy.com/project/badge/Grade/946b0f3482f643cda85f2ab60e8f4fd6)](https://www.codacy.com/app/balan.valeriu/Anvyl.JsonLocalizer?utm_source=github.com&utm_medium=referral&utm_content=aodpi/Anvyl.JsonLocalizer&utm_campaign=badger) | [![Build status](https://ci.appveyor.com/api/projects/status/jgbp47cautxbkoaq?svg=true)](https://ci.appveyor.com/project/aodpi/anvyl-jsonlocalizer) | [![Build Status](https://travis-ci.org/aodpi/Anvyl.JsonLocalizer.svg?branch=master)](https://travis-ci.org/aodpi/Anvyl.JsonLocalizer) |
---

This package is an Implementation of `IStringLocalizer` for asp.net core. It uses json files to store localized strings plus the `IDistributedCache` implementation in order to cache out the localized strings. The flow is pretty simple,

* whenever a localized string is requested it is firstly checked in the `IDistributedCache`
* If the value for the key is not found in the cache, it is then read from the json file for the respective `CultureInfo`
* When the value is retrieved from json file and it is valid, it's stored in the cache before returning it to the caller.

In this way the localized strings are cached only when requested and are physically stored in json files only.

## Getting started

In Startup.cs make sure to add the localizer factory and the IStringLocalizer service with it's factory to create objects.

```csharp
services.Configure<JsonLocalizerOptions>(Configuration.GetSection(nameof(JsonLocalizerOptions)));
services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
services.AddTransient(serviceProvider =>
{
    var factory = serviceProvider.GetRequiredService<IStringLocalizerFactory>();
    return factory.Create(null);
});
```
