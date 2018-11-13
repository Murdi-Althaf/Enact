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
using Enact.Models.BackendViewModels;

namespace Enact.Controllers.BackendControllers
{
    public class AllocationsController : Controller
    {
        private EnactEntities db = new EnactEntities();

        // GET: /Allocations/
        public async Task<ActionResult> Index()
        {
            List<AllocationView> viewallocationlist = new List<AllocationView>();
            AllocationView viewallocation;
            List <Allocation> orgallocation = await db.Allocations.ToListAsync();
            foreach (Allocation a in orgallocation)
            {
                viewallocation = new AllocationView();
                viewallocation.allocation = a;
                viewallocation.auditname = db.Audits.Find(a.Audit_ID).Name;
                viewallocationlist.Add(viewallocation);
            }

            return View(viewallocationlist);
        }

        // GET: /Allocations/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrialView viewtrial;
            Trial t = db.Trials.Find(id);
            viewtrial = new TrialView();
            viewtrial.audit = db.Audits.Find(t.Audit_ID);
            viewtrial.trial = t;
            viewtrial.run = db.TrialRuns.Where(x => x.Trial_ID == t.Trial_ID).FirstOrDefault();
            long allID = viewtrial.run.Allocation_ID;
            viewtrial.allocation = db.Allocations.Where(x => x.Allocation_ID == allID).FirstOrDefault();
            
            return View(viewtrial);
        }



        

        // GET: /Allocations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Allocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Allocation_ID,Block,isAllocated,Combination_ID,Audit_ID")] Allocation allocation)
        {
            if (ModelState.IsValid)
            {
                db.Allocations.Add(allocation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(allocation);
        }

        // GET: /Allocations/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Allocation allocation = await db.Allocations.FindAsync(id);
            if (allocation == null)
            {
                return HttpNotFound();
            }
            return View(allocation);
        }

        // POST: /Allocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Allocation_ID,Block,isAllocated,Combination_ID,Audit_ID")] Allocation allocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(allocation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(allocation);
        }

        // GET: /Allocations/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Allocation allocation = await db.Allocations.FindAsync(id);
            if (allocation == null)
            {
                return HttpNotFound();
            }
            return View(allocation);
        }

        // POST: /Allocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Allocation allocation = await db.Allocations.FindAsync(id);
            db.Allocations.Remove(allocation);
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
