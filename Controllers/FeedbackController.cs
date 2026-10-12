using System;
using System.Linq;
using System.Web.Mvc;
using WasteManagementSystemMVC.Models;

namespace WasteManagementSystemMVC.Controllers
{
    public class FeedbackController : Controller
    {
        [HttpGet]
        public ActionResult Create(int complaintId)
        {
            if (Session["CitizenID"] == null)
                return RedirectToAction("Login", "Citizen");

            int citizenId = (int)Session["CitizenID"];

            using (WasteDbContext db = new WasteDbContext())
            {
                WasteComplaint complaint =
                    db.WasteComplaints.FirstOrDefault(c =>
                        c.ComplaintID == complaintId &&
                        c.CitizenID == citizenId);

                if (complaint == null)
                    return RedirectToAction("MyComplaints", "Waste");

                if (complaint.Status != "Resolved")
                {
                    TempData["ErrorMessage"] =
                        "Feedback can be submitted only after the complaint is resolved.";

                    return RedirectToAction("MyComplaints", "Waste");
                }

                bool alreadySubmitted =
                    db.Feedbacks.Any(f =>
                        f.ComplaintID == complaintId &&
                        f.CitizenID == citizenId);

                if (alreadySubmitted)
                {
                    TempData["ErrorMessage"] =
                        "You have already submitted feedback for this complaint.";

                    return RedirectToAction("MyComplaints", "Waste");
                }

                ViewBag.ComplaintID = complaintId;

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            int complaintId,
            int rating,
            string comments)
        {
            if (Session["CitizenID"] == null)
                return RedirectToAction("Login", "Citizen");

            int citizenId = (int)Session["CitizenID"];

            if (rating < 1 || rating > 5)
            {
                ViewBag.ErrorMessage =
                    "Please select a rating between 1 and 5 stars.";

                ViewBag.ComplaintID = complaintId;

                return View();
            }

            using (WasteDbContext db = new WasteDbContext())
            {
                WasteComplaint complaint =
                    db.WasteComplaints.FirstOrDefault(c =>
                        c.ComplaintID == complaintId &&
                        c.CitizenID == citizenId);

                if (complaint == null)
                    return RedirectToAction("MyComplaints", "Waste");

                if (complaint.Status != "Resolved")
                {
                    TempData["ErrorMessage"] =
                        "Feedback can be submitted only after the complaint is resolved.";

                    return RedirectToAction("MyComplaints", "Waste");
                }

                bool alreadySubmitted =
                    db.Feedbacks.Any(f =>
                        f.ComplaintID == complaintId &&
                        f.CitizenID == citizenId);

                if (alreadySubmitted)
                {
                    TempData["ErrorMessage"] =
                        "You have already submitted feedback for this complaint.";

                    return RedirectToAction("MyComplaints", "Waste");
                }

                Feedback feedback = new Feedback
                {
                    ComplaintID = complaintId,
                    CitizenID = citizenId,
                    Rating = rating,
                    Comments = comments,
                    FeedbackDate = DateTime.Now
                };

                db.Feedbacks.Add(feedback);
                db.SaveChanges();
            }

            TempData["SuccessMessage"] =
                "Thank you! Your feedback has been submitted successfully.";

            return RedirectToAction("MyComplaints", "Waste");
        }
    }

}