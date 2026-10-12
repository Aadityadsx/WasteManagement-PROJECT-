using System.Linq;
using System.Web.Mvc;
using WasteManagementSystemMVC.Models;

namespace WasteManagementSystemMVC.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["AdminLoggedIn"] != null)
                return RedirectToAction("Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "Admin@123")
            {
                Session["AdminLoggedIn"] = true;
                return RedirectToAction("Dashboard");
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }

        public ActionResult Dashboard()
        {
            if (Session["AdminLoggedIn"] == null)
                return RedirectToAction("Login");

            using (WasteDbContext db = new WasteDbContext())
            {
                var complaints = db.WasteComplaints
                    .OrderByDescending(c => c.ReportDate)
                    .ToList();

                var workers = db.Workers
                    .Where(w => w.IsActive)
                    .OrderBy(w => w.FullName)
                    .ToList();

                ViewBag.Workers = workers;

                ViewBag.TotalComplaints = complaints.Count;

                ViewBag.PendingComplaints =
                    complaints.Count(c => c.Status == "Pending");

                ViewBag.InProgressComplaints =
                    complaints.Count(c => c.Status == "In Progress");

                ViewBag.CollectedComplaints =
                    complaints.Count(c => c.Status == "Collected");

                ViewBag.ResolvedComplaints =
                    complaints.Count(c => c.Status == "Resolved");

                return View(complaints);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignWorker(int complaintId, int workerId)
        {
            if (Session["AdminLoggedIn"] == null)
                return RedirectToAction("Login");

            using (WasteDbContext db = new WasteDbContext())
            {
                WasteComplaint complaint =
                    db.WasteComplaints.Find(complaintId);

                Worker worker =
                    db.Workers.FirstOrDefault(w =>
                        w.WorkerID == workerId &&
                        w.IsActive);

                if (complaint != null && worker != null)
                {
                    complaint.WorkerID = workerId;

                    if (complaint.Status == "Pending")
                    {
                        complaint.Status = "In Progress";
                    }

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int complaintId, string status)
        {
            if (Session["AdminLoggedIn"] == null)
                return RedirectToAction("Login");

            string[] allowedStatuses =
            {
            "Pending",
            "In Progress",
            "Collected",
            "Resolved"
        };

            if (!allowedStatuses.Contains(status))
                return RedirectToAction("Dashboard");

            using (WasteDbContext db = new WasteDbContext())
            {
                WasteComplaint complaint =
                    db.WasteComplaints.Find(complaintId);

                if (complaint != null)
                {
                    complaint.Status = status;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Dashboard");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }

}