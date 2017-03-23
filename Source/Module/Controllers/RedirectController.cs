using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Owin;
using Surya.India.Model.Models;
using Surya.India.Data.Models;
using Surya.India.Service;
using Surya.India.Data.Infrastructure;
using System.Configuration;
using Surya.India.Model.ViewModel;

namespace Surya.India.Presentation.Controllers
{
    [Authorize]
    public class RedirectController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        IUnitOfWork _unitOfWork;
        public RedirectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult RedirectToDocument(int DocTypeId,int DocId,int ? DocLineId)
        {

            if (DocTypeId == 0 || DocId == 0)
            {
                return View("Error");
            }

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var DocumentType = new DocumentTypeService(_unitOfWork).Find(DocTypeId);


            if (DocumentType.ControllerActionId.HasValue && DocumentType.ControllerActionId.Value > 0)
            {
                ControllerAction CA = new ControllerActionService(_unitOfWork).Find(DocumentType.ControllerActionId.Value);

                if (CA == null)
                {
                    return View("~/Views/Shared/UnderImplementation.cshtml");
                }
                else if (!string.IsNullOrEmpty(DocumentType.DomainName) && !DocLineId.HasValue)
                {
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings[DocumentType.DomainName] + "/" + CA.ControllerName + "/" + CA.ActionName + "/" + DocId);
                }
                else if (!string.IsNullOrEmpty(DocumentType.DomainName) && DocLineId.HasValue && DocLineId.Value > 0)
                {
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings[DocumentType.DomainName] + "/" + CA.ControllerName + "/" + CA.ActionName + "?Id=" + DocId+"&DocLineId="+DocLineId);
                }
                else
                {
                    return RedirectToAction(CA.ActionName, CA.ControllerName, new { id = DocId });
                }
            }

            return RedirectToAction("GetTrialBalance");
            
        }


    }
}