using MediterraneoBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MediterraneoBack.Controllers
{
    public class HomeController : Controller
    {
        private MediterraneoContext db = new MediterraneoContext();

        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            return View(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Mediterraneo Internacional SRL";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}