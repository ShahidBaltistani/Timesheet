using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace computan.timesheet.Extensions
{
    public static class ViewExtensions
    {
        public static List<int> Ages = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public static readonly string BaseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        public static string RenderToString(this PartialViewResult partialView)
        {
            HttpContext httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
            }

            string controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            ControllerBase controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory()
                .CreateController(httpContext.Request.RequestContext, controllerName);
            ControllerContext controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);
            IView view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    view.Render(
                        new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime AddWorkingDays(int daysToAdd)
        {
            DateTime date = DateTime.Now;
            if (daysToAdd > 0)
            {
                while (daysToAdd != 0)
                {
                    date = date.AddDays(1);
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        daysToAdd -= 1;
                    }
                }
            }
            else if (daysToAdd < 0)
            {
                while (daysToAdd != 0)
                {
                    date = date.AddDays(-1);
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        daysToAdd += 1;
                    }
                }
            }

            return date;
        }
    }
}