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

            var qry = (from cu in db.Salespersons
                       join cc in db.CompanyCustomers on cu.SalespersonId equals cc.SalespersonId
                       join co in db.Companies on cc.CompanyId equals co.CompanyId
                       where co.CompanyId == user.CompanyId
                       select new { cu }).ToList();

            var salespersons = new List<Salesperson>();
            foreach (var item in qry)
            {
                salespersons.Add(item.cu);
            }

            
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
            return View();
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
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Salespersons.Add(salesperson);
                        var response = DBHelper.SaveChanges(db);
                        if (!response.IsSucces)
                        {
                            ModelState.AddModelError(string.Empty, response.Message);
                            transaction.Rollback();
                            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name", salesperson.CityId);
                            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", salesperson.DepartmentId);
                            return View(salesperson);
                        }

                        UsersHelper.CreateUserASP(salesperson.UserName, "Salesperson");

                        var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                        var companyCustomer = new CompanyCustomer
                        {
                            CompanyId = user.CompanyId,
                            SalespersonId = salesperson.SalespersonId,
                        };

                        db.CompanyCustomers.Add(companyCustomer);
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Index");                        
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, ex.Message);

                    }
                }
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
                var response = DBHelper.SaveChanges(db);
                if (response.IsSucces)
                {
                    UsersHelper.CreateUserASP(salesperson.UserName, "Salesperson");
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
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
            var salesperson = db.Salespersons.Find(id);
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var companyCustomer = db.CompanyCustomers.Where(cc => cc.CompanyId == user.CompanyId && cc.SalespersonId == salesperson.SalespersonId).FirstOrDefault();


            using (var transaction = db.Database.BeginTransaction())
            {
                db.CompanyCustomers.Remove(companyCustomer);
                db.Salespersons.Remove(salesperson);
                var response = DBHelper.SaveChanges(db);

                if (response.IsSucces)
                {
                    transaction.Commit();
                    return RedirectToAction("Index");
                }

                transaction.Rollback();
                ModelState.AddModelError(string.Empty, response.Message);
                return View(salesperson); 
            }
            
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
