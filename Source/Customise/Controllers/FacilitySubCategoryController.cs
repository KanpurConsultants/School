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
    public class FacilitySubCategoryController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFacilitySubCategoryService _FacilitySubCategoryService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FacilitySubCategoryController(IFacilitySubCategoryService FacilitySubCategoryService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FacilitySubCategoryService = FacilitySubCategoryService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _FacilitySubCategoryService.GetFacilitySubCategoryList(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }


        private void PrepareViewBag()
        {

        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {
            Sch_FacilitySubCategory s = new Sch_FacilitySubCategory();
            s.FacilityId = Id;
            s.IsActive = true;

            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_FacilitySubCategory svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.FacilitySubCategoryId == 0)
                {
                    svm.CreatedDate = DateTime.Now;
                    svm.ModifiedDate = DateTime.Now;
                    svm.CreatedBy = User.Identity.Name;
                    svm.ModifiedBy = User.Identity.Name;
                    svm.ObjectState = Model.ObjectState.Added;
                    _FacilitySubCategoryService.Create(svm);


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
                    return RedirectToAction("_Create", new { id = svm.FacilityId });
                }
                else
                {
                    Sch_FacilitySubCategory FacilitySubCategory = _FacilitySubCategoryService.Find(svm.FacilitySubCategoryId);
                    StringBuilder logstring = new StringBuilder();

                    FacilitySubCategory.FacilityId = svm.FacilityId;
                    FacilitySubCategory.FacilitySubCategoryName = svm.FacilitySubCategoryName;
                    FacilitySubCategory.IsActive = svm.IsActive;

                    FacilitySubCategory.ModifiedDate = DateTime.Now;
                    FacilitySubCategory.ModifiedBy = User.Identity.Name;
                    FacilitySubCategory.ObjectState = Model.ObjectState.Modified;
                    _FacilitySubCategoryService.Update(FacilitySubCategory);


                    //Saving the Activity Log
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = FacilitySubCategory.FacilitySubCategoryId,
                        CreatedDate = DateTime.Now,
                        Narration = logstring.ToString(),
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.FacilitySubCategory).DocumentTypeId,
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
            Sch_FacilitySubCategory s = _FacilitySubCategoryService.Find(id);


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
            Sch_FacilitySubCategory FacilitySubCategory = _FacilitySubCategoryService.Find(id);
            if (FacilitySubCategory == null)
            {
                return HttpNotFound();
            }
            return View(FacilitySubCategory);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(Sch_FacilitySubCategory vm)
        {
            Sch_FacilitySubCategory FacilitySubCategory = _FacilitySubCategoryService.Find(vm.FacilitySubCategoryId);
            _FacilitySubCategoryService.Delete(vm.FacilitySubCategoryId);

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
