using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Surya.India.Model.Models;
using Surya.India.Data.Models;
using Surya.India.Service;
using Surya.India.Data.Infrastructure;

namespace Surya.India.Presentation.Controllers
{
    [Authorize]
    public class UserPermissionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();        
        IModuleService _ModuleService;
        ISubModuleService _SubModuleService;
        IUnitOfWork _unitOfWork;        
        public UserPermissionController(IModuleService mService, IUnitOfWork unitOfWork,ISubModuleService serv)
        {
            _ModuleService = mService;
            _unitOfWork = unitOfWork;
            _SubModuleService = serv;            
        }


        public JsonResult AddPermission(int caid)        
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            RolesMenu Rm = new RolesMenu();
            Rm.RoleId = RoleId;
            Rm.CreatedBy = User.Identity.Name;
            Rm.CreatedDate = DateTime.Now;
            Rm.DivisionId = (int)System.Web.HttpContext.Current.Session["UserPermissionDivisionId"];
            Rm.SiteId = (int)System.Web.HttpContext.Current.Session["UserPermissionSiteId"];
            Rm.ModifiedBy = User.Identity.Name;
            Rm.ModifiedDate = DateTime.Now;
            Rm.MenuId = caid;

            int AssignedActionsCount = new RolesMenuService(_unitOfWork).GetPermittedActionsCountForMenuId(caid, RoleId);
            if(AssignedActionsCount<=0)
            {
                Rm.FullHeaderPermission = true;
            }

            int AssignedChildActionCount = new RolesMenuService(_unitOfWork).GetChildPermittedActionsCountForMenuId(caid, RoleId);
            if(AssignedChildActionCount<=0)
            {
                Rm.FullLinePermission = true;
            }


            new RolesMenuService(_unitOfWork).Create(Rm);
            _unitOfWork.Save();
            
