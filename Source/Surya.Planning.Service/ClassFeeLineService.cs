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
using Surya.India.Model.ViewModels;

namespace Surya.India.Service
{
    public interface IClassFeeLineService : IDisposable
    {
        Sch_ClassFeeLine Create(Sch_ClassFeeLine pt);
        void Delete(int id);
        void Delete(Sch_ClassFeeLine pt);
        Sch_ClassFeeLine Find(int id);
        IEnumerable<Sch_ClassFeeLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_ClassFeeLine pt);
        Sch_ClassFeeLine Add(Sch_ClassFeeLine pt);
        IEnumerable<Sch_ClassFeeLine> GetClassFeeLineList();

        IEnumerable<Sch_ClassFeeLine> GetClassFeeLineList(int ClassFeeId);

        IEnumerable<Sch_ClassFeeLineViewModel> GetClassFeeLineListForIndex(int ClassFeeId);

        // IEnumerable<Sch_ClassFeeLine> GetClassFeeLineList(int buyerId);
        Task<IEquatable<Sch_ClassFeeLine>> GetAsync();
        Task<Sch_ClassFeeLine> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class ClassFeeLineService : IClassFeeLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_ClassFeeLine> _ClassFeeLineRepository;
        RepositoryQuery<Sch_ClassFeeLine> ClassFeeLineRepository;
        public ClassFeeLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ClassFeeLineRepository = new Repository<Sch_ClassFeeLine>(db);
            ClassFeeLineRepository = new RepositoryQuery<Sch_ClassFeeLine>(_ClassFeeLineRepository);
        }

        public Sch_ClassFeeLine Find(int id)
        {
            return _unitOfWork.Repository<Sch_ClassFeeLine>().Find(id);
        }

        public Sch_ClassFeeLine Create(Sch_ClassFeeLine pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_ClassFeeLine>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_ClassFeeLine>().Delete(id);
        }

        public void Delete(Sch_ClassFeeLine pt)
        {
            _unitOfWork.Repository<Sch_ClassFeeLine>().Delete(pt);
        }

        public void Update(Sch_ClassFeeLine pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_ClassFeeLine>().Update(pt);
        }

        public IEnumerable<Sch_ClassFeeLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_ClassFeeLine>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_ClassFeeLine> GetClassFeeLineList()
        {
            var pt = _unitOfWork.Repository<Sch_ClassFeeLine>().Query().Get();

            return pt;
        }

        public IEnumerable<Sch_ClassFeeLine> GetClassFeeLineList(int ClassFeeId)
        {
            var pt = _unitOfWork.Repository<Sch_ClassFeeLine>().Query().Get().Where(i => i.ClassFeeId == ClassFeeId);

            return pt;
        }

        public IEnumerable<Sch_ClassFeeLineViewModel> GetClassFeeLineListForIndex(int ClassFeeId)
        {
            var pt = (from C in db.Sch_ClassFeeLine
                      join F in db.Sch_Fee on C.FeeId equals F.FeeId into FeeTable
                      from FeeTab in FeeTable.DefaultIfEmpty()
                      join A in db.LedgerAccount on C.LedgerAccountId equals A.LedgerAccountId into LedgerAccountTable
                      from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()
                      where C.ClassFeeId == ClassFeeId
                      select new Sch_ClassFeeLineViewModel
                      {
                          ClassFeeLineId = C.ClassFeeLineId,
                          FeeId = C.FeeId,
                          FeeName = FeeTab.FeeName,
                          LedgerAccountId = C.LedgerAccountId,
                          LedgerAccountName = LedgerAccountTab.LedgerAccountName,
                          Recurrence = C.Recurrence,
                          Amount = C.Amount
                      });


            return pt.ToList();
        }

        public Sch_ClassFeeLine Add(Sch_ClassFeeLine pt)
        {
            _unitOfWork.Repository<Sch_ClassFeeLine>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_ClassFeeLine
                        select p.ClassFeeLineId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_ClassFeeLine
                        select p.ClassFeeLineId).FirstOrDefault();
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

                temp = (from p in db.Sch_ClassFeeLine
                        select p.ClassFeeLineId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_ClassFeeLine
                        select p.ClassFeeLineId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_ClassFeeLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_ClassFeeLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
