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
using Surya.India.Core.Common;
using System.Net.Http;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using Surya.India.Presentation.ViewModels;
using Surya.India.Model.ViewModels;
using AutoMapper;
using System.Configuration;
using Surya.India.Presentation.Helper;
using Surya.India.Presentation;
using System.Text;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;

namespace Surya.India.Web
{

    [Authorize]
    public class ClassFeeLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IClassFeeLineService _ClassFeeLineService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public ClassFeeLineController(IClassFeeLineService ClassFeeLineService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _ClassFeeLineService = ClassFeeLineService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _ClassFeeLineService.GetClassFeeLineListForIndex(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }


        private void PrepareViewBag()
        {

        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {
            Sch_ClassFeeLine s = new Sch_ClassFeeLine();
            s.ClassFeeId  = Id;
            s.IsActive = true;

            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_ClassFeeLine svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.ClassFeeLineId == 0)
                {
                    svm.CreatedDate = DateTime.Now;
                    svm.ModifiedDate = DateTime.Now;
                    svm.CreatedBy = User.Identity.Name;
                    svm.ModifiedBy = User.Identity.Name;
                    svm.ObjectState = Model.ObjectState.Added;
                    _ClassFeeLineService.Create(svm);


                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return PartialView("_Create", svm);
                    }
                    return RedirectToAction("_Create", new { id = svm.ClassFeeId });
                }
                else
                {
                    Sch_ClassFeeLine ClassFeeLine = _ClassFeeLineService.Find(svm.ClassFeeLineId);
                    StringBuilder logstring = new StringBuilder();

                    ClassFeeLine.FeeId = svm.FeeId;
                    ClassFeeLine.LedgerAccountId = svm.LedgerAccountId ;
                    ClassFeeLine.Amount = svm.Amount;
                    ClassFeeLine.Recurrence = svm.Recurrence;
                    ClassFeeLine.IsActive = svm.IsActive;

                    ClassFeeLine.ModifiedDate = DateTime.Now;
                    ClassFeeLine.ModifiedBy = User.Identity.Name;
                    ClassFeeLine.ObjectState = Model.ObjectState.Modified;
                    _ClassFeeLineService.Update(ClassFeeLine);


                    //Saving the Activity Log
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = ClassFeeLine.ClassFeeLineId,
                        CreatedDate = DateTime.Now,
                        Narration = logstring.ToString(),
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ClassFeeLine).DocumentTypeId,
                    };
                    new ActivityLogService(_unitOfWork).Create(al);
                    //End of Saving the Activity Log


                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return PartialView("_Create", svm);

                    }
                    return Json(new { success = true });
                }
            }

            PrepareViewBag();
            return PartialView("_Create", svm);
        }


        public ActionResult _Edit(int id)
        {
            Sch_ClassFeeLine s = _ClassFeeLineService.Find(id);


            PrepareViewBag();

            if (s == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Create", s);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sch_ClassFeeLine ClassFeeLine = _ClassFeeLineService.Find(id);
            if (ClassFeeLine == null)
            {
                return HttpNotFound();
            }
            return View(ClassFeeLine);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(Sch_ClassFeeLine vm)
        {
            Sch_ClassFeeLine ClassFeeLine = _ClassFeeLineService.Find(vm.ClassFeeLineId);
            _ClassFeeLineService.Delete(vm.ClassFeeLineId);

            try
            {
                _unitOfWork.Save();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("EditSize", vm);

            }
            return Json(new { success = true });
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
