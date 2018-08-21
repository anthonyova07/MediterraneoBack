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
    [Authorize(Roles = "Admin")]
    public class CompaniesController : Controller
    {
        private MediterraneoContext db = new MediterraneoContext();

        // GET: Companies
        public ActionResult Index()
        {
            return View(db.Companies.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            var model = new Company
            {
                Departments = db.Departments.ToList(),
                Cities = db.Cities.ToList()
            };
            return View(model);
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Logos";

                if (company.LogoFile != null)
                {
                    pic = FilesHelper.UploadPhoto(company.LogoFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }
                company.Logo = pic;
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(
                db.Departments.OrderBy(d => d.Name),
                "DepartmentId",
                "Name",
                company.DepartmentId);

            ViewBag.CityId = new SelectList(
                db.Cities.OrderBy(d => d.Name),
                "CityId",
                "Name",
                company.CityId);

            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", company.DepartmentId);
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", company.CityId);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyId,Name,Phone,Address,Logo,DepartmentId,CityId")] Company company)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Logos";

                if (company.LogoFile != null)
                {
                    pic = FilesHelper.UploadPhoto(company.LogoFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }
                company.Logo = pic;
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //if (ModelState.IsValid)
            //{
            //    var pic = string.Empty;
            //    var folder = "~/Content/Logos";

            //    if (company.LogoFile != null)
            //    {
            //        pic = FilesHelper.UploadPhoto(company.LogoFile, folder);
            //        pic = string.Format("{0}/{1}", folder, pic);
            //    }
            //    company.Logo = pic;
            //    db.Companies.Add(company);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", company.DepartmentId);
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", company.CityId);
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetCities(int departmentId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(m => m.DepartmentId == departmentId);
            return Json(cities);
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
