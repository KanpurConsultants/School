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
using System.Reflection;
using Surya.India.Model.ViewModel;
using System.Xml.Linq;

namespace Surya.India.Web
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public UserRolesController(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /UserRoleMaster/

        public ActionResult Index()
        {
            //var test = db.Users.Include(m => m.Roles).ToList();

            var UserRole = (from p in db.Users
                            join t1 in db.UserRole on p.Id equals t1.UserId into table
                            from tab in table.DefaultIfEmpty()
                            join t2 in db.Roles on tab.RoleId equals t2.Id into table2
                            from tab2 in table2.DefaultIfEmpty()
                            where tab.ExpiryDate == null
                            select new UserRoleViewModel
                            {
                                UserName = p.UserName,
                                Email = p.Email,
                                UserId = p.Id,
                                RolesList = tab2.Name,
                            }).ToList();

            var GroupedRolesList = (from p in UserRole
                                    group p by p.UserId into g
                                    orderby g.Max(m => m.UserName)
                                    select new UserRoleViewModel
                                    {
                                        UserName = g.Max(m => m.UserName),
                                        Email = g.Max(m => m.Email),
                                        RolesList = string.Join(",", g.Select(m => m.RolesList)),
                                        UserId = g.Key,
                                    }).ToList();

            return View(GroupedRolesList);
            //return RedirectToAction("Create");
        }

        // GET: /UserRoleMaster/Create

        public ActionResult UpdateRoles(string UserId)
        {
            UserRoleViewModel vm = new UserRoleViewModel();
            vm.UserId = UserId;
            vm.UserName = db.Users.Find(UserId).UserName;
            vm.RoleIdList = (string.Join(",", (from p in db.UserRole
                                               where p.UserId == UserId && p.ExpiryDate == null
                                               select p.RoleId).ToList()));
            return View("Create", vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(UserRoleViewModel vm)
        {
            string NewUserRoles = "New Roles Assigned to User " + vm.UserId + " RoleIds: ";
            string OldUserRoles = "Roles Deleted From User " + vm.UserId + " RoleIds: ";
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(vm.RoleIdList))
                    foreach (var item in vm.RoleIdList.Split(',').ToList())
                    {
                        UserRole pt = new UserRole();
                        pt.UserId = vm.UserId;
                        pt.RoleId = item;
                        pt.ObjectState = Model.ObjectState.Added;
                        db.UserRole.Add(pt);

                    }

                NewUserRoles += vm.RoleIdList;

                var ExistingRoles = (from p in db.UserRole
                                     where p.UserId == vm.UserId
                                     && p.ExpiryDate == null
                                     select p).ToList();

                if (ExistingRoles.Count > 0)
                    OldUserRoles += string.Join(",", ExistingRoles.Select(m => m.RoleId).ToList());

                foreach (var item in ExistingRoles)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.UserRole.Add(item);
                }

                try
                {
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ModelState.AddModelError("", message);
                    return View("Create", vm);
                }

                var DocTypeId=new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.UserRoles).DocumentTypeId;

                LogActivity.LogActivityDetail(DocTypeId,
                0,
                null,
                (int)ActivityTypeContants.Added,
                NewUserRoles,
                User.Identity.Name, "", null);

                LogActivity.LogActivityDetail(DocTypeId,
                0,
                null,
                (int)ActivityTypeContants.Deleted,
                OldUserRoles,
                User.Identity.Name, "", null);

                return RedirectToAction("Index").Success("Data saved successfully");

            }
            return View("Create", vm);
        }


        public ActionResult TempRolesIndex()
        {

            DateTime Today = DateTime.Now.Date;

            var Users = (from p in db.Users
                         select new UserRoleViewModel
                         {
                             UserId = p.Id,
                             UserName = p.UserName,
                             Email = p.Email,
                         }).ToList();

            return View(Users);
            //return RedirectToAction("Create");
        }

        // GET: /UserRoleMaster/Create

        public ActionResult UpdateTempRoles(string UserId)
        {
            UserRoleViewModel vm = new UserRoleViewModel();
            vm.UserId = UserId;
            vm.ExpiryDate = DateTime.Now;
            vm.UserName = db.Users.Find(UserId).UserName;
            //vm.RoleIdList = (string.Join(",", (from p in db.UserRole
            //                                   where p.UserId == UserId && p.ExpiryDate != null && p.ExpiryDate >= DateTime.Now
            //                                   select p.RoleId).ToList()));
            return View("CreateTempRoles", vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostTempRoles(UserRoleViewModel vm)
        {
            string NewUserRoles = "New Temporary Roles Assigned to User " + vm.UserId + " RoleIds: ";

            if (!vm.ExpiryDate.HasValue || vm.ExpiryDate < DateTime.Now.Date)
                ModelState.AddModelError("ExpiryDate", "Expiry date field is required.");
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(vm.RoleIdList))
                    foreach (var item in vm.RoleIdList.Split(',').ToList())
                    {
                        UserRole pt = new UserRole();
                        pt.UserId = vm.UserId;
                        pt.RoleId = item;
                        pt.ExpiryDate = vm.ExpiryDate;
                        pt.ObjectState = Model.ObjectState.Added;
                        db.UserRole.Add(pt);
                    }

                NewUserRoles += vm.RoleIdList;

                try
                {
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ModelState.AddModelError("", message);
                    return View("CreateTempRoles", vm);
                }

                LogActivity.LogActivityDetail(new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.UserRoles).DocumentTypeId,
                0,
                null,
                (int)ActivityTypeContants.Added,
                NewUserRoles,
                User.Identity.Name, "", null);



                return RedirectToAction("TempRolesIndex").Success("Data saved successfully");

            }
            return View("CreateTempRoles", vm);
        }



        [HttpGet]
        public ActionResult Copy()
        {
            return PartialView("Copy");
        }


        [HttpPost]
        public ActionResult CopyFromExisting(UserRoleViewModel Vm)
        {

            if (!string.IsNullOrEmpty(Vm.UserId))
            {
                var RolesList = (from p in db.UserRole
                                 join t in db.Roles on p.RoleId equals t.Id
                                 where p.UserId == Vm.UserId && p.ExpiryDate == null
                                 select new
                                 {
                                     id = p.RoleId,
                                     text = t.Name,
                                 }).ToArray();

                return Json(new { success = true, data = RolesList });

            }
            return Json(new { success = false });

        }

        [HttpGet]
        public ActionResult GetUserTempRoles(string UserId)
        {

            var Today = DateTime.Now.Date;
            ViewBag.UserName = db.Users.Find(UserId).UserName;
            var RoleIdList = (from p in db.UserRole
                              join t in db.Roles on p.RoleId equals t.Id
                              join t2 in db.Users on p.UserId equals t2.Id
                              where p.UserId == UserId && p.ExpiryDate != null && p.ExpiryDate >= Today
                              select new UserRoleViewModel
                              {
                                  UserId = t2.Id,
                                  ExpiryDate = p.ExpiryDate,
                                  RoleName = t.Name,
                              }).ToList();

            var GroupList = (from p in RoleIdList
                             group p by p.ExpiryDate into g
                             select new UserRoleViewModel
                             {
                                 UserId = g.Max(m => m.UserId),
                                 ExpiryDate = g.Key,
                                 RolesList = string.Join(",", g.Select(m => m.RoleName).ToList()),
                             }).ToList();

            return PartialView("UserTempRoles", GroupList);
        }

        public ActionResult DeleteTempUserRole(DateTime ExpiryDate, string UserId)
        {
            string OldUserRoles = "Temporary Roles Deleted From User " + UserId + " RoleIds: ";
            if (!string.IsNullOrEmpty(UserId))
            {

                var TempUserRoleRecords = (from p in db.UserRole
                                           where p.UserId == UserId && p.ExpiryDate == ExpiryDate
                                           select p).ToList();

                OldUserRoles += string.Join(",", TempUserRoleRecords.Select(m => m.RoleId).ToList());

                foreach (var item in TempUserRoleRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.UserRole.Remove(item);
                }

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false });
                }


                LogActivity.LogActivityDetail(new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.UserRoles).DocumentTypeId,
                0,
                null,
                (int)ActivityTypeContants.Deleted,
                OldUserRoles,
                User.Identity.Name, "", null);

                return Json(new { success = true });

            }
            else
                return Json(new { success = false });
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
