using Autofac;
using Autofac.Integration.Mvc;
using computan.graphapi;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace computan.timesheet
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ConfigureDependencyInjection();
        }

        protected void Application_EndRequest()
        {
            HttpContextWrapper context = new HttpContextWrapper(Context);
            // If we're an ajax request and forms authentication caused a 302, 
            // then we actually need to do a 401
            if (FormsAuthentication.IsEnabled && context.Response.StatusCode == 302
                                              && context.Request.IsAjaxRequest() && !context.Request.IsAuthenticated)
            {
                context.Response.Clear();
                context.Response.StatusCode = 403;
            }
        }

        public static void ConfigureDependencyInjection()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(Global).Assembly);
            builder.RegisterType<GraphMail>().As<IGraphMail>().InstancePerRequest();
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}