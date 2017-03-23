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
using Surya.India.Presentation;
using Surya.India.Presentation.ViewModels;
using Surya.India.Core.Common;
using Surya.India.Model.ViewModel;
using AutoMapper;
using System.Xml.Linq;

namespace Surya.India.Web
{
    [Authorize]
    public class ReportHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        IReportHeaderService _ReportHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public ReportHeaderController(IReportHeaderService ReportHeaderService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _ReportHeaderService = ReportHeaderService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /ProductMaster/

        public ActionResult Index()
        {
            var ReportHeader = _ReportHeaderService.GetReportHeaderList().ToList();
            return View(ReportHeader);
        }

        // GET: /ProductMaster/Create

        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HeaderPost(ReportHeader pt)
        {
            if (ModelState.IsValid)
            {
                if (pt.ReportHeaderId <= 0)
                {

                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    _ReportHeaderService.Create(pt);


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
                    return RedirectToAction("Edit", new { id = pt.ReportHeaderId }).Success("Data saved successfully");

                }
                else
                {

                    pt.ModifiedDate = DateTime.Now;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Modified;
                    _ReportHeaderService.Update(pt);


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

            return View("Create", pt);
        }

        // GET: /ProductMaster/Edit/5

        public ActionResult Edit(int id)
        {
            ReportHeader pt = _ReportHeaderService.GetReportHeader(id);
            if (pt == null)
            {
                return HttpNotFound();
            }
            return View("Create", pt);
        }

        // GET: /ProductMaster/Delete/5

        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportHeader ReportHeader = _ReportHeaderService.Find(id);
            if (ReportHeader == null)
            {
                return HttpNotFound();
            }
            ReasonViewModel rvm = new ReasonViewModel()
            {
                id = id,
            };
            return PartialView("_Reason", rvm);
        }

        // POST: /ProductMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ReasonViewModel vm)
        {

            if (ModelState.IsValid)
            {
                List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                var temp = _ReportHeaderService.Find(vm.id);

                LogList.Add(new LogTypeViewModel
                {
                    ExObj = temp,
                });

                var line = new ReportLineService(_unitOfWork).GetReportLineList(vm.id);


                foreach (var item in line)
                {

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = item,
                    });


                    new ReportLineService(_unitOfWork).Delete(item.ReportLineId);
                }

                _ReportHeaderService.Delete(vm.id);
                XElement Modifications = new ModificationsCheckService(_unitOfWork).CheckChanges(LogList);
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

                //Logging Activity
                LogActivity.LogActivityDetail(temp.ReportHeaderId,
                vm.id,
                null,
                (int)ActivityTypeContants.Deleted,
                vm.Reason + "  -CommonReport",
                User.Identity.Name,
                temp.ReportName, Modifications);

                return Json(new { success = true });

            }
            return PartialView("_Reason", vm);
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
