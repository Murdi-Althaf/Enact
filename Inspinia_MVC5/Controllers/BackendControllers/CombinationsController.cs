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
    public class CombinationsController : Controller
    {
        private EnactEntities db = new EnactEntities();

        // GET: /Combinations/
        public async Task<ActionResult> Index()
        {
            return View(await db.Combinations.ToListAsync());
        }

        // GET: /Combinations/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Combination combination = await db.Combinations.FindAsync(id);
            if (combination == null)
            {
                return HttpNotFound();
            }
            return View(combination);
        }

        // GET: /Combinations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Combinations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Combination_ID,Modification_A,Modification_B,Modification_C,Modification_D,Modification_E,Modification_F")] Combination combination)
        {
            if (ModelState.IsValid)
            {
                db.Combinations.Add(combination);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(combination);
        }

        // GET: /Combinations/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Combination combination = await db.Combinations.FindAsync(id);
            if (combination == null)
            {
                return HttpNotFound();
            }
            return View(combination);
        }

        // POST: /Combinations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Combination_ID,Modification_A,Modification_B,Modification_C,Modification_D,Modification_E,Modification_F")] Combination combination)
        {
            if (ModelState.IsValid)
            {
                db.Entry(combination).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(combination);
        }

        // GET: /Combinations/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Combination combination = await db.Combinations.FindAsync(id);
            if (combination == null)
            {
                return HttpNotFound();
            }
            return View(combination);
        }

        // POST: /Combinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Combination combination = await db.Combinations.FindAsync(id);
            db.Combinations.Remove(combination);
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
