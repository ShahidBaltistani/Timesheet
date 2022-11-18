using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Helpers;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace computan.timesheet.Controllers
{
    [CustomeAuthorizeAttribute]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contacts
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetData(bool? filter, long? id)
        {
            try
            {
                string draw = Request.Params.GetValues("draw")[0];
                int start = Convert.ToInt32(Request.Params.GetValues("start")[0]);
                int lenght = Convert.ToInt32(Request.Params.GetValues("length")[0]);
                string searchValue = Request["search[value]"];
                List<Contact> contacts = new List<Contact>();
                if (id == null)
                {
                    switch (filter)
                    {
                        case true:
                            contacts = db.Contact.Include(x => x.ContactCompany).Where(x => x.isactive == true)
                                .ToList();
                            break;

                        case false:
                            contacts = db.Contact.Include(x => x.ContactCompany).Where(x => x.isactive == false)
                                .ToList();
                            break;

                        default:
                            contacts = db.Contact.Include(x => x.ContactCompany).ToList();
                            break;
                    }
                }
                else
                {
                    switch (filter)
                    {
                        case true:
                            contacts = db.Contact.Include(x => x.ContactCompany)
                                .Where(x => x.isactive == true && x.contactdomainid == id).ToList();
                            break;

                        case false:
                            contacts = db.Contact.Include(x => x.ContactCompany)
                                .Where(x => x.isactive == false && x.contactdomainid == id).ToList();
                            break;

                        default:
                            contacts = db.Contact.Include(x => x.ContactCompany).Where(x => x.contactdomainid == id)
                                .ToList();
                            break;
                    }
                }

                int TotalRecordsCount = contacts.Count();
                if (!string.IsNullOrEmpty(searchValue))
                {
                    string[] searcharray = searchValue.Split(' ').ToArray();
                    contacts = (from c in contacts
                                where c.DisplayName != null &&
                                      searcharray.Any(val => c.DisplayName.ToLower().Contains(val.ToLower()))
                                      || c.Email != null && searcharray.Any(val => c.Email.ToLower().Contains(val.ToLower()))
                                select c).ToList();
                }

                int FilteredRecordCount = contacts.Count();
                if (lenght > 0)
                {
                    contacts = contacts.Skip(start).Take(lenght).ToList();
                }

                List<ContactsViewModels> listContacts = new List<ContactsViewModels>();
                foreach (Contact contact in contacts)
                {
                    ContactsViewModels cvm = new ContactsViewModels
                    {
                        id = contact.Id,
                        Email = contact.Email,
                        DisplayName = contact.DisplayName,
                        isactive = contact.isactive,
                        createdonutc = contact.createdonutc
                    };
                    listContacts.Add(cvm);
                }

                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = TotalRecordsCount,
                    recordsFiltered = FilteredRecordCount,
                    data = listContacts
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    data = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Contacts/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = db.Contact.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,DisplayName,Email,isactive")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                Contact objcontact = db.Contact.Where(x => x.Email == contact.Email).AsQueryable().FirstOrDefault();
                if (objcontact != null)
                {
                    ModelState.AddModelError("Email", "Sorry, Email Address Already exist");
                    return View(contact);
                }

                string emailAddress = contact.Email;
                // Add/Update ContactCompany
                if (!string.IsNullOrEmpty(contact.Email))
                {
                    string[] emailElement = emailAddress.Split('@');

                    string Contactdomain = string.Empty;
                    if (emailElement.Length > 0)
                    {
                        Contactdomain = emailElement[1];
                    }

                    // Add ContactCompany if doesn't exists.
                    ContactCompany objContactCompany = db.ContactCompany.Where(x => x.name == Contactdomain).AsQueryable().FirstOrDefault();
                    if (objContactCompany == null)
                    {
                        objContactCompany = new ContactCompany
                        {
                            name = Contactdomain
                        };
                        db.ContactCompany.Add(objContactCompany);
                        db.SaveChanges();
                    }

                    // Add Contact if doesn't exists.
                    Contact objContact = db.Contact.Where(x => x.Email == emailAddress).AsQueryable().FirstOrDefault();

                    if (objContact == null)
                    {
                        contact.contactdomainid = objContactCompany.id;
                        contact.createdonutc = DateTime.Now;
                        contact.updatedonutc = DateTime.Now;
                        contact.ipused = Request.UserHostAddress;
                        contact.userid = User.Identity.GetUserId();
                        db.Contact.Add(contact);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }

            return View(contact);
        }

        // GET: Contacts/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = db.Contact.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(
            [Bind(Include = "Id,FirstName,LastName,DisplayName,Email,isactive,createdonutc,updatedonutc,ipused,userid")]
            Contact contact)
        {
            if (ModelState.IsValid)
            {
                List<Contact> objcontactlist = db.Contact.Where(x => x.Email == contact.Email).ToList();
                if (objcontactlist != null && objcontactlist.Count > 1)
                {
                    ModelState.AddModelError("Email", "Sorry, Email Address Already exist");
                    return View(contact);
                }

                string emailAddress = contact.Email;
                // Add/Update ContactCompany
                if (!string.IsNullOrEmpty(contact.Email))
                {
                    string[] emailElement = emailAddress.Split('@');

                    string Contactdomain = string.Empty;
                    if (emailElement.Length > 0)
                    {
                        Contactdomain = emailElement[1];
                    }

                    // Add ContactCompany if doesn't exists.
                    ContactCompany objContactCompany = db.ContactCompany.Where(x => x.name == Contactdomain).FirstOrDefault();
                    if (objContactCompany == null)
                    {
                        objContactCompany = new ContactCompany
                        {
                            name = Contactdomain
                        };
                        db.ContactCompany.Add(objContactCompany);
                        db.SaveChanges();
                    }

                    // Add Contact if doesn't exists.
                    Contact objContact = db.Contact.Where(x => x.Email.ToUpper() == emailAddress.ToUpper())
                        .FirstOrDefault();

                    if (objContact == null)
                    {
                        contact.contactdomainid = objContactCompany.id;
                        contact.createdonutc = DateTime.Now;
                        contact.updatedonutc = DateTime.Now;
                        contact.ipused = Request.UserHostAddress;
                        contact.userid = User.Identity.GetUserId();
                        db.Contact.Add(contact);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                    objContact.FirstName = contact.FirstName;
                    objContact.LastName = contact.LastName;
                    objContact.DisplayName = contact.DisplayName;
                    objContact.Email = contact.Email;
                    objContact.isactive = contact.isactive;
                    objContact.contactdomainid = objContactCompany.id;
                    objContact.createdonutc = DateTime.Now;
                    objContact.updatedonutc = DateTime.Now;
                    objContact.ipused = Request.UserHostAddress;
                    objContact.userid = User.Identity.GetUserId();

                    db.Entry(objContact).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(contact);
        }

        // GET: Contacts/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contact contact = db.Contact.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Contact contact = db.Contact.Find(id);
            db.Contact.Remove(contact);
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