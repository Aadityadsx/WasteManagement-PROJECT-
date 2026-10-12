using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WasteManagementSystemMVC.Models;

namespace WasteManagementSystemMVC.Controllers
{
    public class WorkerController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullName, string email, string password, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Please fill all required fields.";
                return View();
            }

            using (WasteDbContext db = new WasteDbContext())
            {
                bool exists = db.Workers.Any(w => w.Email == email);

                if (exists)
                {
                    ViewBag.ErrorMessage = "Worker with this email already exists.";
                    return View();
                }

                Worker worker = new Worker
                {
                    FullName = fullName,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    Phone = phone,
                    IsActive = true
                };

                db.Workers.Add(worker);
                db.SaveChanges();
            }

            return RedirectToAction("Login");
        }


        [HttpGet]
        public ActionResult Login()
        {
            if (Session["WorkerID"] != null)
                return RedirectToAction("Dashboard");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            string passwordHash = HashPassword(password);

            using (WasteDbContext db = new WasteDbContext())
            {
                Worker worker = db.Workers.FirstOrDefault(w =>
                    w.Email == email &&
                    w.PasswordHash == passwordHash &&
                    w.IsActive);

                if (worker != null)
                {
                    Session["WorkerID"] = worker.WorkerID;
                    Session["WorkerName"] = worker.FullName;

                    return RedirectToAction("Dashboard");
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }


        public ActionResult Logout()
        {
            Session.Remove("WorkerID");
            Session.Remove("WorkerName");

            return RedirectToAction("Login");
        }


        public ActionResult Dashboard()
        {
            if (Session["WorkerID"] == null)
                return RedirectToAction("Login");

            int workerId = (int)Session["WorkerID"];

            using (WasteDbContext db = new WasteDbContext())
            {
                var complaints = db.WasteComplaints
                    .Where(c => c.WorkerID == workerId)
                    .OrderByDescending(c => c.ReportDate)
                    .ToList();

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
        public ActionResult UpdateStatus(int complaintId, string status)
        {
            if (Session["WorkerID"] == null)
                return RedirectToAction("Login");

            int workerId = (int)Session["WorkerID"];

            string[] allowedStatuses =
            {
            "In Progress",
            "Collected"
        };

            if (!allowedStatuses.Contains(status))
                return RedirectToAction("Dashboard");

            using (WasteDbContext db = new WasteDbContext())
            {
                WasteComplaint complaint =
                    db.WasteComplaints.FirstOrDefault(c =>
                        c.ComplaintID == complaintId &&
                        c.WorkerID == workerId);

                if (complaint != null)
                {
                    complaint.Status = status;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Dashboard");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyLocation(
            int complaintId,
            string workerLatitude,
            string workerLongitude)
        {
            if (Session["WorkerID"] == null)
                return RedirectToAction("Login");

            int workerId = (int)Session["WorkerID"];


            double latitude;
            double longitude;


            bool latitudeValid =
                double.TryParse(
                    workerLatitude,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out latitude);


            bool longitudeValid =
                double.TryParse(
                    workerLongitude,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out longitude);


            if (!latitudeValid || !longitudeValid)
            {
                TempData["ErrorMessage"] =
                    "Unable to read your GPS location. Please try again.";

                return RedirectToAction("Dashboard");
            }


            using (WasteDbContext db = new WasteDbContext())
            {
                WasteComplaint complaint =
                    db.WasteComplaints.FirstOrDefault(c =>
                        c.ComplaintID == complaintId &&
                        c.WorkerID == workerId);


                if (complaint == null)
                {
                    TempData["ErrorMessage"] =
                        "Complaint not found or not assigned to you.";

                    return RedirectToAction("Dashboard");
                }


                if (!complaint.Latitude.HasValue ||
                    !complaint.Longitude.HasValue)
                {
                    TempData["ErrorMessage"] =
                        "Citizen GPS location is not available for this complaint.";

                    return RedirectToAction("Dashboard");
                }


                double distance =
                    CalculateDistanceInMeters(
                        complaint.Latitude.Value,
                        complaint.Longitude.Value,
                        latitude,
                        longitude);


                complaint.WorkerLatitude = latitude;
                complaint.WorkerLongitude = longitude;


                if (distance <= 50)
                {
                    complaint.LocationVerified = true;

                    complaint.Status = "Collected";

                    TempData["SuccessMessage"] =
                        "GPS verified successfully. Waste collection confirmed.";
                }
                else
                {
                    complaint.LocationVerified = false;

                    TempData["ErrorMessage"] =
                        "You are " +
                        Math.Round(distance, 1) +
                        " meters away from the reported location. " +
                        "You must be within 50 meters.";
                }


                db.SaveChanges();
            }


            return RedirectToAction("Dashboard");
        }


        private double CalculateDistanceInMeters(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2)
        {
            const double earthRadius = 6371000;


            double dLatitude =
                (latitude2 - latitude1) * Math.PI / 180;


            double dLongitude =
                (longitude2 - longitude1) * Math.PI / 180;


            double lat1Radians =
                latitude1 * Math.PI / 180;


            double lat2Radians =
                latitude2 * Math.PI / 180;


            double a =
                Math.Sin(dLatitude / 2) *
                Math.Sin(dLatitude / 2)
                +
                Math.Cos(lat1Radians) *
                Math.Cos(lat2Radians) *
                Math.Sin(dLongitude / 2) *
                Math.Sin(dLongitude / 2);


            double c =
                2 *
                Math.Atan2(
                    Math.Sqrt(a),
                    Math.Sqrt(1 - a));


            return earthRadius * c;
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes =
                    Encoding.UTF8.GetBytes(password);

                byte[] hash =
                    sha256.ComputeHash(bytes);

                StringBuilder builder =
                    new StringBuilder();

                foreach (byte b in hash)
                {
                    builder.Append(
                        b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }

}