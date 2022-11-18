using computan.timesheet.core;
using computan.timesheet.core.integrations;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using computan.timesheet.Models.FreedCamp;
using computan.timesheet.Models.OrphanTickets;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class IntegrationsController : BaseController
    {
        // GET: Integrations
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult Index()
        {
            FreedCampViewModel model = new FreedCampViewModel();
            // Fetch Integration row for Freedcamp.
            Integration freedcamp = db.integration.Where(i => i.name == "freedcamp").FirstOrDefault();
            // Deserialize the settings into FreedCampSetting
            FreedcampSetting settings = JsonConvert.DeserializeObject<FreedcampSetting>(freedcamp.appsettings);
            // Prepare ViewModel and generate the form.
            model.Id = freedcamp.id;
            model.IsEnabled = freedcamp.isenabled;
            model.Name = freedcamp.name;
            model.BaseUrl = settings.baseurl;
            model.Publickey = settings.publickey;
            model.Privatekey = settings.privatekey;
            model.createdonutc = freedcamp.createdonutc;

            ViewBag.currentSubTab = "Integration";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FreedCampViewModel model)
        {
            if (ModelState.IsValid)
            {
                Integration integration = new Integration();
                try
                {
                    FreedcampSetting setting = new FreedcampSetting();
                    integration = db.integration.Find(model.Id);
                    setting.baseurl = model.BaseUrl;
                    setting.publickey = model.Publickey;
                    setting.privatekey = model.Privatekey;
                    integration.appsettings = JsonConvert.SerializeObject(setting);
                    integration.updatedonutc = DateTime.Now;
                    integration.ipused = Request.UserHostAddress;
                    integration.isenabled = model.IsEnabled;
                    integration.userid = User.Identity.GetUserId();
                    db.Entry(integration).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.AddModelError("", "Successfully Updated.");
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        //setting of team in rocket 
        public ActionResult RocketWebhookEndPoints()
        {
            System.Collections.Generic.List<Team> webhookEndPoints = db.Team.Where(x => x.isactive).ToList();
            //webhookEndPoints.Add(new Team { id = 0, name = "Unassigned",RocketUrl="its a text and not editable" });
            return View(webhookEndPoints);
        }

        [HttpPost]
        public ActionResult EditRocketUrl(RocketUrlViewModel model)
        {
            Team team = db.Team.Where(x => x.id == model.Id).FirstOrDefault();
            if (team != null)
            {
                team.RocketUrl = model.RocketUrl;
                db.SaveChanges();
            }

            return RedirectToAction("RocketWebhookEndPoints");
        }
    }
}