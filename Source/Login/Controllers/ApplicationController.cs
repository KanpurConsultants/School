using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Login.Models;

namespace Login.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        [HttpGet]
        public ActionResult ApplicationSelection()
        {
            return View(db.Application.ToList());
        }
        
        public ActionResult SelectedApplication(int id)
        {
            var AppRecord= (from p in db.Application
                                       where p.ApplicationId==id
                                       select p).FirstOrDefault();

            //"Data Source=ITSERVER;Initial Catalog=RUG;Integrated Security=False;User Id=sa; pwd="

            System.Web.HttpContext.Current.Session["DefaultConnectionString"] = AppRecord.ConnectionString;

            
                return Redirect(AppRecord.ApplicationDefaultPage);
            
            
            
        }
        //// GET: /Application/
        //public ActionResult Index()
        //{           
        //    return View(db.Application.ToList());
        //}

        //// GET: /Application/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Application Application = db.Application.Find(id);
        //    if (Application == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(Application);
        //}

        //// GET: /Application/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: /Application/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="ApplicationId,Name")] Application Application)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Application.Add(Application);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(Application);
        //}

        //// GET: /Application/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Application Application = db.Application.Find(id);
        //    if (Application == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(Application);
        //}

        //// POST: /Application/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="ApplicationId,Name")] Application Application)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(Application).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(Application);
        //}

        //// GET: /Application/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Application Application = db.Application.Find(id);
        //    if (Application == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(Application);
        //}

        //// POST: /Application/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Application Application = db.Application.Find(id);
        //    db.Application.Remove(Application);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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