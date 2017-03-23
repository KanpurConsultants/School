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
    public class FacilityStopController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFacilityStopService _FacilityStopService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FacilityStopController(IFacilityStopService FacilityStopService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FacilityStopService = FacilityStopService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _FacilityStopService.GetFacilityStopListForIndex(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }


        private void PrepareViewBag()
        {

        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {
            Sch_FacilityStopViewModel s = new Sch_FacilityStopViewModel();
            s.FacilityStopHeaderId  = Id;
            s.StartDate = DateTime.Now;
            

            Sch_FacilityStopHeader H = new FacilityStopHeaderService(_unitOfWork).Find(Id);
            s.AdmissionId = H.AdmissionId;
            s.StopDate = H.DocDate;

            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_FacilityStopViewModel svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.FacilityStopId == 0)
                {
                    Sch_FacilityStop Line = new Sch_FacilityStop();
                    Line.FacilityStopHeaderId = svm.FacilityStopHeaderId;
                    Line.FacilityEnrollmentId = svm.FacilityEnrollmentId;
                    Line.StopReason = svm.StopReason;
                    Line.AvailDays = svm.AvailDays;
                    Line.CreatedDate = DateTime.Now;
                    Line.ModifiedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    _FacilityStopService.Create(Line);


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
                    return RedirectToAction("_Create", new { id = svm.FacilityStopHeaderId });
                }
                else
                {
                    Sch_FacilityStop FacilityStop = _FacilityStopService.Find(svm.FacilityStopId);
                    StringBuilder logstring = new StringBuilder();

                    FacilityStop.FacilityEnrollmentId = svm.FacilityEnrollmentId;
                    FacilityStop.StopReason = svm.StopReason;
                    FacilityStop.AvailDays = svm.AvailDays;
                    FacilityStop.ModifiedDate = DateTime.Now;
                    FacilityStop.ModifiedBy = User.Identity.Name;
                    FacilityStop.ObjectState = Model.ObjectState.Modified;
                    _FacilityStopService.Update(FacilityStop);


                    //Saving the Activity Log
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = FacilityStop.FacilityStopId,
                        CreatedDate = DateTime.Now,
                        Narration = logstring.ToString(),
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.FacilityStop).DocumentTypeId,
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
            Sch_FacilityStopViewModel s = _FacilityStopService.GetFacilityStopForEdit(id);


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
            Sch_FacilityStop FacilityStop = _FacilityStopService.Find(id);
            if (FacilityStop == null)
            {
                return HttpNotFound();
            }
            return View(FacilityStop);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(Sch_FacilityStop vm)
        {
            Sch_FacilityStop FacilityStop = _FacilityStopService.Find(vm.FacilityStopId);
            _FacilityStopService.Delete(vm.FacilityStopId);

            try
            {
                _unitOfWork.Save();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("FacilityStop", vm);

            }
            return Json(new { success = true });
        }

        public ActionResult GetFacilityEnrollment(string searchTerm, int pageSize, int pageNum, int AdmissionId)
        {
            AutoCompleteComboBoxRepositoryAndHelper ar = new AutoCompleteComboBoxRepositoryAndHelper(GetFacilityEnrollmentHelpList(AdmissionId));


            List<ComboBoxList> prodLst = ar.GetListForComboBox(searchTerm, pageSize, pageNum);
            int prodCount = ar.GetCountForComboBox(searchTerm, pageSize, pageNum);

            //Translate the attendees into a format the select2 dropdown expects
            ComboBoxPagedResult pagedAttendees = ar.TranslateToComboBoxFormat(prodLst, prodCount);

            //Return the data as a jsonp result
            return new JsonpResult
            {
                Data = pagedAttendees,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public IEnumerable<ComboBoxList> GetFacilityEnrollmentHelpList(int AdmissionId)
        {
            var prodList = from p in db.Sch_FacilityEnrollment
                           where p.AdmissionId == AdmissionId
                           select new ComboBoxList
                           {
                               Id = p.FacilityEnrollmentId,
                               PropFirst = p.FacilitySubCategory.Facility.FacilityName
                           };

            return prodList.ToList();
        }

        public JsonResult GetFacilityEnrollmentDetailJson(int FacilityEnrollmentId, DateTime StopDate)
        {
            Sch_FacilityEnrollmentViewModel FacilityEnrollmentDetail = new FacilityEnrollmentService(_unitOfWork).GetFacilityEnrollmentDetail(FacilityEnrollmentId);

            List<Sch_FacilityEnrollmentViewModel> FacilityEnrollmentDetailJson = new List<Sch_FacilityEnrollmentViewModel>();

            if (FacilityEnrollmentDetail != null)
            {
                FacilityEnrollmentDetailJson.Add(new Sch_FacilityEnrollmentViewModel()
                {
                    StartDate = FacilityEnrollmentDetail.StartDate,
                    AvailDays = (int) (StopDate - FacilityEnrollmentDetail.StartDate).TotalDays
                });
            }

            return Json(FacilityEnrollmentDetailJson);
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


