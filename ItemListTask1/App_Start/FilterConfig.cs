/*using System.Web;
using System.Web.Mvc;

namespace ItemListTask1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
*/
// =============================================
// FILE: App_Start/FilterConfig.cs  (updated)
// Register GlobalMvcExceptionFilter for ALL MVC controllers
// =============================================

using ItemListTask1.GlobalHandler;
using System.Web.Mvc;

namespace ItemListTask1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalMvcExceptionFilter()); // ← Add this line
        }
    }
}


// =============================================
// FILE: Global.asax.cs  (relevant part shown)
// =============================================

/*
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using ItemListTask1.GlobalHandler;

protected void Application_Start()
{
    // ... your existing registrations ...

    // Register global exception handler for Web API controllers
    GlobalConfiguration.Configuration.Services
        .Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
}
*/


// =============================================
// SUMMARY: Which exception goes where?
//
//  EXCEPTION TYPE          │ THROWN BY   │ LOG DESTINATION  │ USER SEES
//  ────────────────────────┼─────────────┼──────────────────┼──────────────────────────
//  DataAccessException     │ DAL (repo)  │ FILE (NLog/txt)  │ "Database error, try again"
//  BusinessException       │ Service     │ DB (ErrorLogs)   │ The actual message (user-friendly)
//  NotFoundException       │ DAL/Service │ No log           │ "Item not found"
//  SqlException (raw)      │ DAL         │ FILE             │ "Database error, try again"
//  Anything else           │ Anywhere    │ FILE + DB both   │ "Something went wrong"
//
//
//  WHY DB LOG for Business errors?
//  → You want to TRACK them: "50 users got INVALID_PRICE today" = product bug
//  → Management can query ErrorLogs and spot patterns
//  → They're expected, recoverable, worth recording for analytics
//
//  WHY FILE LOG for Infra errors?
//  → DB might be DOWN — you can't log to it!
//  → DevOps/infra team monitors log files, not DB tables
//  → SqlException details (server name, connection string hints) are sensitive
//
//  WHY NO LOG for NotFoundException?
//  → Normal user behavior (bad URL, stale link)
//  → Logging every 404 pollutes your logs with noise
//  → Just tell the user and move on
//
// =============================================