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
    public class FeeReceiveLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFeeReceiveLineService _FeeReceiveLineService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FeeReceiveLineController(IFeeReceiveLineService FeeReceiveLineService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FeeReceiveLineService = FeeReceiveLineService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _FeeReceiveLineService.GetFeeReceiveLineListForIndex(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }

        public ActionResult _ForMultiple(int Id)
        {
            Sch_FeeReceiveHeader Header = new FeeReceiveHeaderService(_unitOfWork).Find(Id);

            List<Sch_FeeReceiveLineViewModel> temp = _FeeReceiveLineService.GetFeeReceiveLineList((int)Header.StudentId).ToList();
            Sch_FeeReceiveMasterDetailModel svm = new Sch_FeeReceiveMasterDetailModel();
            svm.FeeReceiveHeaderId = Id;
            svm.FeeReceiveLineViewModel = temp;

            return PartialView("_Results", svm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ResultsPost(Sch_FeeReceiveMasterDetailModel vm)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in vm.FeeReceiveLineViewModel)
                {
                    Sch_FeeReceiveLine Line = new Sch_FeeReceiveLine();
                    Line.FeeReceiveHeaderId = vm.FeeReceiveHeaderId;
                    Line.FeeDueLineId = item.FeeDueLineId;
                    Line.Amount = item.Amount;
                    Line.Remark = item.Remark;
                    Line.CreatedDate = DateTime.Now;
                    Line.ModifiedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    _FeeReceiveLineService.Create(Line);
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
            Sch_FeeReceiveLineViewModel s = new Sch_FeeReceiveLineViewModel();
            s.FeeReceiveHeaderId  = Id;



            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_FeeReceiveLineViewModel svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.FeeReceiveLineId == 0)
                {
                    Sch_FeeReceiveLine Line = new Sch_FeeReceiveLine();
                    Line.FeeReceiveHeaderId = svm.FeeReceiveHeaderId;
                    Line.FeeDueLineId = svm.FeeDueLineId;
                    Line.Amount = svm.Amount;
                    Line.Remark = svm.Remark;
                    Line.CreatedDate = DateTime.Now;
                    Line.ModifiedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    _FeeReceiveLineService.Create(Line);


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
                    return RedirectToAction("_Create", new { id = svm.FeeReceiveHeaderId });
                }
                else
                {
                    Sch_FeeReceiveLine FeeReceiveLine = _FeeReceiveLineService.Find(svm.FeeReceiveLineId);
                    StringBuilder logstring = new StringBuilder();

                    FeeReceiveLine.FeeDueLineId = svm.FeeDueLineId;
                    FeeReceiveLine.Amount = svm.Amount;
                    FeeReceiveLine.Remark = svm.Remark;
                    FeeReceiveLine.ModifiedDate = DateTime.Now;
                    FeeReceiveLine.ModifiedBy = User.Identity.Name;
                    FeeReceiveLine.ObjectState = Model.ObjectState.Modified;
                    _FeeReceiveLineService.Update(FeeReceiveLine);


                    //Saving the Activity Log
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = FeeReceiveLine.FeeReceiveLineId,
                        CreatedDate = DateTime.Now,
                        Narration = logstring.ToString(),
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(DocLineTypeConstants.FeeReceiveLine).DocumentTypeId,
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
            Sch_FeeReceiveLineViewModel s = _FeeReceiveLineService.GetFeeReceiveLineForEdit(id);


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
            Sch_FeeReceiveLine FeeReceiveLine = _FeeReceiveLineService.Find(id);
            if (FeeReceiveLine == null)
            {
                return HttpNotFound();
            }
            return View(FeeReceiveLine);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(Sch_FeeReceiveLine vm)
        {
            Sch_FeeReceiveLine FeeReceiveLine = _FeeReceiveLineService.Find(vm.FeeReceiveLineId);
            _FeeReceiveLineService.Delete(vm.FeeReceiveLineId);

            try
            {
                _unitOfWork.Save();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("FeeReceiveLine", vm);

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


