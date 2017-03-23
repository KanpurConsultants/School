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
    public class UserBookMarkController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();        
        IModuleService _ModuleService;
        ISubModuleService _SubModuleService;
        IUnitOfWork _unitOfWork;
        IUserBookMarkService _userBookMarkService;
        public UserBookMarkController(IModuleService mService, IUnitOfWork unitOfWork,ISubModuleService serv,IUserBookMarkService userbokser)
        {
            _ModuleService = mService;
            _unitOfWork = unitOfWork;
            _SubModuleService = serv;
            _userBookMarkService = userbokser;
        }


        public JsonResult AddBookMark(int caid)
        
        {

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));


            string appuserid=User.Identity.Name;


            UserBookMark ub = new UserBookMark();
            ub.ApplicationUserName = appuserid;
            ub.CreatedBy = User.Identity.Name;
            ub.CreatedDate = DateTime.Now;
            ub.ModifiedBy = User.Identity.Name;
            ub.ModifiedDate = DateTime.Now;
            ub.MenuId = caid;

            _userBookMarkService.Create(ub);
            _unitOfWork.Save();

            List<UserBookMarkViewModel> bookmark = (List<UserBookMarkViewModel>)(System.Web.HttpContext.Current.Session["BookMarks"]);
            Menu menu=new MenuService(_unitOfWork).Find(ub.MenuId);

            bookmark.Add(new UserBookMarkViewModel()
            {
                IconName = menu.IconName,
                MenuId = menu.MenuId,
                MenuName = menu.MenuName,
            });
            System.Web.HttpContext.Current.Session["BookMarks"] = bookmark;
            return Json(new { success = true });

        }

        public JsonResult RemoveBookMark(int caid)
        {

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));


            string appuserid = User.Identity.Name;

            UserBookMark temp = _userBookMarkService.FindUserBookMark(appuserid, caid);

            _userBookMarkService.Delete(temp);

            _unitOfWork.Save();
           List<UserBookMarkViewModel> bookmark = (List<UserBookMarkViewModel>)(System.Web.HttpContext.Current.Session["BookMarks"]);

           bookmark.RemoveAll(m => m.MenuId == caid);

            System.Web.HttpContext.Current.Session["BookMarks"] = bookmark;

            return Json(new { success = true });

        }


    }
}