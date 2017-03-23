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
    public class ClassFeeController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IClassFeeService _ClassFeeService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public ClassFeeController(IClassFeeService ClassFeeService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _ClassFeeService = ClassFeeService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /Order/
        public ActionResult Index()
        {
            var Boms = _ClassFeeService.GetClassFeeList().ToList();
            return View(Boms);
        }

        [HttpGet]
        public ActionResult NextPage(int id, string name)//BomId
        {
            var nextId = _ClassFeeService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id, string name)//BomId
        {
            var nextId = _ClassFeeService.PrevId(id);
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




        public void PrepareViewBag()
        {
            //ViewBag.PersonRateGroupList = new PersonRateGroupService(_unitOfWork).GetPersonRateGroupList().ToList();
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
            Sch_ClassFee p = new Sch_ClassFee();
            PrepareViewBag();
            return View("Create", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sch_ClassFee svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.ClassFeeId <= 0)
                {
                    svm.IsActive = true;
                    svm.CreatedDate = DateTime.Now;
                    svm.ModifiedDate = DateTime.Now;
                    svm.CreatedBy = User.Identity.Name;
                    svm.ModifiedBy = User.Identity.Name;
                    svm.ObjectState = Model.ObjectState.Added;
                    _ClassFeeService.Create(svm);

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

                    return RedirectToAction("Edit", new { id = svm.ClassFeeId }).Success("Data saved Successfully");
                }
                else
                {
                    Sch_ClassFee ClassFee = _ClassFeeService.Find(svm.ClassFeeId);

                    ClassFee.ProgramId  = svm.ProgramId;
                    ClassFee.ClassId = svm.ClassId;
                    ClassFee.StreamId = svm.StreamId;
                    
                    ClassFee.ModifiedBy = User.Identity.Name;
                    ClassFee.ModifiedDate = DateTime.Now;
                    _ClassFeeService.Update(ClassFee);

                    ////Saving Activity Log::
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = svm.ClassFeeId,
                        Narration = "",
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ClassFee).DocumentTypeId,

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
            PrepareViewBag();
            return View(svm);
        }


        public ActionResult Edit(int id)
        {
            Sch_ClassFee bvm = _ClassFeeService.Find(id);
            PrepareViewBag();
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
            Sch_ClassFee ClassFee = _ClassFeeService.Find(id);
            if (ClassFee == null)
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
                Sch_ClassFee ClassFee = _ClassFeeService.Find(vm.id);


                ActivityLog al = new ActivityLog()
                {
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DocId = vm.id,
                    UserRemark = vm.Reason,
                    Narration = vm.Reason,
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ClassFee).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);


                IEnumerable<Sch_ClassFeeLine> ClassFeeLine = new ClassFeeLineService(_unitOfWork).GetClassFeeLineList(vm.id);
                //Mark ObjectState.Delete to all the Bom Detail For Above Bom. 
                foreach (Sch_ClassFeeLine item in ClassFeeLine)
                {
                    new ClassFeeLineService(_unitOfWork).Delete(item.ClassFeeLineId);
                }


                _ClassFeeService.Delete(vm.id);

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
