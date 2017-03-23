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
    public class UnScheduledFeeDueLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IUnScheduledFeeDueLineService _UnScheduledFeeDueLineService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public UnScheduledFeeDueLineController(IUnScheduledFeeDueLineService UnScheduledFeeDueLineService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _UnScheduledFeeDueLineService = UnScheduledFeeDueLineService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        public ActionResult _ForOrder(int id)
        {
            Sch_UnScheduledFeeDueLineFilterViewModel vm = new Sch_UnScheduledFeeDueLineFilterViewModel();
            vm.FeeDueHeaderId = id;
            Sch_FeeDueHeader header = new UnScheduledFeeDueHeaderService(_unitOfWork).Find(vm.FeeDueHeaderId);
            vm.DocTypeId = header.DocTypeId;
            return PartialView("_Filters", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _FilterPost(Sch_UnScheduledFeeDueLineFilterViewModel vm)
        {
            List<Sch_UnScheduledFeeDueLineViewModel> temp = _UnScheduledFeeDueLineService.GetUnScheduledFeeDueListForFilters(vm);
            Sch_UnScheduledFeeDueMasterDetailModel svm = new Sch_UnScheduledFeeDueMasterDetailModel();
            svm.UnScheduledFeeDueLineViewModel = temp;
            

            return PartialView("_Results", svm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ResultsPost(Sch_UnScheduledFeeDueMasterDetailModel vm)
        {
            if (ModelState.IsValid)
            {
                Sch_FeeDueHeader Header = new UnScheduledFeeDueHeaderService(_unitOfWork).Find(vm.UnScheduledFeeDueLineViewModel.FirstOrDefault().FeeDueHeaderId);

                foreach (var item in vm.UnScheduledFeeDueLineViewModel)
                {
                    Sch_FeeDueLine Line = new Sch_FeeDueLine();
                    Line.FeeDueHeaderId = item.FeeDueHeaderId;
                    Line.AdmissionId = item.AdmissionId;
                    Line.FeeId = item.FeeId;
                    Line.Amount = item.Amount;
                    Line.Remark = item.Remark;
                    Line.CreatedDate = DateTime.Now;
                    Line.ModifiedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    _UnScheduledFeeDueLineService.Create(Line);
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

        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _UnScheduledFeeDueLineService.GetFeeDueLineListForIndex(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }

  
        private void PrepareViewBag()
        {

        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {
            Sch_UnScheduledFeeDueLineViewModel s = new Sch_UnScheduledFeeDueLineViewModel();
            s.FeeDueHeaderId  = Id;

            Sch_FeeDueHeader Header = new FeeDueHeaderService(_unitOfWork).Find(Id);
            s.FeeId = Header.FeeId;




            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_UnScheduledFeeDueLineViewModel svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.FeeDueLineId == 0)
                {
                    Sch_FeeDueLine Line = new Sch_FeeDueLine();
                    Line.FeeDueHeaderId = svm.FeeDueHeaderId;
                    Line.AdmissionId = svm.AdmissionId;
                    Line.FeeId = svm.FeeId;
                    Line.Recurrence = svm.Recurrence;
                    Line.Amount = svm.Amount;
                    Line.Remark = svm.Remark;
                    Line.CreatedDate = DateTime.Now;
                    Line.ModifiedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    _UnScheduledFeeDueLineService.Create(Line);


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
                    return RedirectToAction("_Create", new { id = svm.FeeDueHeaderId });
                }
                else
                {
                    Sch_FeeDueLine FeeDueLine = _UnScheduledFeeDueLineService.Find(svm.FeeDueLineId);
                    StringBuilder logstring = new StringBuilder();

                    FeeDueLine.AdmissionId = svm.AdmissionId;
                    FeeDueLine.FeeId = svm.FeeId;
                    FeeDueLine.Amount = svm.Amount;
                    FeeDueLine.Remark = svm.Remark;
                    FeeDueLine.ModifiedDate = DateTime.Now;
                    FeeDueLine.ModifiedBy = User.Identity.Name;
                    FeeDueLine.ObjectState = Model.ObjectState.Modified;
                    _UnScheduledFeeDueLineService.Update(FeeDueLine);


                    //Saving the Activity Log
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = FeeDueLine.FeeDueLineId,
                        CreatedDate = DateTime.Now,
                        Narration = logstring.ToString(),
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(DocLineTypeConstants.FeeDueLine).DocumentTypeId,
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
            Sch_UnScheduledFeeDueLineViewModel s = _UnScheduledFeeDueLineService.GetFeeDueLineForEdit(id);


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
            Sch_FeeDueLine FeeDueLine = _UnScheduledFeeDueLineService.Find(id);
            if (FeeDueLine == null)
            {
                return HttpNotFound();
            }
            return View(FeeDueLine);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(Sch_FeeDueLine vm)
        {
            Sch_FeeDueLine FeeDueLine = _UnScheduledFeeDueLineService.Find(vm.FeeDueLineId);
            _UnScheduledFeeDueLineService.Delete(vm.FeeDueLineId);

            try
            {
                _unitOfWork.Save();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("FeeDueLine", vm);

            }
            return Json(new { success = true });
        }

        public JsonResult GetAdmissionDetailJson(int AdmissionId)
        {
            Sch_AdmissionViewModel AdmissionDetail = new AdmissionService(_unitOfWork).GetAdmissionDetail(AdmissionId);

            List<Sch_AdmissionViewModel> AdmissionDetailJson = new List<Sch_AdmissionViewModel>();

            if (AdmissionDetail != null)
            {
                AdmissionDetailJson.Add(new Sch_AdmissionViewModel()
                {
                    ProgramName = AdmissionDetail.ProgramName,
                    ClassName = AdmissionDetail.ClassName,
                    StreamName = AdmissionDetail.StreamName
                });
            }

            return Json(AdmissionDetailJson);
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


