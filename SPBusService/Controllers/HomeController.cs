using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPBusService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            TempData["message"] = "This is a string stroed in TempData!";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}