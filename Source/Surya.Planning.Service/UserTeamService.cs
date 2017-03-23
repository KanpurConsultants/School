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
    public interface IUserTeamService : IDisposable
    {
        UserTeam Create(UserTeam pt);
        void Delete(int id);
        void Delete(UserTeam pt);
        UserTeam Find(int id);
        void Update(UserTeam pt);
        UserTeam Add(UserTeam pt);
        IEnumerable<UserTeam> GetUserTeamList();
        int NextId(int id);
        int PrevId(int id);
        UserTeamViewModel GetUserTeam(int id);
        IEnumerable<UserTeamViewModel> GetUserTeamListVM(int Id, string UserId);
        IQueryable<UserIndexViewModel> GetUserIndex();
        IEnumerable<ProjectIndexViewModel> GetProjectIndex(string UserId);
    }

    public class UserTeamService : IUserTeamService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        public UserTeamService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public UserTeam Find(int id)
        {
            return _unitOfWork.Repository<UserTeam>().Find(id);
        }

        public UserTeam Create(UserTeam pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<UserTeam>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<UserTeam>().Delete(id);
        }

        public void Delete(UserTeam pt)
        {
            _unitOfWork.Repository<UserTeam>().Delete(pt);
        }

        public void Update(UserTeam pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<UserTeam>().Update(pt);
        }

        public IEnumerable<UserTeam> GetUserTeamList()
        {
            var pt = _unitOfWork.Repository<UserTeam>().Query().Get().OrderBy(m => m.User);

            return pt;
        }

        public UserTeam Add(UserTeam pt)
        {
            _unitOfWork.Repository<UserTeam>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.UserTeam
                        orderby p.User
                        select p.UserTeamId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.UserTeam
                        orderby p.User
                        select p.UserTeamId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.UserTeam
                        orderby p.User
                        select p.UserTeamId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.UserTeam
                        orderby p.User
                        select p.UserTeamId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }
        public UserTeamViewModel GetUserTeam(int id)
        {
            return (from p in db.UserTeam
                    where p.UserTeamId == id
                    select new UserTeamViewModel
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.Project.ProjectName,
                        Srl = p.Srl,
                        TeamUser = p.TeamUser,
                        User = p.User,
                        UserTeamId = p.UserTeamId,
                    }).FirstOrDefault();
        }
        public IEnumerable<UserTeamViewModel> GetUserTeamListVM(int Id, string UserId)
        {
            return (from p in db.UserTeam
                    where p.User == UserId && p.ProjectId == Id
                    orderby p.TeamUser
                    select new UserTeamViewModel
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.Project.ProjectName,
                        Srl = p.Srl,
                        TeamUser = p.TeamUser,
                        User = p.User,
                        UserTeamId = p.UserTeamId,
                    }).ToList();
        }
        public IQueryable<UserIndexViewModel> GetUserIndex()
        {
            return (from p in db.Users
                    orderby p.UserName
                    select new UserIndexViewModel
                    {
                        UserId = p.UserName,
                        UserName = p.UserName,
                    });
        }
        public IEnumerable<ProjectIndexViewModel> GetProjectIndex(string UserId)
        {
            return (from p in db.Project
                    orderby p.ProjectName
                    select new ProjectIndexViewModel
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        UserTeamCount = (from t2 in db.UserTeam
                                         where t2.User == UserId && t2.ProjectId == p.ProjectId
                                         select t2).Count(),
                    }).ToList();
        }
        public void Dispose()
        {
        }
    }
}
