using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Surya.India.Data.Models;
using Surya.India.Model.Models;
using System.Configuration;
using Surya.India.Presentation;
using Module.Controllers;
using System.Data.SqlClient;
using Surya.India.Service;

namespace Surya.India.Web.Controllers
{
    [Authorize]

    public class UsersAdminController : System.Web.Mvc.Controller
    {
        IExceptionHandlingService _exception;
        public UserManager<ApplicationUser> UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public ApplicationDbContext context { get; private set; }
        public LoginApplicationDbContext Lcontext { get; private set; }
        public UsersAdminController()
        {
            Lcontext = new LoginApplicationDbContext();
            context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }
        public UsersAdminController(IExceptionHandlingService exec)
        {
            _exception = exec;
        }

        public UsersAdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }



        //
        // GET: /Users/
        public ActionResult Index()
        {
            //Syncs();

            List<UserRolesViewModel> userRoleList = new List<UserRolesViewModel>();

            var users = UserManager.Users.Include(r => r.Roles).ToList().OrderBy(m => m.UserName);

            foreach (ApplicationUser user in users)
            {
                var usrRolsVm = new UserRolesViewModel { Id = user.Id, UserName = user.UserName, Email = user.Email };
                //var roles = user.Roles.ToList();
                IEnumerable<string> roles = UserManager.GetRoles(user.Id).ToList();
                if (roles != null)
                {
                    string userRoles = string.Empty;
                    foreach (var role in roles)
                    {
                        //var roleId = role.RoleId;
                        //var roleName = RoleManager.FindById(roleId).Name;

                    }

                    usrRolsVm.Roles = string.Join(",", roles);

                }
                userRoleList.Add(usrRolsVm);
            }

            return View(userRoleList);
        }


        public ActionResult UserRoles(string id)
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.Id == id);
            var model = new SelectUserRolesViewModel(user as ApplicationUser);
            return View("Edit",model);
        }

        [HttpPost]
        //
        [AllowAnonymous]

        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            //If user has existing Role then remove the user from the role
            // This also accounts for the case when the Admin selected Empty from the drop-down and
            // this means that all roles for the user must be removed
            ApplicationDbContext db = new ApplicationDbContext();

            var rolesForUser = UserManager.GetRoles(model.UserId);
            if (rolesForUser.Count() > 0)
            {
                foreach (var item in rolesForUser.ToList())
                {
                    var result = UserManager.RemoveFromRole(model.UserId, item);
                }
            }

            foreach (var role in model.Roles.ToList())
            {
                if (role.Selected)
                {   //Find Role
                    var rol = RoleManager.FindByName(role.RoleName);
                    //Add user to new role
                    var result = UserManager.AddToRole(model.UserId, role.RoleName);

                    if (result.Succeeded)
                    {
                        var email = UserManager.GetEmail(model.UserId);
                        //TODO: Need to check for new user only. query to people table for existing user 
                        SendRoleConfirmationToUser(model, email);
                    }
                }
            }


            return RedirectToAction("index");
        }

        private void SendRoleConfirmationToUser(SelectUserRolesViewModel model, string email)
        {
            //GMailer.GmailUsername = ConfigurationManager.AppSettings["EmailUser"];
            //GMailer.GmailPassword = ConfigurationManager.AppSettings["Emailpassword"];
            //string domain = ConfigurationManager.AppSettings["domain"];
            //GMailer mailer = new GMailer();
            //mailer.ToEmail = email;// you can use comma separated email addressess in TO (Recipients)
            ////mailer.CC = "callratnesh@gmail.com";// you can use comma separated email addressess in CC
            //mailer.Subject = "Access Confirmed";
            //mailer.Body = "You can access Surya now. Please login to surya web. ";
            //mailer.Body += "<br>";
            //mailer.Body += "Please click http://" + domain;
            //mailer.IsHtml = true;
            //mailer.Send();
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Id", "Name");
            return View();
        }


        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                user.UserName = userViewModel.UserName;
                //user.HomeTown = userViewModel.HomeTown;

                user.ObjectState = Model.ObjectState.Added;
                //user.CreatedDate = DateTime.Now;
                //user.ModifiedDate = DateTime.Now;
                //user.CreatedBy = User.Identity.Name;
                //user.ModifiedBy = User.Identity.Name;

                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User Admin to Role Admin
                if (adminresult.Succeeded)
                {
                    /* commenting for Time being since  await UserManager.AddToRoleAsync(user.Id, role.Name); throwing run time error
                    if (!String.IsNullOrEmpty(RoleId))
                    {
                        //Find Role Admin
                        var role = await RoleManager.FindByIdAsync(RoleId);
                        var result = await UserManager.AddToRoleAsync(user.Id, role.Name);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First().ToString());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Id", "Name");
                            return View();
                        }
                    }

                    */
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First().ToString());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                    return View();

                }
                return RedirectToAction("Index").Success("Data saved successfully");
            }
            else
            {
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                return View();
            }
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserName,Id,HomeTown")] ApplicationUser formuser, string id, string RoleId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
            var user = await UserManager.FindByIdAsync(id);
            user.UserName = formuser.UserName;
            //user.HomeTown = formuser.HomeTown;

            if (ModelState.IsValid)
            {
                user.ObjectState = Model.ObjectState.Modified;
                //user.CreatedDate = DateTime.Now;
                //user.ModifiedDate = DateTime.Now;
                //user.CreatedBy = User.Identity.Name;
                //user.ModifiedBy = User.Identity.Name;
                //Update the user details
                await UserManager.UpdateAsync(user);

                //If user has existing Role then remove the user from the role
                // This also accounts for the case when the Admin selected Empty from the drop-down and
                // this means that all roles for the user must be removed
                var rolesForUser = await UserManager.GetRolesAsync(id);
                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser)
                    {
                        var result = await UserManager.RemoveFromRoleAsync(id, item);
                    }
                }

                if (!String.IsNullOrEmpty(RoleId))
                {
                    //Find Role
                    var role = await RoleManager.FindByIdAsync(RoleId);
                    //Add user to new role
                    var result = await UserManager.AddToRoleAsync(id, role.Name);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First().ToString());
                        ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                        return View();
                    }
                }
                return RedirectToAction("Index").Success("Data saved successfully");
            }
            else
            {
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                return View();
            }
        }


        public ActionResult Sync()
        {
            try
            {
                Syncs();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = _exception.HandleException(ex);
                return View("Error");
            }

            return Redirect("/Menu/Module");
        }

        private static void Syncs()
        {
            IEnumerable<ApplicationUser> FromList;
            IEnumerable<ApplicationUser> ToList;

            ApplicationDbContext db = new ApplicationDbContext();
            LoginApplicationDbContext ldb = new LoginApplicationDbContext();

            FromList = ldb.Database.SqlQuery<ApplicationUser>(" SELECT * FROM AspNetUsers ").ToList();
            ToList = db.Database.SqlQuery<ApplicationUser>(" SELECT * FROM Web.Users ").ToList();


            IEnumerable<ApplicationUser> PendingToUpdate;

            PendingToUpdate = from p in FromList
                              join t in ToList on p.Id equals t.Id into Left
                              from lef in Left.DefaultIfEmpty()
                              where lef == null
                              select p;

            foreach (var item in PendingToUpdate)
            {
                ApplicationUser NewRec = new ApplicationUser();
                NewRec = item;
                NewRec.ObjectState = Model.ObjectState.Added;
                db.Users.Add(NewRec);
            }

            db.SaveChanges();

        }

    }
}
