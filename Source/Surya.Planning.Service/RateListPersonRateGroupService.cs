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

namespace Surya.India.Service
{
    public interface IRateListPersonRateGroupService : IDisposable
    {
        RateListPersonRateGroup Create(RateListPersonRateGroup pt);
        void Delete(int id);
        void Delete(RateListPersonRateGroup pt);
        RateListPersonRateGroup Find(int id);
        void Update(RateListPersonRateGroup pt);
        RateListPersonRateGroup Add(RateListPersonRateGroup pt);
        IQueryable<RateListPersonRateGroup> GetRateListPersonRateGroupList(int Id);
    }

    public class RateListPersonRateGroupService : IRateListPersonRateGroupService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<RateListPersonRateGroup> _RateListPersonRateGroupRepository;
        RepositoryQuery<RateListPersonRateGroup> RateListPersonRateGroupRepository;

        public RateListPersonRateGroupService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _RateListPersonRateGroupRepository = new Repository<RateListPersonRateGroup>(db);
            RateListPersonRateGroupRepository = new RepositoryQuery<RateListPersonRateGroup>(_RateListPersonRateGroupRepository);
        }


        public RateListPersonRateGroup Find(int id)
        {
            return _unitOfWork.Repository<RateListPersonRateGroup>().Find(id);
        }

        public RateListPersonRateGroup Create(RateListPersonRateGroup pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<RateListPersonRateGroup>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<RateListPersonRateGroup>().Delete(id);
        }

        public void Delete(RateListPersonRateGroup pt)
        {
            _unitOfWork.Repository<RateListPersonRateGroup>().Delete(pt);
        }

        public void Update(RateListPersonRateGroup pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<RateListPersonRateGroup>().Update(pt);
        }

        public IQueryable<RateListPersonRateGroup> GetRateListPersonRateGroupList(int Id)
        {
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var pt = _unitOfWork.Repository<RateListPersonRateGroup>().Query().Get().Where(m=>m.RateListHeaderId==Id).OrderBy(m => m.RateListPersonRateGroupId);

            return pt;
        }

        public RateListPersonRateGroup Add(RateListPersonRateGroup pt)
        {
            _unitOfWork.Repository<RateListPersonRateGroup>().Insert(pt);
            return pt;
        }     
        public void Dispose()
        {
        }

    }
}
