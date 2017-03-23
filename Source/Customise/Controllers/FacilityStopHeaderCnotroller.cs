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
    public class FacilityStopHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFacilityStopHeaderService _FacilityStopHeaderService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FacilityStopHeaderController(IFacilityStopHeaderService FacilityStopHeaderService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FacilityStopHeaderService = FacilityStopHeaderService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /Order/
        public ActionResult Index()
        {
            var Boms = _FacilityStopHeaderService.GetFacilityStopHeaderListForIndex().ToList();
            return View(Boms);
        }

     
        [HttpGet]
        public ActionResult NextPage(int id, string name)//BomId
        {
            var nextId = _FacilityStopHeaderService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id, string name)//BomId
        {
            var nextId = _FacilityStopHeaderService.PrevId(id);
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




        public void PrepareViewBag(Sch_FacilityStopHeaderViewModel svm)
        {

        }



        public ActionResult ChooseType()
        {
            return PartialView("ChooseType");
        }
   


        public ActionResult Create()
        {
            Sch_FacilityStopHeaderViewModel p = new Sch_FacilityStopHeaderViewModel();
            p.DocDate = DateTime.Now;
            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".Sch_FacilityStopHeader", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            PrepareViewBag(p);
            return View("Create", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sch_FacilityStopHeaderViewModel svm)
        {


            if (ModelState.IsValid)
            {
                if (svm.FacilityStopHeaderId <= 0)
                {
                    Sch_FacilityStopHeader FacilityStopHeader = new Sch_FacilityStopHeader();
                    FacilityStopHeader.DocTypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.FacilityStopHeader).DocumentTypeId;
                    FacilityStopHeader.DocNo = svm.DocNo;
                    FacilityStopHeader.DocDate = svm.DocDate;
                    FacilityStopHeader.AdmissionId = svm.AdmissionId;
                    FacilityStopHeader.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    FacilityStopHeader.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                    FacilityStopHeader.CreatedDate = DateTime.Now;
                    FacilityStopHeader.ModifiedDate = DateTime.Now;
                    FacilityStopHeader.CreatedBy = User.Identity.Name;
                    FacilityStopHeader.ModifiedBy = User.Identity.Name;
                    FacilityStopHeader.ObjectState = Model.ObjectState.Added;
                    _FacilityStopHeaderService.Create(FacilityStopHeader);

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

                    return RedirectToAction("Edit", new { id = FacilityStopHeader.FacilityStopHeaderId }).Success("Data saved Successfully");
                }
                else
                {
                    Sch_FacilityStopHeader FacilityStopHeader = new FacilityStopHeaderService(_unitOfWork).Find(svm.FacilityStopHeaderId);
                    FacilityStopHeader.DocNo = svm.DocNo;
                    FacilityStopHeader.DocDate = svm.DocDate;
                    FacilityStopHeader.AdmissionId = svm.AdmissionId;
                    FacilityStopHeader.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    FacilityStopHeader.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                    FacilityStopHeader.ModifiedDate = DateTime.Now;
                    FacilityStopHeader.ModifiedBy = User.Identity.Name;
                    _FacilityStopHeaderService.Update(FacilityStopHeader);




                    ////Saving Activity Log::
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = svm.FacilityStopHeaderId,
                        Narration = "",
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.FacilityStopHeader).DocumentTypeId,

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
            Sch_FacilityStopHeaderViewModel bvm = _FacilityStopHeaderService.GetFacilityStopHeaderForEdit(id);
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
            
            
            Sch_FacilityStopHeader FacilityStopHeader = _FacilityStopHeaderService.Find(id);
            if (FacilityStopHeader == null)
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
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.FacilityStopHeader).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);


                //IEnumerable<Sch_FacilityStopHeaderLine> FacilityStopHeaderLine = new FacilityStopHeaderLineService(_unitOfWork).GetFacilityStopHeaderLineList(vm.id);
                ////Mark ObjectState.Delete to all the Bom Detail For Above Bom. 
                //foreach (Sch_FacilityStopHeaderLine item in FacilityStopHeaderLine)
                //{
                //    new FacilityStopHeaderLineService(_unitOfWork).Delete(item.FacilityStopHeaderLineId);
                //}

                _FacilityStopHeaderService.Delete(vm.id);

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
