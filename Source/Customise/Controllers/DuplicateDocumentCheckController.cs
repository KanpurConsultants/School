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
using Surya.India.Data.Infrastructure;
using Surya.India.Service;
using Surya.India.Web;
using AutoMapper;
using Surya.India.Presentation.ViewModels;
using Surya.India.Presentation;

namespace Surya.India.Web.Controllers
{
   [Authorize]
    public class DuplicateDocumentCheckController : System.Web.Mvc.Controller
    {

       IDuplicateDocumentCheckService _DuplicateCheckService;

       public DuplicateDocumentCheckController(IDuplicateDocumentCheckService ser)
       {
           _DuplicateCheckService = ser;
       }

       public JsonResult DuplicateCheckForCreate(string table, string docno, int doctypeId)
       {
           var temp = (_DuplicateCheckService.CheckForDocNoExists(table,docno,doctypeId));
           return Json(new { returnvalue = temp });
       }

       public JsonResult DuplicateCheckForEdit(string table, string docno, int doctypeId, int headerid)
       {
           var temp = (_DuplicateCheckService.CheckForDocNoExists(table, docno, doctypeId, headerid));
           return Json(new { returnvalue = temp });
       }
       
    }
}
