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
using PagedList;

namespace MediterraneoBack.Controllers
{
    [Authorize(Roles ="User, Admin")]
    public class ProductsController : Controller
    {
        private MediterraneoContext db = new MediterraneoContext();

        // GET: Products
        //int? page = null
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var user = db.Users
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefault();


            var products = db.Products
                .Include(p => p.Category)
                .Include(p => p.Tax) 
                .Where(p => p.CompanyId == user.CompanyId);

            return View(products
                .ToList()
                .ToPagedList((int)page, 10));

            //var products = db.Products
            //    .Where(p => p.CompanyId == user.CompanyId);
            // return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(user.CompanyId), "CategoryId", "Description");
            ViewBag.TaxId = new SelectList(CombosHelper.GetTaxes(user.CompanyId), "TaxId", "Description");
            var product = new Product { CompanyId = user.CompanyId, };
            return View(product);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Products";

                if (product.ImageFile != null)
                {
                    pic = FilesHelper.UploadPhoto(product.ImageFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }

                product.Image = pic;
                db.Products.Add(product);
                var response = DBHelper.SaveChanges(db);
                if (response.IsSucces)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(user.CompanyId), "CategoryId", "Description", product.CategoryId);
            ViewBag.TaxId = new SelectList(CombosHelper.GetTaxes(user.CompanyId), "TaxId", "Description", product.TaxId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(product.CompanyId), "CategoryId", "Description", product.CategoryId);
            ViewBag.TaxId = new SelectList(CombosHelper.GetTaxes(product.CompanyId), "TaxId", "Description", product.TaxId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    var file = string.Format("{0}.png", product.ProductId);
                    var folder = "~/Content/Products";
                    var response = FilesHelper.UploadPhoto(product.ImageFile, folder);
                    product.Image = string.Format("{0}/{1}", folder, file);
                }
                //product.Image = file;
                db.Entry(product).State = EntityState.Modified;
                var responses = DBHelper.SaveChanges(db);
                if (responses.IsSucces)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, responses.Message);
            }
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(product.CompanyId), "CategoryId", "Description", product.CategoryId);
            ViewBag.TaxId = new SelectList(CombosHelper.GetTaxes(product.CompanyId), "TaxId", "Description", product.TaxId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
