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
    public interface IFeeService : IDisposable
    {
        Sch_Fee Create(Sch_Fee pt);
        void Delete(int id);
        void Delete(Sch_Fee pt);
        Sch_Fee Find(string Name);
        Sch_Fee Find(int id);
        IEnumerable<Sch_Fee> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_Fee pt);
        Sch_Fee Add(Sch_Fee pt);
        IEnumerable<Sch_Fee> GetFeeList();


        // IEnumerable<Sch_Fee> GetFeeList(int buyerId);
        Task<IEquatable<Sch_Fee>> GetAsync();
        Task<Sch_Fee> FindAsync(int id);
        Sch_Fee GetFeeByName(string terms);
        int NextId(int id);
        int PrevId(int id);
    }

    public class FeeService : IFeeService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_Fee> _FeeRepository;
        RepositoryQuery<Sch_Fee> FeeRepository;
        public FeeService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FeeRepository = new Repository<Sch_Fee>(db);
            FeeRepository = new RepositoryQuery<Sch_Fee>(_FeeRepository);
        }
        public Sch_Fee GetFeeByName(string terms)
        {
            return (from p in db.Sch_Fee
                    where p.FeeName == terms
                    select p).FirstOrDefault();
        }

        public Sch_Fee Find(string Name)
        {
            return FeeRepository.Get().Where(i => i.FeeName == Name).FirstOrDefault();
        }


        public Sch_Fee Find(int id)
        {
            return _unitOfWork.Repository<Sch_Fee>().Find(id);
        }

        public Sch_Fee Create(Sch_Fee pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_Fee>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_Fee>().Delete(id);
        }

        public void Delete(Sch_Fee pt)
        {
            _unitOfWork.Repository<Sch_Fee>().Delete(pt);
        }

        public void Update(Sch_Fee pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_Fee>().Update(pt);
        }

        public IEnumerable<Sch_Fee> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_Fee>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.FeeName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_Fee> GetFeeList()
        {
            var pt = _unitOfWork.Repository<Sch_Fee>().Query().Get().OrderBy(m=>m.FeeName);

            return pt;
        }

        public Sch_Fee Add(Sch_Fee pt)
        {
            _unitOfWork.Repository<Sch_Fee>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_Fee
                        orderby p.FeeName
                        select p.FeeId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_Fee
                        orderby p.FeeName
                        select p.FeeId).FirstOrDefault();
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

                temp = (from p in db.Sch_Fee
                        orderby p.FeeName
                        select p.FeeId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_Fee
                        orderby p.FeeName
                        select p.FeeId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_Fee>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_Fee> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
