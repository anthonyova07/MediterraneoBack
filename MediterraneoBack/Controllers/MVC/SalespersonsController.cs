using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MediterraneoBack.Classes;
using MediterraneoBack.Models;

namespace MediterraneoBack.Controllers
{
    [Authorize(Roles ="User")]
    public class SalespersonsController : Controller
    {
        private MediterraneoContext db = new MediterraneoContext();

        // GET: Salespersons
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var salespersons = db.Salespersons.Where(s => s.CompanyId == user.CompanyId).Include(s => s.City).Include(s => s.Department);
            return View(salespersons.ToList());
        }

        // GET: Salespersons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var salesperson = db.Salespersons.Find(id);

            if (salesperson == null)
            {
                return HttpNotFound();
            }
            return View(salesperson);
        }

        // GET: Salespersons/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name");
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name");
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var salesperson = new Salesperson { CompanyId = user.CompanyId, };
            return View(salesperson);
        }

        // POST: Salespersons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Salesperson salesperson)
        {
            if (ModelState.IsValid)
            {
                db.Salespersons.Add(salesperson);
                db.SaveChanges();
                UsersHelper.CreateUserASP(salesperson.UserName, "Salesperson");
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name", salesperson.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", salesperson.DepartmentId);
            return View(salesperson);
        }

        // GET: Salespersons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var  salesperson = db.Salespersons.Find(id);

            if (salesperson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name", salesperson.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", salesperson.DepartmentId);
            return View(salesperson);
        }

        // POST: Salespersons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Salesperson salesperson)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salesperson).State = EntityState.Modified;
                db.SaveChanges();
                // TODO: Validate when the salesperson email change
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name", salesperson.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", salesperson.DepartmentId);
            return View(salesperson);
        }

        // GET: Salespersons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var salesperson = db.Salespersons.Find(id);

            if (salesperson == null)
            {
                return HttpNotFound();
            }
            return View(salesperson);
        }

        // POST: Salespersons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Salesperson salesperson = db.Salespersons.Find(id);

            db.Salespersons.Remove(salesperson);
            db.SaveChanges();
            UsersHelper.DeleteUser(salesperson.UserName);
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
