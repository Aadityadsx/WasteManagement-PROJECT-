using System;
using System.Linq;
using System.Web.Mvc;
using WasteManagementSystemMVC.Models;

namespace WasteManagementSystemMVC.Controllers
{
    public class WasteController : Controller
    {
        [HttpGet]
        public ActionResult Report()
        {
            if (Session["CitizenID"] == null)
                return RedirectToAction("Login", "Citizen");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(WasteComplaint complaint)
        {
            if (Session["CitizenID"] == null)
                return RedirectToAction("Login", "Citizen");

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage =
                        "Please check all required fields.";

                    return View(complaint);
                }


                complaint.CitizenID =
                    (int)Session["CitizenID"];


                complaint.Status =
                    "Pending";


                complaint.ReportDate =
                    DateTime.Now;


                complaint.ComplaintPhoto =
                    null;


                using (WasteDbContext db =
                       new WasteDbContext())
                {
                    db.WasteComplaints.Add(
                        complaint);

                    db.SaveChanges();
                }


                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage =
                    "Error while submitting complaint: "
                    + ex.Message;

                ViewBag.InnerError =
                    ex.InnerException != null
                        ? ex.InnerException.Message
                        : "";

                return View(complaint);
            }
        }


        public ActionResult Details(int id)
        {
            if (Session["CitizenID"] == null)
                return RedirectToAction("Login", "Citizen");


            int citizenId =
                (int)Session["CitizenID"];


            using (WasteDbContext db =
                   new WasteDbContext())
            {
                var complaint =
                    db.WasteComplaints
                    .FirstOrDefault(c =>
                        c.ComplaintID == id &&
                        c.CitizenID == citizenId);


                if (complaint == null)
                    return HttpNotFound();


                return View(complaint);
            }
        }


        public ActionResult Success()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Track()
        {
            if (Session["CitizenID"] == null)
                return RedirectToAction("Login", "Citizen");


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Track(int complaintId)
        {
            if (Session["CitizenID"] == null)
                return RedirectToAction("Login", "Citizen");


            int citizenId =
                (int)Session["CitizenID"];


            using (WasteDbContext db =
                   new WasteDbContext())
            {
                WasteComplaint complaint =
                    db.WasteComplaints
                    .FirstOrDefault(c =>
                        c.ComplaintID == complaintId &&
                        c.CitizenID == citizenId);


                if (complaint == null)
                {
                    ViewBag.ErrorMessage =
                        "Complaint not found or this complaint does not belong to your account.";

                    return View();
                }


                return View(
                    "TrackResult",
                    complaint);
            }
        }


        public ActionResult MyComplaints()
        {
            if (Session["CitizenID"] == null)
                return RedirectToAction("Login", "Citizen");

            int citizenId =
                (int)Session["CitizenID"];

            using (WasteDbContext db =
                   new WasteDbContext())
            {
                var complaints =
                    db.WasteComplaints
                    .Where(c =>
                        c.CitizenID == citizenId)
                    .OrderByDescending(
                        c => c.ReportDate)
                    .ToList();

                var resolvedComplaint =
                    complaints.FirstOrDefault(
                        c => c.Status == "Resolved");

                if (resolvedComplaint != null)
                {
                    ViewBag.ResolutionMessage =
                        "Your complaint #" +
                        resolvedComplaint.ComplaintID +
                        " has been resolved successfully. " +
                        "Thank you for helping keep the community clean!";
                }

                return View(complaints);
            }

        }
    }

}