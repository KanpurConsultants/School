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
    public interface ICostCenterStatusService : IDisposable
    {
        CostCenterStatus Create(CostCenterStatus pt);
        void Delete(int id);
        void Delete(CostCenterStatus pt);
        CostCenterStatus Find(int id);
        IEnumerable<CostCenterStatus> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(CostCenterStatus pt);
        CostCenterStatus Add(CostCenterStatus pt);
        IEnumerable<CostCenterStatus> GetCostCenterStatusList();

        // IEnumerable<CostCenterStatus> GetCostCenterStatusList(int buyerId);
        Task<IEquatable<CostCenterStatus>> GetAsync();
        Task<CostCenterStatus> FindAsync(int id);
        void CreateLineStatus(int id, ref ApplicationDbContext context, bool IsDBbased);
    }

    public class CostCenterStatusService : ICostCenterStatusService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<CostCenterStatus> _CostCenterStatusRepository;
        RepositoryQuery<CostCenterStatus> CostCenterStatusRepository;
        public CostCenterStatusService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _CostCenterStatusRepository = new Repository<CostCenterStatus>(db);
            CostCenterStatusRepository = new RepositoryQuery<CostCenterStatus>(_CostCenterStatusRepository);
        }


        public CostCenterStatus Find(int id)
        {
            return _unitOfWork.Repository<CostCenterStatus>().Find(id);
        }

        public CostCenterStatus Create(CostCenterStatus pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<CostCenterStatus>().Insert(pt);
            return pt;
        }

        public CostCenterStatus DBCreate(CostCenterStatus pt)
        {
            pt.ObjectState = ObjectState.Added;
            db.CostCenterStatus.Add(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<CostCenterStatus>().Delete(id);
        }

        public void Delete(CostCenterStatus pt)
        {
            _unitOfWork.Repository<CostCenterStatus>().Delete(pt);
        }

        public void Update(CostCenterStatus pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<CostCenterStatus>().Update(pt);
        }

        public IEnumerable<CostCenterStatus> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<CostCenterStatus>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<CostCenterStatus> GetCostCenterStatusList()
        {
            var pt = _unitOfWork.Repository<CostCenterStatus>().Query().Get();

            return pt;
        }

        public CostCenterStatus Add(CostCenterStatus pt)
        {
            _unitOfWork.Repository<CostCenterStatus>().Insert(pt);
            return pt;
        }

        public void CreateLineStatus(int id, ref ApplicationDbContext context, bool IsDBbased)
        {
            CostCenterStatus Stat = new CostCenterStatus();
            Stat.CostCenterId = id;
            Stat.ObjectState = Model.ObjectState.Added;
            if (IsDBbased)
                context.CostCenterStatus.Add(Stat);
            else
                Add(Stat);
        }

        public void CreateLineStatusExtended(int id)
        {
            CostCenterStatusExtended Stat = new CostCenterStatusExtended();
            Stat.CostCenterId = id;
            Stat.ObjectState = Model.ObjectState.Added;

            _unitOfWork.Repository<CostCenterStatusExtended>().Insert(Stat);
        }

        public void DeleteLineStatus(int id)
        {
            CostCenterStatus Stat = Find(id);
            Delete(Stat);
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<CostCenterStatus>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CostCenterStatus> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
