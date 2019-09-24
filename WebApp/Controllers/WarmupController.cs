using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class WarmupController : Controller
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // GET: Warm up
        public ActionResult Index()
        {
            Logger.Info("Welcome to warm up page...");
            return View();
        }
    }
}