
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using WasteManagementSystemMVC.Models;

namespace WasteManagementSystemMVC.Controllers
{
    public class CitizenController : Controller
    {
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        // Display registration page
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Register a new citizen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Please fill in all fields.";
                return View();
            }

            using (WasteDbContext db = new WasteDbContext())
            {
                string cleanEmail = email.Trim().ToLowerInvariant();

                bool emailExists = db.Citizens.Any(
                    c => c.Email == cleanEmail);

                if (emailExists)
                {
                    ViewBag.ErrorMessage = "This email is already registered.";
                    return View();
                }

                Citizen citizen = new Citizen
                {
                    FullName = fullName.Trim(),
                    Email = cleanEmail,
                    PasswordHash = HashPassword(password)
                };

                db.Citizens.Add(citizen);
                db.SaveChanges();
            }

            return RedirectToAction("RegisterSuccess");
        }

        // Hash password using SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(
                    Encoding.UTF8.GetBytes(password));

                return System.BitConverter.ToString(bytes)
                    .Replace("-", "").ToLowerInvariant();
            }
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Please enter your email and password.";
                return View();
            }

            string cleanEmail = email.Trim().ToLowerInvariant();
            string passwordHash = HashPassword(password);

            using (WasteDbContext db = new WasteDbContext())
            {
                var citizen = db.Citizens.FirstOrDefault(c =>
                    c.Email == cleanEmail &&
                    c.PasswordHash == passwordHash);

                if (citizen != null)
                {
                    Session["CitizenID"] = citizen.CitizenID;
                    Session["CitizenName"] = citizen.FullName;

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }
    }
}