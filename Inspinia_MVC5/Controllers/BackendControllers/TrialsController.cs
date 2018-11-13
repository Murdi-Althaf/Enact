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
    public class TrialsController : Controller
    {
        private EnactEntities db = new EnactEntities();

        // GET: /Trials/
        public async Task<ActionResult> Index()
        {
            List<TrialView> viewtriallist = new List<TrialView>();
            TrialView viewtrial;
            List<Trial> triallist = await db.Trials.ToListAsync();
            foreach (Trial t in triallist)
            {
                viewtrial = new TrialView();
                viewtrial.audit = db.Audits.Find(t.Audit_ID);
                viewtrial.trial = t;
                viewtrial.run = db.TrialRuns.Where(x => x.Trial_ID == t.Trial_ID).FirstOrDefault();
                long allID = viewtrial.run.Allocation_ID;
                viewtrial.allocation = db.Allocations.Where(x => x.Allocation_ID == allID).FirstOrDefault();
                viewtriallist.Add(viewtrial);
            }

            return View(viewtriallist);
        }


        public async Task<ActionResult> survey(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Details", "Surveys", new { @id = id });
        }

        public async Task<ActionResult> allocation(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Details", "Allocations", new { @id = id });
        }

       /* public async Task<ActionResult> analytics(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Details", "Allocations", new { @id = id });
        }*/

        // GET: /Trials/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trial trial = await db.Trials.FindAsync(id);
            if (trial == null)
            {
                return HttpNotFound();
            }
            return View(trial);
        }

        // GET: /Trials/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Trials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Trial_ID,Audit_ID")] Trial trial)
        {
            if (ModelState.IsValid)
            {
                db.Trials.Add(trial);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(trial);
        }

        // GET: /Trials/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trial trial = await db.Trials.FindAsync(id);
            if (trial == null)
            {
                return HttpNotFound();
            }
            return View(trial);
        }

        // POST: /Trials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Trial_ID,Audit_ID")] Trial trial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trial).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(trial);
        }

        // GET: /Trials/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trial trial = await db.Trials.FindAsync(id);
            if (trial == null)
            {
                return HttpNotFound();
            }
            return View(trial);
        }

        // POST: /Trials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Trial trial = await db.Trials.FindAsync(id);
            db.Trials.Remove(trial);
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
