using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anvyl.JsonLocalizer.Presentation.Models;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace Anvyl.JsonLocalizer.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private IStringLocalizer _localizer;

        public HomeController(IStringLocalizerFactory localizer)
        {
            _localizer = localizer.Create(null);

        }

        public IActionResult Index()
        {
            Response.Cookies.Append(
               CookieRequestCultureProvider.DefaultCookieName,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(new System.Globalization.CultureInfo("ro-RO"))),
               new CookieOptions
               {
                   Expires = DateTimeOffset.Now.AddYears(1)
               });
               
            string val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello {0}", "Walera"];
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
