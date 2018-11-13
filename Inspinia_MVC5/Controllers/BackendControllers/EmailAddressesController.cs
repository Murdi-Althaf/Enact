using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Enact.Models;

namespace Enact.Controllers.BackendControllers
{
    public class EmailAddressesController : Controller
    {
        private EnactEntities db = new EnactEntities();

        // GET: /EmailAddresses/
        public async Task<ActionResult> Index()
        {
            return View(await db.EmailAddresses.ToListAsync());
        }

        // GET: /EmailAddresses/Details/5
        public async Task<ActionResult> Send(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailAddress emailAddress = await db.EmailAddresses.FindAsync(id);
            if (emailAddress == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", "EmailAddresses");
        }

        // GET: /EmailAddresses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EmailAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="ID,Email_address,Completed_survey,Sent_voucher,Trial_ID")] EmailAddress emailAddress)
        {
            if (ModelState.IsValid)
            {
                db.EmailAddresses.Add(emailAddress);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(emailAddress);
        }

        // GET: /EmailAddresses/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailAddress emailAddress = await db.EmailAddresses.FindAsync(id);
            if (emailAddress == null)
            {
                return HttpNotFound();
            }
            return View(emailAddress);
        }

        // POST: /EmailAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="ID,Email_address,Completed_survey,Sent_voucher,Trial_ID")] EmailAddress emailAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailAddress).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(emailAddress);
        }

        // GET: /EmailAddresses/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailAddress emailAddress = await db.EmailAddresses.FindAsync(id);
            if (emailAddress == null)
            {
                return HttpNotFound();
            }
            return View(emailAddress);
        }

        // POST: /EmailAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            EmailAddress emailAddress = await db.EmailAddresses.FindAsync(id);
            db.EmailAddresses.Remove(emailAddress);
            await db.SaveChangesAsync();
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
