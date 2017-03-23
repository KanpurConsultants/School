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
    public interface IFacilityService : IDisposable
    {
        Sch_Facility Create(Sch_Facility pt);
        void Delete(int id);
        void Delete(Sch_Facility pt);
        Sch_Facility Find(string Name);
        Sch_Facility Find(int id);
        IEnumerable<Sch_Facility> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_Facility pt);
        Sch_Facility Add(Sch_Facility pt);
        IEnumerable<Sch_Facility> GetFacilityList();

        // IEnumerable<Sch_Facility> GetFacilityList(int buyerId);
        Task<IEquatable<Sch_Facility>> GetAsync();
        Task<Sch_Facility> FindAsync(int id);
        Sch_Facility GetFacilityByName(string terms);
        int NextId(int id);
        int PrevId(int id);
    }

    public class FacilityService : IFacilityService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_Facility> _FacilityRepository;
        RepositoryQuery<Sch_Facility> FacilityRepository;
        public FacilityService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FacilityRepository = new Repository<Sch_Facility>(db);
            FacilityRepository = new RepositoryQuery<Sch_Facility>(_FacilityRepository);
        }
        public Sch_Facility GetFacilityByName(string terms)
        {
            return (from p in db.Sch_Facility
                    where p.FacilityName == terms
                    select p).FirstOrDefault();
        }

        public Sch_Facility Find(string Name)
        {
            return FacilityRepository.Get().Where(i => i.FacilityName == Name).FirstOrDefault();
        }


        public Sch_Facility Find(int id)
        {
            return _unitOfWork.Repository<Sch_Facility>().Find(id);
        }

        public Sch_Facility Create(Sch_Facility pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_Facility>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_Facility>().Delete(id);
        }

        public void Delete(Sch_Facility pt)
        {
            _unitOfWork.Repository<Sch_Facility>().Delete(pt);
        }

        public void Update(Sch_Facility pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_Facility>().Update(pt);
        }

        public IEnumerable<Sch_Facility> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_Facility>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.FacilityName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_Facility> GetFacilityList()
        {
            var pt = _unitOfWork.Repository<Sch_Facility>().Query().Get().OrderBy(m=>m.FacilityName);

            return pt;
        }

        public Sch_Facility Add(Sch_Facility pt)
        {
            _unitOfWork.Repository<Sch_Facility>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_Facility
                        orderby p.FacilityName
                        select p.FacilityId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_Facility
                        orderby p.FacilityName
                        select p.FacilityId).FirstOrDefault();
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

                temp = (from p in db.Sch_Facility
                        orderby p.FacilityName
                        select p.FacilityId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_Facility
                        orderby p.FacilityName
                        select p.FacilityId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_Facility>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_Facility> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
