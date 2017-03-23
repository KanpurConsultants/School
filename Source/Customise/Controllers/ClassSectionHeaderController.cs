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
using Surya.India.Presentation.ViewModels;
using Surya.India.Presentation;
using Surya.India.Core.Common;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using Surya.India.Model.ViewModel;
using Surya.India.Presentation.Helper;
using Surya.India.Model.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Surya.India.Web
{
    [Authorize]
    public class ClassSectionHeaderController : System.Web.Mvc.Controller
    {
          private ApplicationDbContext db = new ApplicationDbContext();

          IClassSectionService _ClassSectionService;
          IUnitOfWork _unitOfWork;
          IExceptionHandlingService _exception;
          public ClassSectionHeaderController(IClassSectionService ClassSectionService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
          {
              _ClassSectionService = ClassSectionService;
              _unitOfWork = unitOfWork;
              _exception = exec;
          }
        // GET: /ClassSectionMaster/
        
          public ActionResult Index()
          {
              var Sch_ClassSection = _ClassSectionService.GetClassSectionHeaderList().ToList();
              return View(Sch_ClassSection);
              //return RedirectToAction("Create");
          }

          // GET: /ClassSectionMaster/Create
        
          public ActionResult Create()
          {
              Sch_ClassSectionHeaderViewModel vm = new Sch_ClassSectionHeaderViewModel();
              return View("Create",vm);
          }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
          public ActionResult Post(Sch_ClassSectionHeaderViewModel vm)
          {
              return Edit(vm.ProgramId, vm.ClassId, vm.StreamId).Success("Data saved Successfully");
          }


        // GET: /ProductMaster/Edit/5

        public ActionResult Edit(int ProgramId, int ClassId, int StreamId)
        {
            Sch_ClassSectionHeaderViewModel pt = new Sch_ClassSectionHeaderViewModel();

            pt.ProgramId = ProgramId;
            pt.ClassId = ClassId;
            pt.StreamId = StreamId;

            if (pt == null)
            {
                return HttpNotFound();
            }
            return View("Create", pt);
        }

        // GET: /ProductMaster/Delete/5

        public ActionResult Delete(int ProgramId, int ClassId, int StreamId)
        {
            DeleteReasonViewModel vm = new DeleteReasonViewModel()
            {
                ProgramId = ProgramId,
                ClassId = ClassId,
                StreamId = StreamId
            };

            return PartialView("_Reason", vm);
        }

        // POST: /ProductMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DeleteReasonViewModel vm)
        {
            if(ModelState.IsValid)
            {
                IEnumerable<Sch_ClassSection> ClassSectionList = new ClassSectionService(_unitOfWork).GetClassSectionList(vm.ProgramId, vm.ClassId, vm.StreamId);

                foreach(Sch_ClassSection item in ClassSectionList )
                {
                    new ClassSectionService(_unitOfWork).Delete(item.ClassSectionId);
                }

                return Json(new { success = true });

            }
            return PartialView("_Reason", vm);
        }

        [HttpGet]
        public ActionResult NextPage(int id)//CurrentHeaderId
        {
            var nextId = _ClassSectionService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id)//CurrentHeaderId
        {
            var nextId = _ClassSectionService.PrevId(id);
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
            //Dt = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Sch_ClassSection);

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

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

    public class DeleteReasonViewModel
    {
        [Required, MinLength(20, ErrorMessage = "Minimum Length of 20 characters should be typed")]
        public string Reason { get; set; }
        public int ProgramId { get; set; }
        public int ClassId { get; set; }
        public int StreamId { get; set; }
        public string sid { get; set; }
        public int DocTypeId { get; set; }

    }
}
