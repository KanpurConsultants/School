using System.Collections.Generic;
using System.Linq;
using Surya.India.Data;
using Surya.India.Data.Infrastructure;
using Surya.India.Model.Models;
using Surya.India.Model.ViewModels;
using Surya.India.Core.Common;
using System;
using Surya.India.Model;
using System.Threading.Tasks;
using Surya.India.Data.Models;
using Surya.India.Model.ViewModel;

namespace Surya.India.Service
{
    public interface IRolesSiteService : IDisposable
    {
        RolesSite Create(RolesSite pt);
        void Delete(int id);
        void Delete(RolesSite pt);
        RolesSite Find(int ptId);
        void Update(RolesSite pt);
        RolesSite Add(RolesSite pt);
        IEnumerable<RolesSite> GetRolesSiteList();
        RolesSite Find(int SiteId, string RoleId);
        IEnumerable<RolesSiteViewModel> GetRolesSiteList(string RoleId);
    }

    public class RolesSiteService : IRolesSiteService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<RolesSite> _RolesSiteRepository;
        RepositoryQuery<RolesSite> RolesSiteRepository;
        public RolesSiteService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _RolesSiteRepository = new Repository<RolesSite>(db);
            RolesSiteRepository = new RepositoryQuery<RolesSite>(_RolesSiteRepository);
        }

        public RolesSite Find(int pt)
        {
            return _unitOfWork.Repository<RolesSite>().Find(pt);
        }

        public RolesSite Create(RolesSite pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<RolesSite>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<RolesSite>().Delete(id);
        }

        public void Delete(RolesSite pt)
        {
            _unitOfWork.Repository<RolesSite>().Delete(pt);
        }

        public void Update(RolesSite pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<RolesSite>().Update(pt);
        }

        public IEnumerable<RolesSite> GetRolesSiteList()
        {
            var pt = _unitOfWork.Repository<RolesSite>().Query().Get();

            return pt;
        }


        public RolesSite Add(RolesSite pt)
        {
            _unitOfWork.Repository<RolesSite>().Insert(pt);
            return pt;
        }

        public RolesSite Find(int SiteId, string RoleId)
        {
            return _unitOfWork.Repository<RolesSite>().Query().Get().Where(m=>m.RoleId==RoleId && m.SiteId==SiteId).FirstOrDefault();
        }

        public IEnumerable<RolesSiteViewModel> GetRolesSiteList(string RoleId)
        {

            return (from p in db.RolesSite
                    where p.RoleId == RoleId
                    select new RolesSiteViewModel
                    {
                        RoleId = p.RoleId,
                        RoleName = p.Role.Name,
                        RolesSiteId = p.RolesSiteId,
                        SiteId = p.SiteId,
                        SiteName = p.Site.SiteName,
                    }
                        );

        }
        public void Dispose()
        {
        }
    }
}
