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
using Surya.India.Core.Common;
using System.Net;
using System.Data.SqlClient;
using System.Web.Security;

namespace Module
{
    [Authorize]
    public class SiteSelectionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        IUnitOfWork _unitOfWork;
        public SiteSelectionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        [Authorize]
        [HttpGet]
        public ActionResult SiteSelection()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            string UserId = (string)System.Web.HttpContext.Current.Session["ApplicationUserId"];
            IEnumerable<string> userInRoles = (from p in db.UserRole
                                               where p.UserId == UserId
                                               select p.RoleId).ToList();

            if (userInRoles.Count() <= 0)
            {

                AuthenticationManager.SignOut();
                FormsAuthentication.SignOut();
                Session.Abandon();
                return View("NoRoles");
            }




            SiteSelectionViewModel vm = new SiteSelectionViewModel();

            AssignSession();

            IEnumerable<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Testing Block

            var temp = (from p in db.Roles
                        select p).ToList();

            var RoleIds = string.Join(",", from p in temp
                                           where UserRoles.Contains(p.Name)
                                           select p.Id.ToString());
            //End

            if (UserRoles.Contains("Admin"))
            {
                ViewBag.SiteList = new SiteService(_unitOfWork).GetSiteList().ToList();
                ViewBag.DivisionList = new DivisionService(_unitOfWork).GetDivisionList().ToList();
            }
            else
            {
                var SiteList = new SiteService(_unitOfWork).GetSiteList(RoleIds).ToList();
                ViewBag.SiteList = SiteList;
                var DivList = new DivisionService(_unitOfWork).GetDivisionList(RoleIds).ToList();
                ViewBag.DivisionList = DivList;
                if (SiteList.Count == 0 || DivList.Count == 0)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
            }
            if (System.Web.HttpContext.Current.Session["DivisionId"] != null && System.Web.HttpContext.Current.Session["SiteId"] != null)
            {
                vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            }

            return View(vm);
        }


        private void AssignSession()
        {

            //var UManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            //SqlParameter SqlParameterUserId = new SqlParameter("@UserId", (string)System.Web.HttpContext.Current.Session["ApplicationUserId"]);
            //IEnumerable<string> UserRoles = db.Database.SqlQuery<string>("web.sp_GetUserRoles @UserId", SqlParameterUserId).ToList();
            var Today = DateTime.Now.Date;

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            //IEnumerable<string> UserRoles = UserManager.GetRoles(User.Identity.GetUserId());
            var UserId = User.Identity.GetUserId();
            IEnumerable<string> UserRoles = (from p in db.UserRole
                                             join t in db.Roles on p.RoleId equals t.Id
                                             where p.UserId == UserId && (p.ExpiryDate == null || p.ExpiryDate >= Today)
                                             group t by t.Id into g
                                             select g.Max(m => m.Name)).ToList();



            System.Web.HttpContext.Current.Session["Roles"] = UserRoles;
            System.Web.HttpContext.Current.Session["CompanyId"] = 1;
            System.Web.HttpContext.Current.Session["CompanyName"] = "SURYA CARPET PVT. LTD.";


            Dictionary<int, string> bookmarks = new Dictionary<int, string>();
            var temp = new UserBookMarkService(_unitOfWork).GetUserBookMarkListForUser(User.Identity.Name);
            foreach (var item in temp)
            {
                bookmarks.Add(item.MenuId, item.MenuName);
            }

            List<UserBookMarkViewModel> vm = new List<UserBookMarkViewModel>();
            foreach (var item in temp)
            {
                vm.Add(new UserBookMarkViewModel()
                {
                    IconName = item.IconName,
                    MenuId = item.MenuId,
                    MenuName = item.MenuName,
                });
            }

            System.Web.HttpContext.Current.Session["BookMarks"] = vm;

            if (!UserRoles.Contains("Admin"))
            {
                List<RolesControllerActionViewModel> Temp = new RolesControllerActionService(_unitOfWork).GetRolesControllerActionsForRoles(UserRoles.ToList()).ToList();

                System.Web.HttpContext.Current.Session["CAPermissionsCacheKeyHint"] = Temp.ToList();
            }

        }


        [HttpPost]
        public ActionResult SiteSelection(SiteSelectionViewModel vm)
        {
            System.Web.HttpContext.Current.Session["DivisionId"] = vm.DivisionId;
            System.Web.HttpContext.Current.Session["SiteId"] = vm.SiteId;

            Site S = new SiteService(_unitOfWork).Find(vm.SiteId);
            Division D = new DivisionService(_unitOfWork).Find(vm.DivisionId);

            System.Web.HttpContext.Current.Session[SessionNameConstants.LoginDivisionId] = vm.DivisionId;
            System.Web.HttpContext.Current.Session[SessionNameConstants.LoginSiteId] = vm.SiteId;
            System.Web.HttpContext.Current.Session[SessionNameConstants.CompanyName] = S.SiteName;
            System.Web.HttpContext.Current.Session[SessionNameConstants.SiteName] = S.SiteName;
            System.Web.HttpContext.Current.Session[SessionNameConstants.SiteShortName] = S.SiteCode;
            System.Web.HttpContext.Current.Session[SessionNameConstants.SiteAddress] = S.Address;
            System.Web.HttpContext.Current.Session[SessionNameConstants.SiteCityName] = S.City.CityName;
            System.Web.HttpContext.Current.Session[SessionNameConstants.DivisionName] = D.DivisionName;


            return RedirectToAction("DefaultGodownSelection");
        }


        public ActionResult DefaultGodownSelection()
        {

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            ViewBag.GodownList = (from p in db.Godown
                                  where p.SiteId == SiteId && p.IsActive == true
                                  orderby p.GodownName
                                  select p).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult DefaultGodownSelection(int? DefaultGodownId)
        {
            if (DefaultGodownId.HasValue && DefaultGodownId.Value > 0)
                System.Web.HttpContext.Current.Session["DefaultGodownId"] = DefaultGodownId;
            return RedirectToAction("Module", "Menu");
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