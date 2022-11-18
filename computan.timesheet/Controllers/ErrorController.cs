using computan.timesheet.core.common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult InternalServerError()
        {
            if (User.Identity.IsAuthenticated && Session[Role.User.ToString()] != null)
            {
                string currentUrl = Request.Url.AbsoluteUri;
                if (currentUrl.ToLower().Contains("/account/login?returnurl="))
                {
                    string returnurl = HttpUtility.UrlDecode(currentUrl.Split('=').Last());
                    return RedirectPermanent(returnurl);
                }

                if (currentUrl.ToLower().Contains("/account/login"))
                {
                    return RedirectPermanent("/");
                }
            }

            Response.StatusCode = 500;
            return View();
        }
    }
}