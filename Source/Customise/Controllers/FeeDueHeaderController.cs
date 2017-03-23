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
    public class FeeDueHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IFeeDueHeaderService _FeeDueHeaderService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public FeeDueHeaderController(IFeeDueHeaderService FeeDueHeaderService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _FeeDueHeaderService = FeeDueHeaderService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /Order/
        public ActionResult Index()
        {
            var Boms = _FeeDueHeaderService.GetFeeDueHeaderListForIndex().ToList();
            return View(Boms);
        }

      

        [HttpGet]
        public ActionResult NextPage(int id, string name)//BomId
        {
            var nextId = _FeeDueHeaderService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id, string name)//BomId
        {
            var nextId = _FeeDueHeaderService.PrevId(id);
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




        public void PrepareViewBag(Sch_FeeDueHeaderViewModel svm)
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
            Sch_FeeDueHeaderViewModel p = new Sch_FeeDueHeaderViewModel();
            p.DocDate = DateTime.Now;
            p.LastDate = DateTime.Now;
            p.FromDate = DateTime.Now;
            p.ToDate = DateTime.Now;
            p.DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDoctypeConstants.FeeDue).DocumentTypeId;
            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".Sch_FeeDueHeader", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            PrepareViewBag(p);
            return View("Create", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sch_FeeDueHeaderViewModel svm)
        {
            if (ModelState.IsValid)
            {
                
                if (svm.FeeDueHeaderId <= 0)
                {
                    Sch_FeeDueHeader FeeDueHeader = new Sch_FeeDueHeader();
                    FeeDueHeader.DocTypeId = svm.DocTypeId;
                    FeeDueHeader.DocNo = svm.DocNo;
                    FeeDueHeader.DocDate = svm.DocDate;
                    FeeDueHeader.LastDate = svm.LastDate;
                    FeeDueHeader.FromDate = svm.FromDate;
                    FeeDueHeader.ToDate = svm.ToDate;
                    FeeDueHeader.ProgramId = svm.ProgramId;
                    FeeDueHeader.StreamId = svm.StreamId;
                    FeeDueHeader.ClassId = svm.ClassId;
                    FeeDueHeader.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    FeeDueHeader.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                    FeeDueHeader.CreatedDate = DateTime.Now;
                    FeeDueHeader.ModifiedDate = DateTime.Now;
                    FeeDueHeader.CreatedBy = User.Identity.Name;
                    FeeDueHeader.ModifiedBy = User.Identity.Name;
                    FeeDueHeader.ObjectState = Model.ObjectState.Added;
                    _FeeDueHeaderService.Create(FeeDueHeader);


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

                    return RedirectToAction("Edit", new { id = FeeDueHeader.FeeDueHeaderId }).Success("Data saved Successfully");
                }
                else
                {
                    Sch_FeeDueHeader FeeDueHeader = new FeeDueHeaderService(_unitOfWork).Find(svm.FeeDueHeaderId);
                    FeeDueHeader.DocNo = svm.DocNo;
                    FeeDueHeader.DocDate = svm.DocDate;
                    FeeDueHeader.LastDate = svm.LastDate;
                    FeeDueHeader.FromDate = svm.FromDate;
                    FeeDueHeader.ToDate = svm.ToDate;
                    FeeDueHeader.ProgramId = svm.ProgramId;
                    FeeDueHeader.StreamId = svm.StreamId;
                    FeeDueHeader.ClassId = svm.ClassId;
                    FeeDueHeader.ModifiedDate = DateTime.Now;
                    FeeDueHeader.ModifiedBy = User.Identity.Name;
                    _FeeDueHeaderService.Update(FeeDueHeader);


                    ////Saving Activity Log::
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = svm.FeeDueHeaderId,
                        Narration = "",
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDoctypeConstants.FeeDue).DocumentTypeId,

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
            Sch_FeeDueHeaderViewModel bvm = _FeeDueHeaderService.GetFeeDueHeaderForEdit(id);
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
            
            
            Sch_FeeDueHeader FeeDueHeader = _FeeDueHeaderService.Find(id);
            if (FeeDueHeader == null)
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
                IEnumerable<Sch_FeeDueLine> FeeDueLineList = new FeeDueLineService(_unitOfWork).GetFeeDueLineForHeader(vm.id);

                foreach (Sch_FeeDueLine item in  FeeDueLineList)
                {
                    new FeeDueLineService(_unitOfWork).Delete(item);
                }
                 

                ActivityLog al = new ActivityLog()
                {
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DocId = vm.id,
                    UserRemark = vm.Reason,
                    Narration = vm.Reason,
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDoctypeConstants.FeeDue).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);


                _FeeDueHeaderService.Delete(vm.id);
             

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


        public ActionResult GetClass(string searchTerm, int pageSize, int pageNum, int ProgramId)
        {
            AutoCompleteComboBoxRepositoryAndHelper ar = new AutoCompleteComboBoxRepositoryAndHelper(GetClassHelpList(ProgramId));


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

        public IEnumerable<ComboBoxList> GetClassHelpList(int ProgramId)
        {
            var prodList = from p in db.Sch_Class
                           where p.ProgramId == ProgramId
                           orderby p.ClassName
                           select new ComboBoxList
                           {
                               Id = p.ClassId,
                               PropFirst = p.ClassName,
                           };

            return prodList.ToList();
        }


        public ActionResult GetStream(string searchTerm, int pageSize, int pageNum, int ProgramId)
        {
            AutoCompleteComboBoxRepositoryAndHelper ar = new AutoCompleteComboBoxRepositoryAndHelper(GetStreamHelpList(ProgramId));


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

        public IEnumerable<ComboBoxList> GetStreamHelpList(int ProgramId)
        {
            var prodList = from p in db.Sch_Stream
                           where p.ProgramId == ProgramId
                           orderby p.StreamName
                           select new ComboBoxList
                           {
                               Id = p.StreamId,
                               PropFirst = p.StreamName,
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
