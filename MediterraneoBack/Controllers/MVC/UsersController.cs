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
    public class UsersController : Controller
    {
        private MediterraneoContext db = new MediterraneoContext();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.City).Include(u => u.Company).Include(u => u.Department);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            var model = new User
            {
                Departments = db.Departments.ToList(),
                Cities = db.Cities.ToList(),
                Companies = db.Companies.ToList()
            };
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name");
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            return View(model);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( User user)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Users";

                if (user.PhotoFile != null)
                {
                    pic = FilesHelper.UploadPhoto(user.PhotoFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }
                user.Photo = pic;
                db.Users.Add(user);
                var response = DBHelper.SaveChanges(db);
                if (response.IsSucces)
                {
                    UsersHelper.CreateUserASP(user.UserName, "User");
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            //Comentario de prueba
            ViewBag.CityId = new SelectList(
                db.Cities.OrderBy(d => d.Name),
                "CityId",
                "Name",
                user.CityId);

            ViewBag.CompanyId = new SelectList(
                db.Companies.OrderBy(d => d.Name),
                "CompanyId",
                "Name",
                user.DepartmentId);

            ViewBag.DepartmentId = new SelectList(
                db.Departments.OrderBy(d => d.Name),
                "DepartmentId",
                "Name",
                user.DepartmentId);

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", user.CityId);
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", user.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", user.DepartmentId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {                
                if (user.PhotoFile != null)
                {
                    var file = string.Format("{0}.png", user.UserId);
                    var folder = "~/Content/Users";
                    var response = FilesHelper.UploadPhoto(user.PhotoFile, folder);
                    user.Photo = string.Format("{0}/{1}", folder, file);
                }

                var db2 = new MediterraneoContext();
                var currentUser = db2.Users.Find(user.UserId);
                if (currentUser.UserName != user.UserName)
                {
                    UsersHelper.UpdateUserName(currentUser.UserName, user.UserName);
                }
                db2.Dispose();            

                //user.Photo = pic;
                db.Entry(user).State = EntityState.Modified;
                var responses = DBHelper.SaveChanges(db);
                if (responses.IsSucces)
                {
                    UsersHelper.CreateUserASP(user.UserName, "User");
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, responses.Message);
            }

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", user.CityId);
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", user.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", user.DepartmentId);

            //ViewBag.CityId = new SelectList(
            //    db.Cities.OrderBy(d => d.Name),
            //    "CityId",
            //    "Name",
            //    user.CityId);

            //ViewBag.CompanyId = new SelectList(
            //    db.Companies.OrderBy(d => d.Name),
            //    "CompanyId",
            //    "Name",
            //    user.DepartmentId);

            //ViewBag.DepartmentId = new SelectList(
            //    db.Departments.OrderBy(d => d.Name),
            //    "DepartmentId",
            //    "Name",
            //    user.DepartmentId);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Se cambio User por var
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Se cambio User por var
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            UsersHelper.DeleteUser(user.UserName, "User");
            return RedirectToAction("Index");
        }

        public JsonResult GetCities(int departmentId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(c => c.DepartmentId == departmentId);
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
