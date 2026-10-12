using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WasteManagementSystemMVC.Models;

namespace WasteManagementSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (WasteDbContext db = new WasteDbContext())
            {
                ViewBag.TotalComplaints = db.WasteComplaints.Count();
                ViewBag.PendingComplaints = db.WasteComplaints.Count(c => c.Status == "Pending");
                ViewBag.InProgressComplaints = db.WasteComplaints.Count(c => c.Status == "In Progress");
                ViewBag.ResolvedComplaints = db.WasteComplaints.Count(c => c.Status == "Resolved");
            }

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