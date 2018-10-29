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

namespace MediterraneoBack.Controllers.MVC
{
    [Authorize(Roles = "User")]
    public class ConditionsController : Controller
    {
        private MediterraneoContext db = new MediterraneoContext();

        // GET: Conditions
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");

            }
            var conditions = db.Conditions.Where(t => t.CompanyId == user.CompanyId);
            return View(conditions.ToList());
        }

        // GET: Conditions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }

        // GET: Conditions/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");

            }
            //ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name");
            var condition = new Condition { CompanyId = user.CompanyId, };
            return View(condition);
        }

        // POST: Conditions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Condition condition)
        {
            if (ModelState.IsValid)
            {
                db.Conditions.Add(condition);
                var response = DBHelper.SaveChanges(db);
                if (response.IsSucces)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(condition);
        }

        // GET: Conditions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", condition.CompanyId);
            return View(condition);
        }

        // POST: Conditions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Condition condition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(condition).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.IsSucces)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            // ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", tax.CompanyId);
            return View(condition);
        }

        // GET: Conditions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }

        // POST: Conditions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var condition = db.Conditions.Find(id);
            db.Conditions.Remove(condition);
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
