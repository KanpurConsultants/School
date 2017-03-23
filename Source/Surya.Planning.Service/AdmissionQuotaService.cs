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
    public interface IAdmissionQuotaService : IDisposable
    {
        Sch_AdmissionQuota Create(Sch_AdmissionQuota pt);
        void Delete(int id);
        void Delete(Sch_AdmissionQuota pt);
        Sch_AdmissionQuota Find(string Name);
        Sch_AdmissionQuota Find(int id);
        IEnumerable<Sch_AdmissionQuota> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_AdmissionQuota pt);
        Sch_AdmissionQuota Add(Sch_AdmissionQuota pt);
        IEnumerable<Sch_AdmissionQuota> GetAdmissionQuotaList();


        // IEnumerable<Sch_AdmissionQuota> GetAdmissionQuotaList(int buyerId);
        Task<IEquatable<Sch_AdmissionQuota>> GetAsync();
        Task<Sch_AdmissionQuota> FindAsync(int id);
        Sch_AdmissionQuota GetAdmissionQuotaByName(string terms);
        int NextId(int id);
        int PrevId(int id);
    }

    public class AdmissionQuotaService : IAdmissionQuotaService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_AdmissionQuota> _AdmissionQuotaRepository;
        RepositoryQuery<Sch_AdmissionQuota> AdmissionQuotaRepository;
        public AdmissionQuotaService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _AdmissionQuotaRepository = new Repository<Sch_AdmissionQuota>(db);
            AdmissionQuotaRepository = new RepositoryQuery<Sch_AdmissionQuota>(_AdmissionQuotaRepository);
        }
        public Sch_AdmissionQuota GetAdmissionQuotaByName(string terms)
        {
            return (from p in db.Sch_AdmissionQuota
                    where p.AdmissionQuotaName == terms
                    select p).FirstOrDefault();
        }

        public Sch_AdmissionQuota Find(string Name)
        {
            return AdmissionQuotaRepository.Get().Where(i => i.AdmissionQuotaName == Name).FirstOrDefault();
        }


        public Sch_AdmissionQuota Find(int id)
        {
            return _unitOfWork.Repository<Sch_AdmissionQuota>().Find(id);
        }

        public Sch_AdmissionQuota Create(Sch_AdmissionQuota pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_AdmissionQuota>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_AdmissionQuota>().Delete(id);
        }

        public void Delete(Sch_AdmissionQuota pt)
        {
            _unitOfWork.Repository<Sch_AdmissionQuota>().Delete(pt);
        }

        public void Update(Sch_AdmissionQuota pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_AdmissionQuota>().Update(pt);
        }

        public IEnumerable<Sch_AdmissionQuota> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_AdmissionQuota>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.AdmissionQuotaName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_AdmissionQuota> GetAdmissionQuotaList()
        {
            var pt = _unitOfWork.Repository<Sch_AdmissionQuota>().Query().Get().OrderBy(m=>m.AdmissionQuotaName);

            return pt;
        }

        public Sch_AdmissionQuota Add(Sch_AdmissionQuota pt)
        {
            _unitOfWork.Repository<Sch_AdmissionQuota>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_AdmissionQuota
                        orderby p.AdmissionQuotaName
                        select p.AdmissionQuotaId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_AdmissionQuota
                        orderby p.AdmissionQuotaName
                        select p.AdmissionQuotaId).FirstOrDefault();
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

                temp = (from p in db.Sch_AdmissionQuota
                        orderby p.AdmissionQuotaName
                        select p.AdmissionQuotaId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_AdmissionQuota
                        orderby p.AdmissionQuotaName
                        select p.AdmissionQuotaId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_AdmissionQuota>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_AdmissionQuota> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
