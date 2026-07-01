using System;
using System.Web.Mvc;

namespace ItemListTask1.Filters
{
    // Usage:
    // [RoleAuthorize(2)]      -> Restaurant only
    // [RoleAuthorize(1, 2)]   -> User or Restaurant
    // [RoleAuthorize(3)]      -> Admin only
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly int[] _allowedRoles;

        public RoleAuthorizeAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            if (session["RoleId"] == null)
            {
                filterContext.Result = new RedirectResult("/Auth/Login");
                return;
            }

            int role = Convert.ToInt32(session["RoleId"]);

            bool authorized = false;

            foreach (int allowedRole in _allowedRoles)
            {
                if (allowedRole == role)
                {
                    authorized = true;
                    break;
                }
            }

            if (!authorized)
            {
                filterContext.Result = new HttpStatusCodeResult(403, "Forbidden");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}