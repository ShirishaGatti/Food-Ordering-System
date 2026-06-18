using System.Web.Mvc;

namespace ItemListTask1.Controllers
{
    public class AccController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string Password, string Role)
        {
            // Minimal placeholder authentication: accept if username provided
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                // TODO: replace with real auth
                return RedirectToAction("ItemList", "Item");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string UserName, string Password, string Role)
        {
            // Minimal placeholder: normally you'd create the user here
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Invalid data";
            return View();
        }
    }
}