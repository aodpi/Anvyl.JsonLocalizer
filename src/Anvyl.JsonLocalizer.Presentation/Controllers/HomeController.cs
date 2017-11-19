using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anvyl.JsonLocalizer.Presentation.Models;
using Microsoft.Extensions.Localization;

namespace Anvyl.JsonLocalizer.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private IStringLocalizer _localizer;

        public HomeController(IStringLocalizerFactory localizer)
        {
            _localizer = localizer.Create(null);
            string val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello"];
            val = _localizer["Hello {0}", "Walera"];
        }

        public IActionResult Index()
        {
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
