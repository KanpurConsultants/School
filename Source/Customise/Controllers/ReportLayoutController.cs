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
using Surya.India.Model.ViewModels;

namespace Surya.India.Web
{
    [Authorize]
    public class ReportLayoutController : System.Web.Mvc.Controller
    {
          private ApplicationDbContext db = new ApplicationDbContext();

          IUnitOfWork _unitOfWork;
        IReportLineService _ReportLineService;
        public ReportLayoutController(IUnitOfWork work,IReportLineService line)
          {
              _unitOfWork = work;
              _ReportLineService = line;
          }

          [HttpGet]
          public ActionResult ReportLayout(string name)
          {
              ReportHeader header = new ReportHeaderService(_unitOfWork).GetReportHeaderByName(name);
              List<ReportLine> lines = _ReportLineService.GetReportLineList(header.ReportHeaderId).ToList();

              Dictionary<int, string> DefaultValues = TempData["ReportLayoutDefaultValues"] as Dictionary<int, string>;
              TempData["ReportLayoutDefaultValues"] = DefaultValues;
              foreach(var item in lines)
              {
                  if (DefaultValues!=null && DefaultValues.ContainsKey(item.ReportLineId))
                  {
                      item.DefaultValue = DefaultValues[item.ReportLineId];
                  }
              }

              ReportMasterViewModel vm = new ReportMasterViewModel();

              vm.ReportHeader = header;
              vm.ReportLine = lines;
              vm.ReportHeaderId = header.ReportHeaderId;
              
              return View(vm);
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
