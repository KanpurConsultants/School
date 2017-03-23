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
    public class ClassSectionController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IClassSectionService _ClassSectionService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public ClassSectionController(IClassSectionService ClassSectionService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _ClassSectionService = ClassSectionService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        [HttpGet]
        public JsonResult Index(int ProgramId, int ClassId, int StreamId)
        {
            var p = _ClassSectionService.GetClassSectionList(ProgramId, ClassId, StreamId);                               
            return Json(p, JsonRequestBehavior.AllowGet);
            
        }


        private void PrepareViewBag()
        {

        }

        public ActionResult _Create(int ProgramId, int ClassId, int StreamId) //Id ==>Sale Order Header Id
        {
            Sch_ClassSection s = new Sch_ClassSection();
            s.ProgramId = ProgramId;
            s.ClassId = ClassId;
            s.StreamId = StreamId;

            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_ClassSection svm)
        {
            if (ModelState.IsValid)
            {        
                if(svm.ClassSectionId == 0)
                {
                    svm.CreatedDate = DateTime.Now;
                    svm.ModifiedDate = DateTime.Now;
                    svm.CreatedBy = User.Identity.Name;
                    svm.ModifiedBy = User.Identity.Name;
                    svm.ObjectState = Model.ObjectState.Added;
                    _ClassSectionService.Create(svm);


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
                    return RedirectToAction("_Create", new { ProgramId = svm.ProgramId, ClassId = svm.ClassId, StreamId = svm.StreamId });
                }
                else
                {
                    Sch_ClassSection ClassSectionDetail = _ClassSectionService.Find(svm.ClassSectionId);
                    StringBuilder logstring = new StringBuilder();

                    ClassSectionDetail.ProgramId = svm.ProgramId;
                    ClassSectionDetail.ClassId = svm.ClassId;
                    ClassSectionDetail.StreamId = svm.StreamId;
                    ClassSectionDetail.SectionName = svm.SectionName;

                    ClassSectionDetail.ModifiedDate = DateTime.Now;
                    ClassSectionDetail.ModifiedBy = User.Identity.Name;
                    ClassSectionDetail.ObjectState = Model.ObjectState.Modified;
                    _ClassSectionService.Update(ClassSectionDetail);


                    //Saving the Activity Log
                        ActivityLog al = new ActivityLog()
                        {
                            ActivityType = (int)ActivityTypeContants.Modified,
                            DocId = ClassSectionDetail.ClassSectionId,
                            CreatedDate = DateTime.Now,
                            Narration = logstring.ToString(),
                            CreatedBy = User.Identity.Name,
                            //DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.ClassSectionDetail).DocumentTypeId,
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
            return PartialView("_Create",svm);
        }

        
        public ActionResult _Edit(int id)
        {
            Sch_ClassSection s = _ClassSectionService.Find(id);


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
            Sch_ClassSection ClassSectionDetail = _ClassSectionService.Find(id);
            if (ClassSectionDetail == null)
            {
                return HttpNotFound();
            }
            return View(ClassSectionDetail);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(Sch_ClassSection vm)
        {
            Sch_ClassSection ClassSectionDetail = _ClassSectionService.Find(vm.ClassSectionId);
            _ClassSectionService.Delete(vm.ClassSectionId);

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
