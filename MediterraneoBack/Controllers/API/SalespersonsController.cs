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
using MediterraneoBack.Models;

namespace MediterraneoBack.Controllers.API
{
    public class SalespersonsController : ApiController
    {
        private MediterraneoContext db = new MediterraneoContext();

        // GET: api/Salespersons
        public IQueryable<Salesperson> GetSalespersons()
        {
            return db.Salespersons;
        }

        // GET: api/Salespersons/5
        [ResponseType(typeof(Salesperson))]
        public async Task<IHttpActionResult> GetSalesperson(int id)
        {
            Salesperson salesperson = await db.Salespersons.FindAsync(id);
            if (salesperson == null)
            {
                return NotFound();
            }

            return Ok(salesperson);
        }

        // PUT: api/Salespersons/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSalesperson(int id, Salesperson salesperson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesperson.SalespersonId)
            {
                return BadRequest();
            }

            db.Entry(salesperson).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalespersonExists(id))
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

        // POST: api/Salespersons
        [ResponseType(typeof(Salesperson))]
        public async Task<IHttpActionResult> PostSalesperson(Salesperson salesperson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Salespersons.Add(salesperson);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = salesperson.SalespersonId }, salesperson);
        }

        // DELETE: api/Salespersons/5
        [ResponseType(typeof(Salesperson))]
        public async Task<IHttpActionResult> DeleteSalesperson(int id)
        {
            Salesperson salesperson = await db.Salespersons.FindAsync(id);
            if (salesperson == null)
            {
                return NotFound();
            }

            db.Salespersons.Remove(salesperson);
            await db.SaveChangesAsync();

            return Ok(salesperson);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalespersonExists(int id)
        {
            return db.Salespersons.Count(e => e.SalespersonId == id) > 0;
        }
    }
}