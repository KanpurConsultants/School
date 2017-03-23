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
using Notifier.Hubs;

namespace Surya.India.Web
{
    [Authorize]
    public class NotificationController : System.Web.Mvc.Controller
    { 
    
        public ActionResult GetAllNotifications()
        {

            string UserName = User.Identity.Name;

            var temp = RegisterChanges.GetAllNotifications (UserName);

            return View("~/Views/Shared/Notifications.cshtml",temp);
        }

        public ActionResult NotificationRequest(int id)//NotificationId
        {

            var DefaultUrl = HttpContext.Request.UrlReferrer.ToString();

            string RetUrl = RegisterChanges.SetReadDate(id);

            if (string.IsNullOrEmpty(RetUrl))
                return Redirect(DefaultUrl);       

            return Redirect(RetUrl);

        }


    }
}
