using ItemListModel.ViewModel;
using ItemListTask1.Service;
using System.Web.Mvc;

namespace ItemListTask1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController()
        {
            _authService = new AuthService();
        }

        // Register

        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel vm)
        {
            _authService.Register(vm);

            return Json(new
            {
                success = true,
                message = "Registration successful. Please log in."
            });
        }

        // Login

        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel vm)
        {
            LoginViewModel result = _authService.Login(vm);

            // Store identity in Session
            Session["UserId"] = result.ProfileId;
            Session["RoleId"] = result.RoleId;
            Session["UserEmail"] = result.Email;

            string redirectUrl = "";

            switch (result.RoleId)
            {
                case 1:
                    redirectUrl = Url.Action("Index", "Home");
                    break;

                case 2:
                    redirectUrl = Url.Action("Dashboard", "Restaurant");
                    break;

                case 3:
                    redirectUrl = Url.Action("Dashboard", "Admin");
                    break;

                default:
                    redirectUrl = Url.Action("Login", "Auth");
                    break;
            }

            return Json(new
            {
                success = true,
                redirectUrl = redirectUrl
            });
        }

        // Logout

        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Login", "Auth")
            });
        }
    }
}