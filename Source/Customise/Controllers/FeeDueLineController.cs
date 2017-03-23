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
    public class FeeDueLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFeeDueLineService _FeeDueLineService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FeeDueLineController(IFeeDueLineService FeeDueLineService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FeeDueLineService = FeeDueLineService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _FeeDueLineService.GetFeeDueLineListForIndex(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }

        public ActionResult _ForMultiple(int Id)
        {
            Sch_FeeDueHeader Header = new FeeDueHeaderService(_unitOfWork).Find(Id);

            List<Sch_FeeDueLineViewModel> temp = _FeeDueLineService.GetFeeDueLineList((int)Header.ProgramId, (int)Header.ClassId, (int)Header.StreamId, Header.FromDate, Header.ToDate).ToList();
            Sch_FeeDueMasterDetailModel svm = new Sch_FeeDueMasterDetailModel();
            svm.FeeDueHeaderId = Id;
            svm.FeeDueLineViewModel = temp;

            return PartialView("_Results", svm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ResultsPost(Sch_FeeDueMasterDetailModel vm)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in vm.FeeDueLineViewModel)
                {
                    Sch_FeeDueLine Line = new Sch_FeeDueLine();
                    Line.FeeDueHeaderId = vm.FeeDueHeaderId;
                    Line.AdmissionId = item.AdmissionId;
                    Line.FeeId = item.FeeId;
                    Line.Recurrence = item.Recurrence;
                    Line.Amount = item.Amount;
                    Line.Remark = item.Remark;
                    Line.CreatedDate = DateTime.Now;
                    Line.ModifiedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    _FeeDueLineService.Create(Line);
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
            Sch_FeeDueLineViewModel s = new Sch_FeeDueLineViewModel();
            s.FeeDueHeaderId  = Id;



            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_FeeDueLineViewModel svm)
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
                    _FeeDueLineService.Create(Line);


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
                    Sch_FeeDueLine FeeDueLine = _FeeDueLineService.Find(svm.FeeDueLineId);
                    StringBuilder logstring = new StringBuilder();

                    FeeDueLine.AdmissionId = svm.AdmissionId;
                    FeeDueLine.FeeId = svm.FeeId;
                    FeeDueLine.ModifiedDate = DateTime.Now;
                    FeeDueLine.ModifiedBy = User.Identity.Name;
                    FeeDueLine.ObjectState = Model.ObjectState.Modified;
                    _FeeDueLineService.Update(FeeDueLine);


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
            Sch_FeeDueLineViewModel s = _FeeDueLineService.GetFeeDueLineForEdit(id);


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
            Sch_FeeDueLine FeeDueLine = _FeeDueLineService.Find(id);
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
            Sch_FeeDueLine FeeDueLine = _FeeDueLineService.Find(vm.FeeDueLineId);
            _FeeDueLineService.Delete(vm.FeeDueLineId);

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


