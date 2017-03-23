﻿using System;
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
using Surya.India.Model.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Surya.India.Web
{
    [Authorize]
    public class AdminSetupController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string ErrorMessage { get; set; }


        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public AdminSetupController(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _unitOfWork = unitOfWork;
            _exception = exec;
        }


        public ActionResult UpdateCA()
        {
            UpdateControllers();
            UpdateActions();
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ViewBag.ErrorMessage = ErrorMessage;
                return View("Error");
            }
            else
            {
                return RedirectToAction("Module", "Menu");
            }

        }

        public class ControllerActionList
        {
            public int? ControllerId { get; set; }
            public int ActionId { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public bool IsActive { get; set; }
        }

        public void UpdateControllers()
        {
            string s = "";

            List<ControllerActionList> List = new List<ControllerActionList>();
            List<ControllerActionList> Controllers = new List<ControllerActionList>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var controllers = Assembly.GetExecutingAssembly().GetExportedTypes().Where(t => typeof(ControllerBase).IsAssignableFrom(t)).Select(t => t);

            #region ☺ControllerUpdateCode☺

            foreach (Type controller in controllers)
            {
                Controllers.Add(new ControllerActionList { ControllerName = controller.Name.Replace("Controller", "") });
            }

            List<ControllerActionList> DBControllers = (from p in db.MvcController
                                                        select new ControllerActionList { ControllerName = p.ControllerName, IsActive = p.IsActive }
                          ).ToList();

            var PendingToUpdate = from p in Controllers
                                  join t in DBControllers on p.ControllerName equals t.ControllerName into table
                                  from left in table.DefaultIfEmpty()
                                  where left == null
                                  select p.ControllerName;

            var PendingToDeActivate = from p in DBControllers
                                      join t in Controllers on p.ControllerName equals t.ControllerName into table
                                      from right in table.DefaultIfEmpty()
                                      where right == null
                                      select p.ControllerName;

            var PendingToActivate = from p in Controllers
                                    join t in DBControllers.Where(m => m.IsActive == false) on p.ControllerName equals t.ControllerName
                                    select p.ControllerName;


            foreach (var item in PendingToUpdate)
            {
                MvcController temp = new MvcController();

                temp.ControllerName = item;
                temp.IsActive = true;
                temp.ObjectState = Model.ObjectState.Added;
                temp.CreatedBy = User.Identity.Name;
                temp.CreatedDate = DateTime.Now;
                temp.ModifiedBy = User.Identity.Name;
                temp.ModifiedDate = DateTime.Now;
                new MvcControllerService(_unitOfWork).Create(temp);
            }

            foreach (var item in PendingToDeActivate)
            {
                MvcController DeactivateRecord = new MvcControllerService(_unitOfWork).Find(item);
                DeactivateRecord.IsActive = false;
                DeactivateRecord.ModifiedBy = User.Identity.Name;
                DeactivateRecord.ModifiedDate = DateTime.Now;
                new MvcControllerService(_unitOfWork).Update(DeactivateRecord);
            }

            foreach (var item in PendingToActivate)
            {
                MvcController ActivateRecord = new MvcControllerService(_unitOfWork).Find(item);
                ActivateRecord.IsActive = true;
                ActivateRecord.ModifiedBy = User.Identity.Name;
                ActivateRecord.ModifiedDate = DateTime.Now;
                new MvcControllerService(_unitOfWork).Update(ActivateRecord);
            }

            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ErrorMessage = message;
            }


            #endregion

        }

        public void UpdateActions()
        {
            List<ControllerActionList> Actions = new List<ControllerActionList>();

            #region ☻ActionUpdateCode☻

            var controllers = Assembly.GetExecutingAssembly().GetExportedTypes().Where(t => typeof(ControllerBase).IsAssignableFrom(t)).Select(t => t);

            foreach (Type controller in controllers)
            {

                var actions = controller.GetMethods().Where(t => t.Name != "Dispose" && !t.IsSpecialName && t.DeclaringType.IsSubclassOf(typeof(ControllerBase)) && t.IsPublic && !t.IsStatic ).ToList();

                foreach (var action in actions)
                {
                    var myAttributes = action.GetCustomAttributes(false);
                    for (int j = 0; j < myAttributes.Length; j++)
                        if (myAttributes.All(m => (m is HttpGetAttribute)))
                        { 
                    Actions.Add(new ControllerActionList
                    {
                        ActionName = action.Name,
                        ControllerName = controller.Name.Replace("Controller", "")
                    });
                    break;
                     }

                }
            }

            List<ControllerActionList> DbControllers = new List<ControllerActionList>();

            DbControllers = (from p in db.MvcController
                             where p.IsActive == true
                             select new ControllerActionList
                             {
                                 ControllerName = p.ControllerName,
                                 ControllerId = p.ControllerId,
                             }).ToList();


            var MapControllerActions = (from p in Actions
                                        join t in DbControllers on p.ControllerName equals t.ControllerName
                                        select new ControllerActionList
                                        {
                                            ActionName = p.ActionName,
                                            ControllerId = t.ControllerId,
                                        }
                                 );



            List<ControllerActionList> DBActions = new List<ControllerActionList>();

            DBActions = (from p in db.ControllerAction
                         where p.IsActive == true
                         select new ControllerActionList
                         {
                             ActionId = p.ControllerActionId,
                             ActionName = p.ActionName,
                             ControllerId = p.ControllerId,
                             IsActive = p.IsActive
                         }).ToList();



            var PendingToUpdate = from p in MapControllerActions
                                  join t in DBActions on new { ActName = p.ActionName, ContId = p.ControllerId } equals new { ActName = t.ActionName, ContId = t.ControllerId } into table
                                  from left in table.DefaultIfEmpty()
                                  where left == null
                                  select new ControllerActionList
                                  {
                                      ActionName = p.ActionName,
                                      ControllerId = p.ControllerId
                                  };

            var PendingToDeActivate = from p in DBActions
                                      join t in MapControllerActions on new { ActName = p.ActionName, ContId = p.ControllerId } equals new { ActName = t.ActionName, ContId = t.ControllerId } into table
                                      from right in table.DefaultIfEmpty()
                                      where right == null
                                      select new ControllerActionList
                                      {
                                          ActionName = p.ActionName,
                                          ControllerId = p.ControllerId,
                                      };

            var PendingToActivate = from p in MapControllerActions
                                    join t in DBActions.Where(m => m.IsActive == false) on new { ActName = p.ActionName, ContId = p.ControllerId } equals new { ActName = t.ActionName, ContId = t.ControllerId }
                                    select new ControllerActionList
                                    {
                                        ActionName = p.ActionName,
                                        ControllerId = p.ControllerId
                                    };


            foreach (var item in PendingToUpdate)
            {
                ControllerAction temp = new ControllerAction();

                temp.ActionName = item.ActionName;
                temp.IsActive = true;
                temp.ControllerId = item.ControllerId;
                temp.ObjectState = Model.ObjectState.Added;
                temp.CreatedBy = User.Identity.Name;
                temp.CreatedDate = DateTime.Now;
                temp.ModifiedBy = User.Identity.Name;
                temp.ModifiedDate = DateTime.Now;
                new ControllerActionService(_unitOfWork).Create(temp);
            }

            foreach (var item in PendingToDeActivate)
            {
                ControllerAction DeactivateRecord = new ControllerActionService(_unitOfWork).Find(item.ActionName, item.ControllerId);
                DeactivateRecord.IsActive = false;
                DeactivateRecord.ModifiedBy = User.Identity.Name;
                DeactivateRecord.ModifiedDate = DateTime.Now;
                new ControllerActionService(_unitOfWork).Update(DeactivateRecord);
            }

            foreach (var item in PendingToActivate)
            {
                ControllerAction ActivateRecord = new ControllerActionService(_unitOfWork).Find(item.ActionName, item.ControllerId);
                ActivateRecord.IsActive = true;
                ActivateRecord.ModifiedBy = User.Identity.Name;
                ActivateRecord.ModifiedDate = DateTime.Now;
                new ControllerActionService(_unitOfWork).Update(ActivateRecord);
            }

            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ErrorMessage = message;
            }




            #endregion

        }


        public ActionResult AssignPermissions()
        {
            List<RolesViewModel> Roles = new List<RolesViewModel>();
            Roles = (from p in db.Roles
                     orderby p.Name
                     select new RolesViewModel
                     {
                         RoleId = p.Id,
                         RoleName = p.Name,
                     }).ToList();
            return View("RolesList", Roles);
        }
        [HttpGet]

        public ActionResult Roles(string id)
        {
            RoleSitePermissionViewModel Vm = new RoleSitePermissionViewModel();

            Vm.DivisionId = string.Join(",", (from p in db.RolesDivision
                                              where p.RoleId == id
                                              select p.DivisionId.ToString()));


            Vm.SiteId = string.Join(",", (from p in db.RolesSite
                                          where p.RoleId == id
                                          select p.SiteId.ToString()));

            Vm.RoleId = id;

            return View("RolesSiteAndDivision", Vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Roles(RoleSitePermissionViewModel vm)
        {

            List<int> NewDivisionIds = new List<int>();
            List<int> NewSiteIds = new List<int>();

            if (!string.IsNullOrEmpty(vm.DivisionId))
                NewDivisionIds = vm.DivisionId.Split(',').Select(Int32.Parse).ToList();

            if (!string.IsNullOrEmpty(vm.SiteId))
                NewSiteIds = vm.SiteId.Split(',').Select(Int32.Parse).ToList();

            var NewDivisionForRoles = from p in NewDivisionIds
                                      select new RolesDivisionViewModel
                                      {
                                          DivisionId = p,
                                          RoleId = vm.RoleId
                                      };

            var NewSiteForRoles = from p in NewSiteIds
                                  select new RolesSiteViewModel
                                  {
                                      SiteId = p,
                                      RoleId = vm.RoleId
                                  };


            var ExistingDivisionsForRoles = new RolesDivisionService(_unitOfWork).GetRolesDivisionList(vm.RoleId).ToList();

            var ExistingSiteForRoles = new RolesSiteService(_unitOfWork).GetRolesSiteList(vm.RoleId).ToList();

            var DivisionPendingToUpdate = (from p in NewDivisionForRoles
                                           join t in ExistingDivisionsForRoles on new { x = p.RoleId, y = p.DivisionId } equals new { x = t.RoleId, y = t.DivisionId } into table
                                           from left in table.DefaultIfEmpty()
                                           where left == null
                                           select p.DivisionId).ToList();

            var DivisionPendingToDelete = (from p in ExistingDivisionsForRoles
                                           join t in NewDivisionForRoles on new { x = p.RoleId, y = p.DivisionId } equals new { x = t.RoleId, y = t.DivisionId } into table
                                           from right in table.DefaultIfEmpty()
                                           where right == null
                                           select p).ToList();



            foreach (int item in DivisionPendingToUpdate)
            {
                RolesDivision temp = new RolesDivision();
                temp.RoleId = vm.RoleId;
                temp.DivisionId = item;
                temp.CreatedBy = User.Identity.Name;
                temp.CreatedDate = DateTime.Now;
                temp.ModifiedBy = User.Identity.Name;
                temp.ModifiedDate = DateTime.Now;
                new RolesDivisionService(_unitOfWork).Create(temp);

            }

            foreach (var item in DivisionPendingToDelete)
            {
                new RolesDivisionService(_unitOfWork).Delete(item.RolesDivisionId);
            }






            var SitePendingToUpdate = (from p in NewSiteForRoles
                                       join t in ExistingSiteForRoles on new { x = p.RoleId, y = p.SiteId } equals new { x = t.RoleId, y = t.SiteId } into table
                                       from left in table.DefaultIfEmpty()
                                       where left == null
                                       select p.SiteId).ToList();

            var SitePendingToDelete = (from p in ExistingSiteForRoles
                                       join t in NewSiteForRoles on new { x = p.RoleId, y = p.SiteId } equals new { x = t.RoleId, y = t.SiteId } into table
                                       from right in table.DefaultIfEmpty()
                                       where right == null
                                       select p).ToList();



            foreach (int item in SitePendingToUpdate)
            {
                RolesSite temp = new RolesSite();
                temp.RoleId = vm.RoleId;
                temp.SiteId = item;
                temp.CreatedBy = User.Identity.Name;
                temp.CreatedDate = DateTime.Now;
                temp.ModifiedBy = User.Identity.Name;
                temp.ModifiedDate = DateTime.Now;
                new RolesSiteService(_unitOfWork).Create(temp);

            }

            foreach (var item in SitePendingToDelete)
            {
                new RolesSiteService(_unitOfWork).Delete(item.RolesSiteId);
            }


            try
            {
                _unitOfWork.Save();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return View("RolesSiteAndDivision", vm);

            }

            //TempData["Validation"] = "Valid";
            //return RedirectToAction("UserPermissions", "Menu", new { RoleId = vm.RoleId });

            return RedirectToAction("SiteDivisionSummary", new { SiteId=vm.SiteId,DivisionId=vm.DivisionId,RoleId=vm.RoleId});
        }


        public ActionResult SiteDivisionSummary(string SiteId,string DivisionId,string RoleId)
        {

            List<int> NewDivisionIds = new List<int>();
            List<int> NewSiteIds = new List<int>();

            if (!string.IsNullOrEmpty(DivisionId))
                NewDivisionIds = DivisionId.Split(',').Select(Int32.Parse).ToList();

            if (!string.IsNullOrEmpty(SiteId))
                NewSiteIds = SiteId.Split(',').Select(Int32.Parse).ToList();

            var SelectedSites = from p in db.Site
                                where SiteId.Contains(p.SiteId.ToString())
                                select p;

            var SelectedDivisions = from p in db.Divisions
                                    where DivisionId.Contains(p.DivisionId.ToString())
                                    select p;


            var summary = from p in SelectedSites
                          from t in SelectedDivisions
                          select new SiteDivisionSummaryViewModel
                          {
                              SiteId = p.SiteId,
                              SiteName = p.SiteCode,
                              SiteColour=p.ThemeColour,
                              DivisionId = t.DivisionId,
                              DivisionName = t.DivisionName,
                              DivisionColour=p.ThemeColour,
                              RoleId=RoleId,
                          };


            return View(summary.ToList());
        }

        public ActionResult SelectedSiteDivision(int SiteId,int DivisionId,string Id)
        {
            System.Web.HttpContext.Current.Session["UserPermissionDivisionId"] = DivisionId;
            System.Web.HttpContext.Current.Session["UserPermissionSiteId"] = SiteId;

            System.Web.HttpContext.Current.Session["UserPermissionSiteColour"] = (from p in db.Site where p.SiteId==SiteId select p.ThemeColour).FirstOrDefault();

            TempData["Validation"] = "Valid";
            return RedirectToAction("UserPermissions", "Menu", new { RoleId = Id });
        }


        public ActionResult GetActionsForMenu(int MenuId)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            int ControllerId = (from p in db.Menu
                                join t in db.ControllerAction on p.ControllerActionId equals t.ControllerActionId
                                join t2 in db.MvcController on t.ControllerId equals t2.ControllerId
                                where p.MenuId == MenuId
                                select
                                    t2.ControllerId
                        ).FirstOrDefault();

            var list = (from p in db.ControllerAction
                        join t in db.RolesControllerAction.Where(m => m.RoleId == RoleId) on p.ControllerActionId equals t.ControllerActionId into table
                        from tab in table.DefaultIfEmpty()
                        where p.ControllerId == ControllerId
                        orderby p.ActionName
                        select new
                        {
                            p.ControllerActionId,
                            p.ActionName,
                            IsAssigned = tab == null ? false : true,
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLineActionsForMenu(int MenuId)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            string ControllerId = string.Join(",", (from p in db.Menu
                                                    join t in db.MvcController on p.ControllerAction.ControllerId equals t.ParentControllerId
                                                    join t2 in db.MvcController on t.ControllerId equals t2.ControllerId
                                                    where p.MenuId == MenuId
                                                    select
                                                        t2.ControllerId
              ).ToList());

            var list = (from p in db.ControllerAction
                        join t in db.RolesControllerAction.Where(m => m.RoleId == RoleId) on p.ControllerActionId equals t.ControllerActionId into table
                        from tab in table.DefaultIfEmpty()
                        where ControllerId.Contains(p.ControllerId.ToString())
                        orderby p.ActionName
                        select new
                        {
                            p.ControllerActionId,
                            p.ActionName,
                            ControllerName = p.Controller.ControllerName,
                            IsAssigned = tab == null ? false : true,
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CopyPermissions()
        {
            CopyRolesViewModel vm = new CopyRolesViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CopyPermissions(CopyRolesViewModel vm)
        {
            if (ModelState.IsValid && vm.FromRoleId != vm.ToRoleId)
            {

                var RolesSites = new RolesSiteService(_unitOfWork).GetRolesSiteList(vm.FromRoleId);
                var ExistingRolesSites = new RolesSiteService(_unitOfWork).GetRolesSiteList(vm.ToRoleId);

                var PendingToUpdate=from p in RolesSites
                                    join t in ExistingRolesSites on p.RolesSiteId equals t.RolesSiteId into table
                                    from left in table.DefaultIfEmpty()
                                    where left==null
                                    select p;

                foreach (var item in PendingToUpdate)
                {
                    RolesSite site = new RolesSite();
                    site.CreatedBy = User.Identity.Name;
                    site.CreatedDate = DateTime.Now;
                    site.ModifiedBy = User.Identity.Name;
                    site.ModifiedDate = DateTime.Now;
                    site.RoleId = vm.ToRoleId;
                    site.SiteId = item.SiteId;
                    site.ObjectState = Model.ObjectState.Added;
                    new RolesSiteService(_unitOfWork).Create(site);

                }

                var RolesDivisions = new RolesDivisionService(_unitOfWork).GetRolesDivisionList(vm.FromRoleId);
                var ExistingRolesDivisions = new RolesDivisionService(_unitOfWork).GetRolesDivisionList(vm.ToRoleId);

                var PendingDivisionsToUpdate = from p in RolesDivisions
                                               join t in ExistingRolesDivisions on p.RolesDivisionId equals t.RolesDivisionId into table
                                               from left in table.DefaultIfEmpty()
                                               where left == null
                                               select p;

                foreach (var item in PendingDivisionsToUpdate)
                {
                    RolesDivision division = new RolesDivision();
                    division.CreatedBy = User.Identity.Name;
                    division.CreatedDate = DateTime.Now;
                    division.DivisionId = item.DivisionId;
                    division.ModifiedBy = User.Identity.Name;
                    division.ModifiedDate = DateTime.Now;
                    division.RoleId = vm.ToRoleId;
                    division.ObjectState = Model.ObjectState.Added;
                    new RolesDivisionService(_unitOfWork).Create(division);
                }

                var RolesMenus = new RolesMenuService(_unitOfWork).GetRolesMenuList(vm.FromRoleId);
                var ExistingRolesMenus = new RolesMenuService(_unitOfWork).GetRolesMenuList(vm.ToRoleId);

                var PendingMenusToUpDate = from p in RolesMenus
                                           join t in ExistingRolesMenus on p.RolesMenuId equals t.RolesMenuId into table
                                           from left in table.DefaultIfEmpty()
                                           where left == null
                                           select p;

                foreach (var item in PendingMenusToUpDate)
                {
                    RolesMenu menu = new RolesMenu();
                    menu.CreatedBy = User.Identity.Name;
                    menu.CreatedDate = DateTime.Now;
                    menu.FullHeaderPermission = item.FullHeaderPermission;
                    menu.FullLinePermission = item.FullLinePermission;
                    menu.MenuId = item.MenuId;
                    menu.ModifiedBy = User.Identity.Name;
                    menu.ModifiedDate = DateTime.Now;
                    menu.RoleId = vm.ToRoleId;
                    menu.ObjectState = Model.ObjectState.Added;
                    new RolesMenuService(_unitOfWork).Create(menu);
                }

                var RolesActions = new RolesControllerActionService(_unitOfWork).GetRolesControllerActionList(vm.FromRoleId);
                var ExistingRolesActions = new RolesControllerActionService(_unitOfWork).GetRolesControllerActionList(vm.ToRoleId);

                var PendingRolesActionsToUpdate = from p in RolesActions
                                                  join t in ExistingRolesActions on p.RolesControllerActionId equals t.RolesControllerActionId into table
                                                  from left in table.DefaultIfEmpty()
                                                  where left == null
                                                  select p;


                foreach (var item in PendingRolesActionsToUpdate)
                {
                    RolesControllerAction Actions = new RolesControllerAction();
                    Actions.ControllerActionId = item.ControllerActionId;
                    Actions.CreatedBy = User.Identity.Name;
                    Actions.CreatedDate = DateTime.Now;
                    Actions.ModifiedBy = User.Identity.Name;
                    Actions.ModifiedDate = DateTime.Now;
                    Actions.RoleId = vm.ToRoleId;
                    Actions.ObjectState = Model.ObjectState.Added;
                    new RolesControllerActionService(_unitOfWork).Create(Actions);
                }

                _unitOfWork.Save();

                return RedirectToAction("Module", "Menu");

            }

            return View(vm);
        }

        public JsonResult GetRoles(string term)
        {

            var temp = (from p in db.Roles
                        select p);

            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public static class ValidateData
        {
            private static ApplicationDbContext db = new ApplicationDbContext();
            public static bool ValidateUserPermission(string ActionName, string ControllerName)
            {
                bool Temp = false;
                if (System.Web.HttpContext.Current.Session["CAPermissionsCacheKeyHint"] != null)
                {
                    var CacheData = (List<RolesControllerActionViewModel>)System.Web.HttpContext.Current.Session["CAPermissionsCacheKeyHint"];

                    Temp = (from p in CacheData
                                 where p.ControllerName == ControllerName && p.ControllerActionName==ActionName
                                 select p).Any();
                }
                return Temp;
            }
        }

    }


}