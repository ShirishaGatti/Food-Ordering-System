// =============================================
// FILE: ItemListTask1\GlobalHandler\GlobalMvcExceptionFilter.cs
// PROJECT: FoodOrderingSystem (MVC project)
//
// This is an MVC FilterAttribute — NO Web API needed.
// Uses only: System.Web.Mvc  (already in your project)
//
// REGISTER in App_Start\FilterConfig.cs:
//   filters.Add(new GlobalMvcExceptionFilter());
//
// That's it. This single class catches every unhandled exception
// from every controller action in your entire app.
// =============================================

using ItemListModel.Exceptions;
using ItemListTask1.Infrastructure;
using System;
using System.Web.Mvc;

namespace ItemListTask1.GlobalHandler
{
    public class GlobalMvcExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            // If already handled somewhere else, skip
            if (filterContext.ExceptionHandled) return;

            var ex = filterContext.Exception;
            string url = filterContext.HttpContext.Request.Url?.ToString() ?? "";
            string user = filterContext.HttpContext.User?.Identity?.Name;

            // ── Step 1: Decide where to log ───────────────────────────────────

            if (ex is BusinessException)
            {
                // Business rule broken → DB log
                // (track patterns: "100 users hit price-zero error today")
                DbLogger.Log(ex, url, user);
            }
            else if (ex is DataAccessException || ex is System.Data.SqlClient.SqlException)
            {
                // DB/infra failure → FILE log
                // (DB might be down — can't log to it! Infra team reads log files)
                FileLogger.Log(ex, url);
            }
            else if (ex is NotFoundException)
            {
                // Record not found → NO log (normal user behavior, not an error)
            }
            else
            {
                // Unknown exception → log to BOTH to be safe
                FileLogger.Log(ex, url);
                DbLogger.Log(ex, url, user);
            }

            // ── Step 2: Always send friendly message to user ──────────────────

            string friendlyMessage = GetFriendlyMessage(ex);

            // AJAX request (from your JS fetch/$.ajax calls) → return JSON
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, message = friendlyMessage },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.HttpContext.Response.StatusCode = 200; // keep 200 so jQuery doesn't fire .error()
            }
            else
            {
                // Normal page request → show Error view
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Error.cshtml"
                };
                filterContext.HttpContext.Response.StatusCode = 500;
            }

            filterContext.ExceptionHandled = true;
        }

        private string GetFriendlyMessage(Exception ex)
        {
            if (ex is NotFoundException) return ex.Message; // "Item not found" — safe to show
            if (ex is BusinessException) return ex.Message; // Your own user-friendly message
            if (ex is DataAccessException) return "A database error occurred. Please try again later.";
            return "Something went wrong. Please try again or contact support.";
        }
    }
}
