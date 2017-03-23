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

namespace Surya.India.Web
{

    [Authorize]
    public class StreamController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IStreamService _StreamService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public StreamController(IStreamService StreamService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _StreamService = StreamService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }

        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _StreamService.GetStreamList(id);
            return Json(p, JsonRequestBehavior.AllowGet);

        }


        private void PrepareViewBag()
        {

        }

        public ActionResult _Create(int Id) //Id ==>Sale Order Header Id
        {
            Sch_Stream s = new Sch_Stream();
            s.ProgramId = Id;
            s.WEF = DateTime.Now;
            PrepareViewBag();
            return PartialView("_Create", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(Sch_Stream svm)
        {
            if (ModelState.IsValid)
            {
                if (svm.StreamId == 0)
                {
                    svm.CreatedDate = DateTime.Now;
                    svm.ModifiedDate = DateTime.Now;
                    svm.CreatedBy = User.Identity.Name;
                    svm.ModifiedBy = User.Identity.Name;
                    svm.ObjectState = Model.ObjectState.Added;
                    _StreamService.Create(svm);


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
                    return RedirectToAction("_Create", new { id = svm.ProgramId });
                }
                else
                {
                    Sch_Stream StreamDetail = _StreamService.Find(svm.StreamId);
                    StringBuilder logstring = new StringBuilder();

                    StreamDetail.ProgramId = svm.ProgramId;
                    StreamDetail.StreamName = svm.StreamName;
                    StreamDetail.WEF = svm.WEF;

                    StreamDetail.ModifiedDate = DateTime.Now;
                    StreamDetail.ModifiedBy = User.Identity.Name;
                    StreamDetail.ObjectState = Model.ObjectState.Modified;
                    _StreamService.Update(StreamDetail);


                    //Saving the Activity Log
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = StreamDetail.StreamId,
                        CreatedDate = DateTime.Now,
                        Narration = logstring.ToString(),
                        CreatedBy = User.Identity.Name,
                        //DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.StreamDetail).DocumentTypeId,
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
            Sch_Stream s = _StreamService.Find(id);


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
            Sch_Stream StreamDetail = _StreamService.Find(id);
            if (StreamDetail == null)
            {
                return HttpNotFound();
            }
            return View(StreamDetail);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(Sch_Stream vm)
        {
            Sch_Stream StreamDetail = _StreamService.Find(vm.StreamId);
            _StreamService.Delete(vm.StreamId);

            try
            {
                _unitOfWork.Save();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("EditSize", vm);

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
