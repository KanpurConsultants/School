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
    public class FacilityEnrollmentController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFacilityEnrollmentService _FacilityEnrollmentService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FacilityEnrollmentController(IFacilityEnrollmentService FacilityEnrollmentService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FacilityEnrollmentService = FacilityEnrollmentService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /Order/
        public ActionResult Index()
        {
            var Boms = _FacilityEnrollmentService.GetFacilityEnrollmentListForIndex().ToList();
            return View(Boms);
        }

     
        [HttpGet]
        public ActionResult NextPage(int id, string name)//BomId
        {
            var nextId = _FacilityEnrollmentService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id, string name)//BomId
        {
            var nextId = _FacilityEnrollmentService.PrevId(id);
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




        public void PrepareViewBag(Sch_FacilityEnrollmentViewModel svm)
        {

        }



        public ActionResult ChooseType()
        {
            return PartialView("ChooseType");
        }
   


        public ActionResult Create()
        {
            Sch_FacilityEnrollmentViewModel p = new Sch_FacilityEnrollmentViewModel();
            p.DocDate = DateTime.Now;
            p.StartDate = DateTime.Now;
            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".Sch_FacilityEnrollment", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            PrepareViewBag(p);
            return View("Create", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sch_FacilityEnrollmentViewModel svm)
        {


            if (ModelState.IsValid)
            {
                if (svm.FacilityEnrollmentId <= 0)
                {
                    Sch_FacilityEnrollment FacilityEnrollment = new Sch_FacilityEnrollment();
                    FacilityEnrollment.DocTypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.FacilityEnrollment).DocumentTypeId;
                    FacilityEnrollment.DocNo = svm.DocNo;
                    FacilityEnrollment.DocDate = svm.DocDate;
                    FacilityEnrollment.AdmissionId = svm.AdmissionId;
                    FacilityEnrollment.FacilitySubCategoryId = svm.FacilitySubCategoryId;
                    FacilityEnrollment.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    FacilityEnrollment.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                    FacilityEnrollment.StartDate = svm.StartDate ;
                    FacilityEnrollment.IsActive = true;
                    FacilityEnrollment.CreatedDate = DateTime.Now;
                    FacilityEnrollment.ModifiedDate = DateTime.Now;
                    FacilityEnrollment.CreatedBy = User.Identity.Name;
                    FacilityEnrollment.ModifiedBy = User.Identity.Name;
                    FacilityEnrollment.ObjectState = Model.ObjectState.Added;
                    _FacilityEnrollmentService.Create(FacilityEnrollment);

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

                    return RedirectToAction("Edit", new { id = FacilityEnrollment.FacilityEnrollmentId }).Success("Data saved Successfully");
                }
                else
                {
                    Sch_FacilityEnrollment FacilityEnrollment = new FacilityEnrollmentService(_unitOfWork).Find(svm.FacilityEnrollmentId);
                    FacilityEnrollment.DocNo = svm.DocNo;
                    FacilityEnrollment.DocDate = svm.DocDate;
                    FacilityEnrollment.AdmissionId = svm.AdmissionId;
                    FacilityEnrollment.FacilitySubCategoryId = svm.FacilitySubCategoryId;
                    FacilityEnrollment.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    FacilityEnrollment.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                    FacilityEnrollment.IsActive = true;
                    FacilityEnrollment.StartDate = svm.StartDate;
                    FacilityEnrollment.ModifiedDate = DateTime.Now;
                    FacilityEnrollment.ModifiedBy = User.Identity.Name;
                    _FacilityEnrollmentService.Update(FacilityEnrollment);




                    ////Saving Activity Log::
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = svm.FacilityEnrollmentId,
                        Narration = "",
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.FacilityEnrollment).DocumentTypeId,

                    };
                    new ActivityLogService(_unitOfWork).Create(al);
                    //End of Saving ActivityLognh

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
            Sch_FacilityEnrollmentViewModel bvm = _FacilityEnrollmentService.GetFacilityEnrollmentForEdit(id);
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
            
            
            Sch_FacilityEnrollment FacilityEnrollment = _FacilityEnrollmentService.Find(id);
            if (FacilityEnrollment == null)
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
                ActivityLog al = new ActivityLog()
                {
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DocId = vm.id,
                    UserRemark = vm.Reason,
                    Narration = vm.Reason,
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.FacilityEnrollment).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);


                //IEnumerable<Sch_FacilityEnrollmentLine> FacilityEnrollmentLine = new FacilityEnrollmentLineService(_unitOfWork).GetFacilityEnrollmentLineList(vm.id);
                ////Mark ObjectState.Delete to all the Bom Detail For Above Bom. 
                //foreach (Sch_FacilityEnrollmentLine item in FacilityEnrollmentLine)
                //{
                //    new FacilityEnrollmentLineService(_unitOfWork).Delete(item.FacilityEnrollmentLineId);
                //}

                _FacilityEnrollmentService.Delete(vm.id);

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

        public ActionResult GetFacilitySubCategory(string searchTerm, int pageSize, int pageNum, int FacilityId)
        {
            AutoCompleteComboBoxRepositoryAndHelper ar = new AutoCompleteComboBoxRepositoryAndHelper(GetFacilitySubCategoryHelpList(FacilityId));


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

        public IEnumerable<ComboBoxList> GetFacilitySubCategoryHelpList(int FacilityId)
        {
            var prodList = from p in db.Sch_FacilitySubCategory
                           where p.FacilityId == FacilityId
                           orderby p.FacilitySubCategoryName
                           select new ComboBoxList
                           {
                               Id = p.FacilitySubCategoryId,
                               PropFirst = p.FacilitySubCategoryName,
                           };

            return prodList.ToList();
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
