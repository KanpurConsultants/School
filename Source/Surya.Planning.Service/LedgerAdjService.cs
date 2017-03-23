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
    public interface ILedgerAdjService : IDisposable
    {
        LedgerAdj Create(LedgerAdj pt);
        void Delete(int id);
        void Delete(LedgerAdj pt);
        LedgerAdj Find(int id);
        void Update(LedgerAdj pt);
        LedgerAdj Add(LedgerAdj pt);

        // IEnumerable<LedgerAdj> GetLedgerAdjList(int buyerId);
        Task<IEquatable<LedgerAdj>> GetAsync();
        Task<LedgerAdj> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
        IEnumerable<LedgerAdj> FindByDrLedgerId(int DrLedgerId);
        IEnumerable<LedgerAdj> FindByCrLedgerId(int CrLedgerId);
    }

    public class LedgerAdjService : ILedgerAdjService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<LedgerAdj> _LedgerAdjRepository;
        RepositoryQuery<LedgerAdj> LedgerAdjRepository;
        public LedgerAdjService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _LedgerAdjRepository = new Repository<LedgerAdj>(db);
            LedgerAdjRepository = new RepositoryQuery<LedgerAdj>(_LedgerAdjRepository);
        }



      

        public LedgerAdj Find(int id)
        {
            return _unitOfWork.Repository<LedgerAdj>().Find(id);            
        }

        public IEnumerable<LedgerAdj> FindByDrLedgerId(int DrLedgerId)
        {
            return _unitOfWork.Repository<LedgerAdj>().Query().Get().Where(i => i.DrLedgerId == DrLedgerId).ToList();
        }

        public IEnumerable<LedgerAdj> FindByCrLedgerId(int CrLedgerId)
        {
            return _unitOfWork.Repository<LedgerAdj>().Query().Get().Where(i => i.CrLedgerId == CrLedgerId).ToList();
        }

        public LedgerAdj Create(LedgerAdj pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<LedgerAdj>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<LedgerAdj>().Delete(id);
        }

        public void Delete(LedgerAdj pt)
        {
            _unitOfWork.Repository<LedgerAdj>().Delete(pt);
        }

        public void Update(LedgerAdj pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<LedgerAdj>().Update(pt);
        }

        public IEnumerable<LedgerAdj> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<LedgerAdj>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public LedgerAdj Add(LedgerAdj pt)
        {
            _unitOfWork.Repository<LedgerAdj>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.LedgerAdj
                        orderby p.LedgerAdjId
                        select p.LedgerAdjId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.LedgerAdj
                        orderby p.LedgerAdjId
                        select p.LedgerAdjId).FirstOrDefault();
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

                temp = (from p in db.LedgerAdj
                        orderby p.LedgerAdjId
                        select p.LedgerAdjId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.LedgerAdj
                        orderby p.LedgerAdjId
                        select p.LedgerAdjId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<LedgerAdj>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LedgerAdj> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
