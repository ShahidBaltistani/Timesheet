using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ContactCompaniesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: ContactCompanies
        public ActionResult Index()
        {
            return View(db.ContactCompany.ToList());
        }

        // GET: ContactCompanies/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContactCompany contactCompany = db.ContactCompany.Find(id);
            if (contactCompany == null)
            {
                return HttpNotFound();
            }

            return View(contactCompany);
        }

        // GET: ContactCompanies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,clientid")] ContactCompany contactCompany)
        {
            if (ModelState.IsValid)
            {
                ContactCompany objcompany = db.ContactCompany.Where(x => x.name == contactCompany.name).FirstOrDefault();
                if (objcompany != null)
                {
                    ModelState.AddModelError("name", "Sorry, Contact domain already exist.");
                    return View(contactCompany);
                }

                db.ContactCompany.Add(contactCompany);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactCompany);
        }

        // GET: ContactCompanies/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContactCompany contactCompany = db.ContactCompany.Find(id);
            if (contactCompany == null)
            {
                return HttpNotFound();
            }

            return View(contactCompany);
        }

        // POST: ContactCompanies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,clientid")] ContactCompany contactCompany)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactCompany).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactCompany);
        }

        // GET: ContactCompanies/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContactCompany contactCompany = db.ContactCompany.Find(id);
            if (contactCompany == null)
            {
                return HttpNotFound();
            }

            return View(contactCompany);
        }

        // POST: ContactCompanies/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ContactCompany contactCompany = db.ContactCompany.Find(id);
            db.ContactCompany.Remove(contactCompany);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}