using System.Collections.Generic;
using System.Linq;
using Surya.India.Data;
using Surya.India.Data.Infrastructure;
using Surya.India.Model.Models;

using Surya.India.Core.Common;
using System;
using Surya.India.Model;
using System.Threading.Tasks;
using Surya.India.Data.Models;
using Surya.India.Model.ViewModel;

namespace Surya.India.Service
{
    public interface IUserBookMarkService : IDisposable
    {
        UserBookMark Create(UserBookMark pt);
        void Delete(int id);
        void Delete(UserBookMark pt);
        UserBookMark Find(int id);      
        void Update(UserBookMark pt);
        UserBookMark Add(UserBookMark pt);
        IEnumerable<UserBookMark> GetUserBookMarkList();
        UserBookMark FindUserBookMark(string userid, int menuid);
        IEnumerable<UserBookMarkViewModel> GetUserBookMarkListForUser(string AppuserId);
    }

    public class UserBookMarkService : IUserBookMarkService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<UserBookMark> _UserBookMarkRepository;
        RepositoryQuery<UserBookMark> UserBookMarkRepository;
        public UserBookMarkService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _UserBookMarkRepository = new Repository<UserBookMark>(db);
            UserBookMarkRepository = new RepositoryQuery<UserBookMark>(_UserBookMarkRepository);
        }


        public UserBookMark Find(int id)
        {
            return _unitOfWork.Repository<UserBookMark>().Find(id);
        }
        public UserBookMark FindUserBookMark(string userid, int menuid)
        {
            return (from p in db.UserBookMark
                    where p.MenuId == menuid && p.ApplicationUserName == userid
                    select p
                        ).FirstOrDefault();

        }


        public IEnumerable<UserBookMarkViewModel> GetUserBookMarkListForUser(string AppuserId)
        {
            return (from p in db.UserBookMark
                    join t in db.Menu on p.MenuId equals t.MenuId into table from tab in table.DefaultIfEmpty()
                    where p.ApplicationUserName == AppuserId
                    select new UserBookMarkViewModel
                    {
                        MenuId=p.MenuId,
                        MenuName=tab.MenuName,
                        IconName=tab.IconName,
                    }
                        ).ToList();

        }

        public UserBookMark Create(UserBookMark pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<UserBookMark>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<UserBookMark>().Delete(id);
        }

        public void Delete(UserBookMark pt)
        {
            _unitOfWork.Repository<UserBookMark>().Delete(pt);
        }

        public void Update(UserBookMark pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<UserBookMark>().Update(pt);
        }

        public IEnumerable<UserBookMark> GetUserBookMarkList()
        {
            var pt = (from p in db.UserBookMark                      
                      select p
                          );

            return pt;
        }

        public UserBookMark Add(UserBookMark pt)
        {
            _unitOfWork.Repository<UserBookMark>().Insert(pt);
            return pt;
        }

        public void Dispose()
        {
        }

    }
}