            return Json(new { success = true });

        }

        public JsonResult RemovePermission(int caid)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            RolesMenu temp = new RolesMenuService(_unitOfWork).GetRoleMenuForRoleId(RoleId,caid);

            new RolesMenuService(_unitOfWork).Delete(temp);

            _unitOfWork.Save();         

            return Json(new { success = true });

        }

        public JsonResult AddPermissionForAction(int ActionId)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            RolesControllerAction Rm = new RolesControllerAction();
            Rm.RoleId = RoleId;
            Rm.CreatedBy = User.Identity.Name;
            Rm.CreatedDate = DateTime.Now;
            Rm.ModifiedBy = User.Identity.Name;
            Rm.ModifiedDate = DateTime.Now;
            Rm.ControllerActionId = ActionId;

            new RolesControllerActionService(_unitOfWork).Create(Rm);
            _unitOfWork.Save();

            return Json(new { success = true });

        }

        public JsonResult RemovePermissionForAction(int ActionId)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            RolesControllerAction temp = new RolesControllerActionService(_unitOfWork).GetControllerActionForRoleId(RoleId, ActionId);

            new RolesControllerActionService(_unitOfWork).Delete(temp);

            _unitOfWork.Save();

            return Json(new { success = true });

        }

        public JsonResult AddPermissionForMenu(int MenuId)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            var ControllerId = (from p in db.Menu
                                        where p.MenuId == MenuId
                                        select p.ControllerAction.ControllerId).FirstOrDefault();

            List<int> ControllerActionIdList = (from p in db.ControllerAction
                                            where p.ControllerId == ControllerId
                                            select p.ControllerActionId).ToList();

            var ExistingRoles = (from p in db.RolesControllerAction
                                 join t in db.ControllerAction on p.ControllerActionId equals t.ControllerActionId
                                 where p.RoleId == RoleId && t.ControllerId == ControllerId
                                 select p
                                   );

            var PendingActionsToUpdate = (from p in ControllerActionIdList
                                          join t in ExistingRoles on p equals t.ControllerActionId
                                          into table
                                          from left in table.DefaultIfEmpty()
                                          where left == null
                                          select p);

            foreach(int item in PendingActionsToUpdate)
            {

            

            RolesControllerAction Rm = new RolesControllerAction();
            Rm.RoleId = RoleId;
            Rm.CreatedBy = User.Identity.Name;
            Rm.CreatedDate = DateTime.Now;
            Rm.ModifiedBy = User.Identity.Name;
            Rm.ModifiedDate = DateTime.Now;
            Rm.ControllerActionId = item;

            new RolesControllerActionService(_unitOfWork).Create(Rm);

            }
            _unitOfWork.Save();

            return Json(new { success = true });

        }

        public JsonResult AddLinePermissionForMenu(int MenuId)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            var ControllerId = (from p in db.Menu
                                        where p.MenuId == MenuId
                                        select p.ControllerAction.ControllerId).FirstOrDefault();

            string ControllerIDs = string.Join(",", (from p in db.MvcController
                                                     where p.ParentControllerId == ControllerId
                                                     select p.ControllerId
                                                      ).ToList());

            List<int> ControllerActionIdList = (from p in db.ControllerAction
                                            where ControllerIDs.Contains(p.ControllerId.ToString())
                                            select p.ControllerActionId).ToList();

            var ExistingRoles = (from p in db.RolesControllerAction
                                 join t in db.ControllerAction on p.ControllerActionId equals t.ControllerActionId
                                 where p.RoleId == RoleId && ControllerIDs.Contains(t.ControllerId.ToString())
                                 select p
                                   );

            var PendingActionsToUpdate = (from p in ControllerActionIdList
                                          join t in ExistingRoles on p equals t.ControllerActionId
                                          into table
                                          from left in table.DefaultIfEmpty()
                                          where left == null
                                          select p);

            foreach (int item in PendingActionsToUpdate)
            {
                RolesControllerAction Rm = new RolesControllerAction();
                Rm.RoleId = RoleId;
                Rm.CreatedBy = User.Identity.Name;
                Rm.CreatedDate = DateTime.Now;
                Rm.ModifiedBy = User.Identity.Name;
                Rm.ModifiedDate = DateTime.Now;
                Rm.ControllerActionId = item;

                new RolesControllerActionService(_unitOfWork).Create(Rm);

            }
            _unitOfWork.Save();

            return Json(new { success = true });

        }

        public JsonResult RemovePermissionForMenu(int MenuId)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            var ControllerActionList = (from p in db.Menu
                                        where p.MenuId == MenuId
                                        select p.ControllerAction.ControllerId).FirstOrDefault();

            List<int> ControllerActionId = (from p in db.ControllerAction
                                            where p.ControllerId == ControllerActionList
                                            select p.ControllerActionId).ToList();

            foreach (int item in ControllerActionId)
            {

                RolesControllerAction temp = new RolesControllerActionService(_unitOfWork).GetControllerActionForRoleId(RoleId, item);

                new RolesControllerActionService(_unitOfWork).Delete(temp);
            }
            _unitOfWork.Save();

            return Json(new { success = true });

        }

        public JsonResult RemoveLinePermissionForMenu(int MenuId)
        {

            string RoleId = (string)System.Web.HttpContext.Current.Session["RoleUId"];

            var ControllerActionId = (from p in db.Menu
                                        where p.MenuId == MenuId
                                        select p.ControllerAction.ControllerId).FirstOrDefault();

            string ControllerIDs = string.Join(",", (from p in db.MvcController
                                                     where p.ParentControllerId == ControllerActionId
                                                     select p.ControllerId
                                                      ).ToList());

            List<int> ControllerActionIdList = (from p in db.ControllerAction
                                            where ControllerIDs.Contains(p.ControllerId.ToString())
                                            select p.ControllerActionId).ToList();

            foreach (int item in ControllerActionIdList)
            {

                RolesControllerAction temp = new RolesControllerActionService(_unitOfWork).GetControllerActionForRoleId(RoleId, item);

                new RolesControllerActionService(_unitOfWork).Delete(temp);
            }
            _unitOfWork.Save();

            return Json(new { success = true });

        }


    }
}