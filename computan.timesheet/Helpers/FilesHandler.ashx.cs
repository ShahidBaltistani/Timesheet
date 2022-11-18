using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace computan.timesheet.Helpers
{
    public class FilesHandler : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                string filepath = context.Request.QueryString["File"];
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + filepath))
                {
                    string filename = filepath.Split('/').Last();
                    context.Response.Buffer = true;
                    context.Response.Clear();
                    context.Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.WriteFile("~" + filepath);
                }
                else
                {
                    context.Response.ContentType = "text/html";
                    context.Response.Write("File not found");
                }
            }
            else
            {
                context.Response.Redirect("/account/login");
            }
        }

        public bool IsReusable => false;
    }
}