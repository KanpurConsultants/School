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

namespace Surya.India.Web
{
    [Authorize]
    public class ErrorController : System.Web.Mvc.Controller
    {
          
        public ActionResult PermissionDenied()
        {
            return View("PermissionDenied");
        }


    }
}
