using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Footprint_API.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Default Index page for displaying process is currently running
        /// </summary>
        /// <returns>Empty view object</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}