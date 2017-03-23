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
using Surya.India.Web;

namespace Surya.India.Presentation.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        //EventHandlers Obj = new EventHandlers();
        IModuleService _ModuleService;
        ISubModuleService _SubModuleService;
        IUnitOfWork _unitOfWork;
        public MenuController(IModuleService mService, IUnitOfWork unitOfWork, ISubModuleService serv)
        {
            _ModuleService = mService;
            _unitOfWork = unitOfWork;
            _SubModuleService = serv;
        }

        [Authorize]
        public ActionResult Module()
        {            
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            int SiteID = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"]; 
            MenuModuleViewModel Vm = new MenuModuleViewModel();
            List<MenuModule> ma = new List<MenuModule>();

            
            if (UserRoles.Contains("Admin"))
            {
                ma = _ModuleService.GetModuleList().ToList();
            }
            else
            {
                ma = _ModuleService.GetModuleListForUser(UserRoles,SiteID,DivisionId).ToList();
            }
            
            Vm.MenuModule = ma;

            if(ma.Count()==1)
            {
                return RedirectToAction("SubModule", new { id = ma.FirstOrDefault().ModuleId });
            }
            else
            return View("Module", Vm);
        }

        
        public ActionResult UserPermissions(string RoleId)
        {
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"]; 

            MenuModuleViewModel Vm = new MenuModuleViewModel();
            System.Web.HttpContext.Current.Session["RoleUId"] = RoleId;
            List<MenuModule> ma = new List<MenuModule>();
            if (UserRoles.Contains("Admin"))
            {
                ma = _ModuleService.GetModuleList().ToList();
                Vm.RoleId = RoleId; Vm.RoleModification = true;
            }
            
            if (!string.IsNullOrEmpty(RoleId) && (string)TempData["Validation"] == "Valid")
            { }
            Vm.MenuModule = ma;
            return View("Module", Vm);
        }

        [Authorize]
        public ActionResult SubModule(int id,bool ? RolePerm)//ModuleId
        {
            int SiteID = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];           

            List<SubModuleViewModel> vm = new List<SubModuleViewModel>();

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"]; 
           
            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            string appuserid = User.Identity.Name;

            if (UserRoles.Contains("Admin")&& RolePerm.HasValue && RolePerm.Value==true)
            {
                int RolesDivisionId = (int)System.Web.HttpContext.Current.Session["UserPermissionDivisionId"];
                int RolesSIteId = (int)System.Web.HttpContext.Current.Session["UserPermissionSiteId"];

                vm = _SubModuleService.GetSubModuleFromModuleForPermissions(id, RoleId, RolesSIteId, RolesDivisionId).ToList();
            }
            else if(UserRoles.Contains("Admin"))
            {
                vm = _SubModuleService.GetSubModuleFromModule(id, appuserid).ToList();
            }
            else
            {
                vm = _SubModuleService.GetSubModuleFromModuleForUsers(id, appuserid,UserRoles,SiteID,DivisionId).ToList();
            }
            MenuModule tem = _ModuleService.Find(id);
            ViewBag.MName = tem.ModuleName;
            ViewBag.IconName = tem.IconName;
            ViewBag.RolePermissions = RolePerm??false;
            return View("SubModule", vm);
        }

        [Authorize]
        public ActionResult MenuSelection(int id)//Controller ActionId
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);

            MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu(id);


            if (menuviewmodel == null)
            {
                return View("~/Views/Shared/UnderImplementation.cshtml");
            }
            else if (!string.IsNullOrEmpty(menuviewmodel.URL))
            {
                return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/"+menuviewmodel.RouteId+"?MenuId="+menuviewmodel.MenuId);
            }
            else
            {
                return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = menuviewmodel.RouteId });
            }

        }

        [Authorize]
        public ActionResult DropDown(int id)//Menu Id
        {

            //int controlleractionid = new MenuService(_unitOfWork).Find(id).ControllerActionId;

            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(controlleractionid);

            //if (ca == null)
            //{
            //    return View("~/Views/Shared/UnderImplementation.cshtml");
            //}
            //else
            //{
            //    return RedirectToAction(ca.ActionName, ca.ControllerName);
            //}

            MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu(id);


            if (menuviewmodel == null)
            {
                return View("~/Views/Shared/UnderImplementation.cshtml");
            }
            else if (!string.IsNullOrEmpty(menuviewmodel.URL))
            {
                return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + menuviewmodel.RouteId + "?MenuId=" + menuviewmodel.MenuId);
            }
            else
            {
                return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = menuviewmodel.RouteId });
            }

        }


    }
}