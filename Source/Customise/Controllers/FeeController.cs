using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Surya.India.Model.Models;
using Surya.India.Data.Models;
using Surya.India.Service;
using Surya.India.Data.Infrastructure;
using Surya.India.Presentation.ViewModels;
using Surya.India.Presentation;
using Surya.India.Core.Common;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace Surya.India.Web
{
    [Authorize]
    public class FeeController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        IFeeService _FeeService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public FeeController(IFeeService FeeService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FeeService = FeeService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /FeeMaster/
        
        public ActionResult Index()
        {
            var Fee = _FeeService.GetFeeList().ToList();
            return View(Fee);
            //return RedirectToAction("Create");
        }

        // GET: /FeeMaster/Create
        
        public ActionResult Create()
        {
            Sch_Fee vm = new Sch_Fee();
            vm.IsActive = true;
            return View("Create", vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Sch_Fee vm)
        {
            Sch_Fee pt = vm;
            if (ModelState.IsValid)
            {


                if (vm.FeeId <= 0)
                {
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    _FeeService.Create(pt);

                    ActivityLog log = new ActivityLog()
                    {
                        ActivityType = (int)(ActivityTypeContants.Added),
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        DocId = pt.FeeId,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Fee).DocumentTypeId,
                        Narration = "A new Fee is created with the Id " + pt.FeeId,
                    };

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", vm);

                    }


                    return RedirectToAction("Create").Success("Data saved successfully");
                }
                else
                {

                    Sch_Fee temp = _FeeService.Find(pt.FeeId);
                    temp.FeeName = pt.FeeName;
                    temp.IsActive = pt.IsActive;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    _FeeService.Update(temp);

                    ActivityLog log = new ActivityLog()
                    {
                        ActivityType = (int)(ActivityTypeContants.Modified),
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        DocId = pt.FeeId,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Fee).DocumentTypeId,
                        Narration = "Delivery Terms is modified with the name" + pt.FeeName,
                    };

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", pt);

                    }


                    return RedirectToAction("Index").Success("Data saved successfully");


                }








            }
            return View("Create", vm);
        }


        // GET: /ProductMaster/Edit/5
        
        public ActionResult Edit(int id)
        {
            Sch_Fee pt = _FeeService.Find(id);
            if (pt == null)
            {
                return HttpNotFound();
            }
            return View("Create", pt);
        }

        // POST: /ProductMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Fee pt)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Fee temp = _FeeService.Find(pt.FeeId);
        //        temp.FeeName = pt.FeeName;
        //        temp.IsActive = pt.IsActive;
        //        temp.ModifiedDate = DateTime.Now;
        //        temp.ModifiedBy = User.Identity.Name;
        //        temp.ObjectState = Model.ObjectState.Modified;
        //        _FeeService.Update(temp);

        //        try
        //        {
        //            _unitOfWork.Save();
        //        }
        //        catch (DbEntityValidationException dbex)
        //        {
        //            string message = _exception.HandleEntityValidationException(dbex);
        //            ModelState.AddModelError("", message);
        //            return View("Create", pt);
        //        }
        //        catch (DbUpdateException du)
        //        {
        //            string message = _exception.HandleUpdateException(du);
        //            ModelState.AddModelError("", message);
        //            return View("Create", pt);
        //        }
        //        catch (Exception ex)
        //        {
        //            string message = _exception.HandleException(ex);
        //            ModelState.AddModelError("", message);
        //            return View("Create", pt);

        //        }


        //        return RedirectToAction("Index").Success("Data saved successfully");
        //    }
        //    return View("Create", pt);
        //}

        // GET: /ProductMaster/Delete/5
        
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sch_Fee Fee = db.Sch_Fee.Find(id);
            if (Fee == null)
            {
                return HttpNotFound();
            }
            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }

        // POST: /ProductMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var temp = _FeeService.Find(vm.id);
                ActivityLog al = new ActivityLog()
                {
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DocId = vm.id,
                    UserRemark = vm.Reason,
                    Narration = "Delivery terms is deleted with Name:" + temp.FeeName,
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Fee).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);


                _FeeService.Delete(vm.id);

                try
                {
                    _unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ModelState.AddModelError("", message);
                    return PartialView("_Reason", vm);

                }


                return Json(new { success = true });

            }
            return PartialView("_Reason", vm);
        }

        [HttpGet]
        public ActionResult NextPage(int id)//CurrentHeaderId
        {
            var nextId = _FeeService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id)//CurrentHeaderId
        {
            var nextId = _FeeService.PrevId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }

        [HttpGet]
        public ActionResult History()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }
        [HttpGet]
        public ActionResult Print()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }
        [HttpGet]
        public ActionResult Email()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        [HttpGet]
        public ActionResult Report()
        {

            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Fee);

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

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
