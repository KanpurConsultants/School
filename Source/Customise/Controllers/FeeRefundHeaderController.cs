using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Surya.India.Model.Models;
using Surya.India.Model.ViewModels;
using Surya.India.Data.Models;
using Surya.India.Data.Infrastructure;
using Surya.India.Service;
using Surya.India.Web;
using AutoMapper;
using Surya.India.Presentation.ViewModels;
using Surya.India.Presentation;
using Surya.India.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;
using System.Configuration;
using System.IO;
using ImageResizer;
using Surya.India.Model.ViewModel;
using Surya.India.Presentation.Helper;

namespace Surya.India.Web.Controllers
{
    [Authorize]
    public class FeeRefundHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFeeRefundHeaderService _FeeRefundHeaderService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FeeRefundHeaderController(IFeeRefundHeaderService FeeRefundHeaderService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FeeRefundHeaderService = FeeRefundHeaderService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /Order/
        public ActionResult Index()
        {
            var Boms = _FeeRefundHeaderService.GetFeeRefundHeaderListForIndex().ToList();
            return View(Boms);
        }

      

        [HttpGet]
        public ActionResult NextPage(int id, string name)//BomId
        {
            var nextId = _FeeRefundHeaderService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id, string name)//BomId
        {
            var nextId = _FeeRefundHeaderService.PrevId(id);
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
            Dt = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.DesignConsumption);

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }

        [HttpGet]
        public ActionResult Remove()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }




        public void PrepareViewBag(Sch_FeeRefundHeaderViewModel svm)
        {

        }



        public ActionResult ChooseType()
        {
            return PartialView("ChooseType");
        }
        [HttpGet]
        public ActionResult CopyFromExisting()
        {
            return PartialView("CopyFromExisting");
        }


        public ActionResult Create()
        {
            Sch_FeeRefundHeaderViewModel p = new Sch_FeeRefundHeaderViewModel();
            p.DocDate = DateTime.Now;
            p.DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDoctypeConstants.FeeRefund).DocumentTypeId;
            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".Sch_FeeRefundHeader", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            PrepareViewBag(p);
            return View("Create", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sch_FeeRefundHeaderViewModel svm)
        {
            if (ModelState.IsValid)
            {
                
                if (svm.FeeRefundHeaderId <= 0)
                {
                    Sch_FeeRefundHeader FeeRefundHeader = new Sch_FeeRefundHeader();
                    FeeRefundHeader.DocTypeId = svm.DocTypeId;
                    FeeRefundHeader.DocNo = svm.DocNo;
                    FeeRefundHeader.DocDate = svm.DocDate;
                    FeeRefundHeader.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    FeeRefundHeader.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                    FeeRefundHeader.StudentId = svm.StudentId;
                    FeeRefundHeader.CreatedDate = DateTime.Now;
                    FeeRefundHeader.ModifiedDate = DateTime.Now;
                    FeeRefundHeader.CreatedBy = User.Identity.Name;
                    FeeRefundHeader.ModifiedBy = User.Identity.Name;
                    FeeRefundHeader.ObjectState = Model.ObjectState.Added;
                    _FeeRefundHeaderService.Create(FeeRefundHeader);


                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View(svm);

                    }

                    return RedirectToAction("Edit", new { id = FeeRefundHeader.FeeRefundHeaderId }).Success("Data saved Successfully");
                }
                else
                {
                    Sch_FeeRefundHeader FeeRefundHeader = new FeeRefundHeaderService(_unitOfWork).Find(svm.FeeRefundHeaderId);
                    FeeRefundHeader.DocNo = svm.DocNo;
                    FeeRefundHeader.DocDate = svm.DocDate;
                    FeeRefundHeader.StudentId = svm.StudentId;
                    FeeRefundHeader.Remark = svm.Remark;
                    FeeRefundHeader.ModifiedDate = DateTime.Now;
                    FeeRefundHeader.ModifiedBy = User.Identity.Name;
                    _FeeRefundHeaderService.Update(FeeRefundHeader);


                    ////Saving Activity Log::
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = svm.FeeRefundHeaderId,
                        Narration = "",
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDoctypeConstants.FeeRefund).DocumentTypeId,

                    };
                    new ActivityLogService(_unitOfWork).Create(al);
                    //End of Saving ActivityLog

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", svm);
                    }
                    return RedirectToAction("Index").Success("Data saved successfully");
                }
            }
            PrepareViewBag(svm);
            return View(svm);
        }


        public ActionResult Edit(int id)
        {
            Sch_FeeRefundHeaderViewModel bvm = _FeeRefundHeaderService.GetFeeRefundHeaderForEdit(id);
            PrepareViewBag(bvm);
            if (bvm == null)
            {
                return HttpNotFound();
            }
            return View("Create", bvm);
        }


        // GET: /ProductMaster/Delete/5

        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            
            Sch_FeeRefundHeader FeeRefundHeader = _FeeRefundHeaderService.Find(id);
            if (FeeRefundHeader == null)
            {
                return HttpNotFound();
            }

            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<Sch_FeeRefundLine> FeeRefundLineList = new FeeRefundLineService(_unitOfWork).GetFeeRefundLineForHeader(vm.id);

                foreach (Sch_FeeRefundLine item in  FeeRefundLineList)
                {
                    new FeeRefundLineService(_unitOfWork).Delete(item);
                }
                 

                ActivityLog al = new ActivityLog()
                {
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DocId = vm.id,
                    UserRemark = vm.Reason,
                    Narration = vm.Reason,
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDoctypeConstants.FeeRefund).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);


                _FeeRefundHeaderService.Delete(vm.id);
             

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


        public JsonResult GetStudentDetailJson(int StudentId)
        {
            Sch_AdmissionViewModel StudentDetail = new StudentService(_unitOfWork).GetStudentClassDetail(StudentId);

            List<Sch_AdmissionViewModel> StudentDetailJson = new List<Sch_AdmissionViewModel>();

            if (StudentDetail != null)
            {
                StudentDetailJson.Add(new Sch_AdmissionViewModel()
                {
                    ProgramName = StudentDetail.ProgramName,
                    ClassName = StudentDetail.ClassName,
                    StreamName = StudentDetail.StreamName,
                });
            }


            return Json(StudentDetailJson);
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
