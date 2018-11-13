using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Enact.Models;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Services;

namespace Enact.Controllers.Cronjob
{
    public class TrialRunsController : ApiController
    {
        private EnactEntities db = new EnactEntities();

        // GET: api/TrialRuns
        [WebMethod]
        public void GetTrialRuns()
        {

           List<TrialRun> trialrunlist= db.TrialRuns.Where(x=>x.EndDate==null).ToList();
            foreach (TrialRun tr in trialrunlist)
            {
                long id = tr.Allocation_ID;
                Allocation a = db.Allocations.Find(id);
                a.isAllocated = false;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        // GET: api/TrialRuns/5
        [ResponseType(typeof(TrialRun))]
        public async Task<IHttpActionResult> GetTrialRun(long id)
        {
            TrialRun trialRun = await db.TrialRuns.FindAsync(id);
            if (trialRun == null)
            {
                return NotFound();
            }

            return Ok(trialRun);
        }

        // PUT: api/TrialRuns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTrialRun(long id, TrialRun trialRun)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trialRun.ID)
            {
                return BadRequest();
            }

            db.Entry(trialRun).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrialRunExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TrialRuns
        [ResponseType(typeof(TrialRun))]
        public async Task<IHttpActionResult> PostTrialRun(TrialRun trialRun)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TrialRuns.Add(trialRun);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = trialRun.ID }, trialRun);
        }

        // DELETE: api/TrialRuns/5
        [ResponseType(typeof(TrialRun))]
        public async Task<IHttpActionResult> DeleteTrialRun(long id)
        {
            TrialRun trialRun = await db.TrialRuns.FindAsync(id);
            if (trialRun == null)
            {
                return NotFound();
            }

            db.TrialRuns.Remove(trialRun);
            await db.SaveChangesAsync();

            return Ok(trialRun);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TrialRunExists(long id)
        {
            return db.TrialRuns.Count(e => e.ID == id) > 0;
        }
    }
}