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
    public interface IFacilitySubCategoryService : IDisposable
    {
        Sch_FacilitySubCategory Create(Sch_FacilitySubCategory pt);
        void Delete(int id);
        void Delete(Sch_FacilitySubCategory pt);
        Sch_FacilitySubCategory Find(string Name);
        Sch_FacilitySubCategory Find(int id);
        IEnumerable<Sch_FacilitySubCategory> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_FacilitySubCategory pt);
        Sch_FacilitySubCategory Add(Sch_FacilitySubCategory pt);
        IEnumerable<Sch_FacilitySubCategory> GetFacilitySubCategoryList();

        IEnumerable<Sch_FacilitySubCategory> GetFacilitySubCategoryList(int ProgramId);

        // IEnumerable<Sch_FacilitySubCategory> GetFacilitySubCategoryList(int buyerId);
        Task<IEquatable<Sch_FacilitySubCategory>> GetAsync();
        Task<Sch_FacilitySubCategory> FindAsync(int id);
        Sch_FacilitySubCategory GetFacilitySubCategoryByName(string terms);
        int NextId(int id);
        int PrevId(int id);
    }

    public class FacilitySubCategoryService : IFacilitySubCategoryService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_FacilitySubCategory> _FacilitySubCategoryRepository;
        RepositoryQuery<Sch_FacilitySubCategory> FacilitySubCategoryRepository;
        public FacilitySubCategoryService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FacilitySubCategoryRepository = new Repository<Sch_FacilitySubCategory>(db);
            FacilitySubCategoryRepository = new RepositoryQuery<Sch_FacilitySubCategory>(_FacilitySubCategoryRepository);
        }
        public Sch_FacilitySubCategory GetFacilitySubCategoryByName(string terms)
        {
            return (from p in db.Sch_FacilitySubCategory
                    where p.FacilitySubCategoryName == terms
                    select p).FirstOrDefault();
        }

        public Sch_FacilitySubCategory Find(string Name)
        {
            return FacilitySubCategoryRepository.Get().Where(i => i.FacilitySubCategoryName == Name).FirstOrDefault();
        }


        public Sch_FacilitySubCategory Find(int id)
        {
            return _unitOfWork.Repository<Sch_FacilitySubCategory>().Find(id);
        }

        public Sch_FacilitySubCategory Create(Sch_FacilitySubCategory pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FacilitySubCategory>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FacilitySubCategory>().Delete(id);
        }

        public void Delete(Sch_FacilitySubCategory pt)
        {
            _unitOfWork.Repository<Sch_FacilitySubCategory>().Delete(pt);
        }

        public void Update(Sch_FacilitySubCategory pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FacilitySubCategory>().Update(pt);
        }

        public IEnumerable<Sch_FacilitySubCategory> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_FacilitySubCategory>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.FacilitySubCategoryName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_FacilitySubCategory> GetFacilitySubCategoryList()
        {
            var pt = _unitOfWork.Repository<Sch_FacilitySubCategory>().Query().Get().OrderBy(m=>m.FacilitySubCategoryName);

            return pt;
        }

        public IEnumerable<Sch_FacilitySubCategory> GetFacilitySubCategoryList(int FacilityId)
        {
            var pt = _unitOfWork.Repository<Sch_FacilitySubCategory>().Query().Get().Where(i => i.FacilityId == FacilityId).OrderBy(m => m.FacilitySubCategoryName);

            return pt;
        }

        public Sch_FacilitySubCategory Add(Sch_FacilitySubCategory pt)
        {
            _unitOfWork.Repository<Sch_FacilitySubCategory>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_FacilitySubCategory
                        orderby p.FacilitySubCategoryName
                        select p.FacilitySubCategoryId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FacilitySubCategory
                        orderby p.FacilitySubCategoryName
                        select p.FacilitySubCategoryId).FirstOrDefault();
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

                temp = (from p in db.Sch_FacilitySubCategory
                        orderby p.FacilitySubCategoryName
                        select p.FacilitySubCategoryId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FacilitySubCategory
                        orderby p.FacilitySubCategoryName
                        select p.FacilitySubCategoryId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_FacilitySubCategory>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_FacilitySubCategory> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
