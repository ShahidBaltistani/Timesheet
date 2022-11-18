using computan.timesheet.core.common;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace computan.timesheet.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CustomeAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomeAuthorizeAttribute(params Role[] roles)
        {
            Roles = string.Join(",", roles);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            // Make sure the user is authenticated.
            if (httpContext.User.Identity.IsAuthenticated && httpContext.Session[Role.User.ToString()] != null)
            {
                if (Roles.Equals(""))
                {
                    return true;
                }

                if (httpContext.User.IsInRole(Role.User.ToString()))
                {
                    return Roles.Contains(Role.User.ToString());
                }

                if (httpContext.User.IsInRole(Role.Admin.ToString()))
                {
                    return true;
                }

                if (httpContext.User.IsInRole(Role.TeamLead.ToString()))
                {
                    return Roles.Contains(Role.TeamLead.ToString());
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                UrlHelper urlHelper = new UrlHelper(context.RequestContext);
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new JsonResult
                {
                    Data = new
                    {
                        Error = "NotAuthorized",
                        LogOnUrl = urlHelper.Action("LogOn", "Account")
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                context.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "Login",
                        returnUrl = context.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery,
                            UriFormat.SafeUnescaped)
                    }));
            }
        }
    }
}