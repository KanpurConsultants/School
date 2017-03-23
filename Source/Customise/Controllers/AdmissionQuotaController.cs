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
using System.Reflection;

namespace Surya.India.Web
{
    [Authorize]
    public class AdmissionQuotaController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        IAdmissionQuotaService _AdmissionQuotaService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public AdmissionQuotaController(IAdmissionQuotaService AdmissionQuotaService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _AdmissionQuotaService = AdmissionQuotaService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /AdmissionQuotaMaster/
        
        public ActionResult Index()
        {
            var AdmissionQuota = _AdmissionQuotaService.GetAdmissionQuotaList().ToList();
            return View(AdmissionQuota);
            //return RedirectToAction("Create");
        }

        // GET: /AdmissionQuotaMaster/Create
        
        public ActionResult Create()
        {
            Sch_AdmissionQuota vm = new Sch_AdmissionQuota();
            vm.IsActive = true;
            return View("Create", vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Sch_AdmissionQuota vm)
        {
            Sch_AdmissionQuota pt = vm;
            if (ModelState.IsValid)
            {


                if (vm.AdmissionQuotaId <= 0)
                {
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    _AdmissionQuotaService.Create(pt);

                    ActivityLog log = new ActivityLog()
                    {
                        ActivityType = (int)(ActivityTypeContants.Added),
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        DocId = pt.AdmissionQuotaId,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.AdmissionQuota).DocumentTypeId,
                        Narration = "A new AdmissionQuota is created with the Id " + pt.AdmissionQuotaId,
                    };

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", vm);

                    }


                    return RedirectToAction("Create").Success("Data saved successfully");
                }
                else
                {

                    Sch_AdmissionQuota temp = _AdmissionQuotaService.Find(pt.AdmissionQuotaId);
                    temp.AdmissionQuotaName = pt.AdmissionQuotaName;
                    temp.IsActive = pt.IsActive;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    _AdmissionQuotaService.Update(temp);

                    ActivityLog log = new ActivityLog()
                    {
                        ActivityType = (int)(ActivityTypeContants.Modified),
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        DocId = pt.AdmissionQuotaId,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.AdmissionQuota).DocumentTypeId,
                        Narration = "Delivery Terms is modified with the name" + pt.AdmissionQuotaName,
                    };

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", pt);

                    }


                    return RedirectToAction("Index").Success("Data saved successfully");


                }








            }
            return View("Create", vm);
        }


        // GET: /ProductMaster/Edit/5
        
        public ActionResult Edit(int id)
        {
            Sch_AdmissionQuota pt = _AdmissionQuotaService.Find(id);
            if (pt == null)
            {
                return HttpNotFound();
            }
            return View("Create", pt);
        }

        // POST: /ProductMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(AdmissionQuota pt)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AdmissionQuota temp = _AdmissionQuotaService.Find(pt.AdmissionQuotaId);
        //        temp.AdmissionQuotaName = pt.AdmissionQuotaName;
        //        temp.IsActive = pt.IsActive;
        //        temp.ModifiedDate = DateTime.Now;
        //        temp.ModifiedBy = User.Identity.Name;
        //        temp.ObjectState = Model.ObjectState.Modified;
        //        _AdmissionQuotaService.Update(temp);

        //        try
        //        {
        //            _unitOfWork.Save();
        //        }
        //        catch (DbEntityValidationException dbex)
        //        {
        //            string message = _exception.HandleEntityValidationException(dbex);
        //            ModelState.AddModelError("", message);
        //            return View("Create", pt);
        //        }
        //        catch (DbUpdateException du)
        //        {
        //            string message = _exception.HandleUpdateException(du);
        //            ModelState.AddModelError("", message);
        //            return View("Create", pt);
        //        }
        //        catch (Exception ex)
        //        {
        //            string message = _exception.HandleException(ex);
        //            ModelState.AddModelError("", message);
        //            return View("Create", pt);

        //        }


        //        return RedirectToAction("Index").Success("Data saved successfully");
        //    }
        //    return View("Create", pt);
        //}

        // GET: /ProductMaster/Delete/5
        
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sch_AdmissionQuota AdmissionQuota = db.Sch_AdmissionQuota.Find(id);
            if (AdmissionQuota == null)
            {
                return HttpNotFound();
            }
            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }

        // POST: /ProductMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var temp = _AdmissionQuotaService.Find(vm.id);
                ActivityLog al = new ActivityLog()
                {
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DocId = vm.id,
                    UserRemark = vm.Reason,
                    Narration = "Delivery terms is deleted with Name:" + temp.AdmissionQuotaName,
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.AdmissionQuota).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);


                _AdmissionQuotaService.Delete(vm.id);

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

        [HttpGet]
        public ActionResult NextPage(int id)//CurrentHeaderId
        {
            var nextId = _AdmissionQuotaService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id)//CurrentHeaderId
        {
            var nextId = _AdmissionQuotaService.PrevId(id);
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
            Dt = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.AdmissionQuota);

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

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
