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
using Surya.India.Model.ViewModel;

namespace Surya.India.Web
{

    [Authorize]
    public class FeeRefundLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFeeRefundLineService _FeeRefundLineService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FeeRefundLineController(IFeeRefundLineService FeeRefundLineService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FeeRefundLineService = FeeRefundLineService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _FeeRefundLineService.GetFeeRefundLineListForIndex(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }

        public ActionResult _ForMultiple(int Id)
        {
            Sch_FeeRefundHeader Header = new FeeRefundHeaderService(_unitOfWork).Find(Id);

            List<Sch_FeeRefundLineViewModel> temp = _FeeRefundLineService.GetFeeRefundLineList((int)Header.StudentId).ToList();
            Sch_FeeRefundMasterDetailModel svm = new Sch_FeeRefundMasterDetailModel();
            svm.FeeRefundHeaderId = Id;
            svm.FeeRefundLineViewModel = temp;

            return PartialView("_Results", svm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ResultsPost(Sch_FeeRefundMasterDetailModel vm)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in vm.FeeRefundLineViewModel)
                {
                    Sch_FeeRefundLine Line = new Sch_FeeRefundLine();
                    Line.FeeRefundHeaderId = vm.FeeRefundHeaderId;
                    Line.FeeReceiveLineId = item.FeeReceiveLineId;
                    Line.Amount = item.Amount;
                    Line.Remark = item.Remark;
                    Line.CreatedDate = DateTime.Now;
                    Line.ModifiedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    _FeeRefundLineService.Create(Line);
                }

                try
                {
                    _unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ModelState.AddModelError("", message);
                    return PartialView("_Results", vm);
                }
                return Json(new { success = true });

            }
            return PartialView("_Results", vm);

        }


        private void PrepareViewBag()
        {

        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {
            Sch_FeeRefundLineViewModel s = new Sch_FeeRefundLineViewModel();
            s.FeeRefundHeaderId  = Id;



            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_FeeRefundLineViewModel svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.FeeRefundLineId == 0)
                {
                    Sch_FeeRefundLine Line = new Sch_FeeRefundLine();
                    Line.FeeRefundHeaderId = svm.FeeRefundHeaderId;
                    Line.FeeReceiveLineId = svm.FeeReceiveLineId;
                    Line.Amount = svm.Amount;
                    Line.Remark = svm.Remark;
                    Line.CreatedDate = DateTime.Now;
                    Line.ModifiedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    _FeeRefundLineService.Create(Line);


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
                    return RedirectToAction("_Create", new { id = svm.FeeRefundHeaderId });
                }
                else
                {
                    Sch_FeeRefundLine FeeRefundLine = _FeeRefundLineService.Find(svm.FeeRefundLineId);
                    StringBuilder logstring = new StringBuilder();

                    FeeRefundLine.FeeReceiveLineId = svm.FeeReceiveLineId;
                    FeeRefundLine.ModifiedDate = DateTime.Now;
                    FeeRefundLine.ModifiedBy = User.Identity.Name;
                    FeeRefundLine.ObjectState = Model.ObjectState.Modified;
                    _FeeRefundLineService.Update(FeeRefundLine);


                    //Saving the Activity Log
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = FeeRefundLine.FeeRefundLineId,
                        CreatedDate = DateTime.Now,
                        Narration = logstring.ToString(),
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(DocLineTypeConstants.FeeRefundLine).DocumentTypeId,
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
            Sch_FeeRefundLineViewModel s = _FeeRefundLineService.GetFeeRefundLineForEdit(id);


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
            Sch_FeeRefundLine FeeRefundLine = _FeeRefundLineService.Find(id);
            if (FeeRefundLine == null)
            {
                return HttpNotFound();
            }
            return View(FeeRefundLine);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(Sch_FeeRefundLine vm)
        {
            Sch_FeeRefundLine FeeRefundLine = _FeeRefundLineService.Find(vm.FeeRefundLineId);
            _FeeRefundLineService.Delete(vm.FeeRefundLineId);

            try
            {
                _unitOfWork.Save();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("FeeRefundLine", vm);

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


