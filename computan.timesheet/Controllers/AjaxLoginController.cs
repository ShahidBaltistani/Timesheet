using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    public class AjaxLoginController : Controller
    {
        // GET: AjaxLogin
        public JsonResult RedirectToLogin(string returnUrl)
        {
            return Json(new
            {
                redirectTo = "/Account/Login?returnUrl=" + HttpUtility.UrlEncode(returnUrl)
            }, JsonRequestBehavior.AllowGet);
        }
    }
}