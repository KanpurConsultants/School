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
    public interface IFeeDueHeaderService : IDisposable
    {
        Sch_FeeDueHeader Create(Sch_FeeDueHeader pt);
        void Delete(int id);
        void Delete(Sch_FeeDueHeader pt);
        Sch_FeeDueHeader Find(int id);
        IEnumerable<Sch_FeeDueHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_FeeDueHeader pt);
        Sch_FeeDueHeader Add(Sch_FeeDueHeader pt);

        IEnumerable<Sch_FeeDueHeader> GetFeeDueHeaderList();

        IEnumerable<Sch_FeeDueHeaderViewModel> GetFeeDueHeaderListForIndex();

        Sch_FeeDueHeaderViewModel GetFeeDueHeaderForEdit(int FeeDueHeaderId);

        Task<IEquatable<Sch_FeeDueHeader>> GetAsync();
        Task<Sch_FeeDueHeader> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class FeeDueHeaderService : IFeeDueHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_FeeDueHeader> _FeeDueHeaderRepository;
        RepositoryQuery<Sch_FeeDueHeader> FeeDueHeaderRepository;
        public FeeDueHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FeeDueHeaderRepository = new Repository<Sch_FeeDueHeader>(db);
            FeeDueHeaderRepository = new RepositoryQuery<Sch_FeeDueHeader>(_FeeDueHeaderRepository);
        }
       
      

        public Sch_FeeDueHeader Find(int id)
        {
            return _unitOfWork.Repository<Sch_FeeDueHeader>().Find(id);
        }

        public IEnumerable<Sch_FeeDueHeader> GetFeeDueHeaderList()
        {
            return _unitOfWork.Repository<Sch_FeeDueHeader>().Query().Get();
        }

        public IEnumerable<Sch_FeeDueHeaderViewModel> GetFeeDueHeaderListForIndex()
        {
            var temp = (from H in db.Sch_FeeDueHeader
                        select new Sch_FeeDueHeaderViewModel
                        {
                            FeeDueHeaderId = H.FeeDueHeaderId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            FromDate = H.FromDate,
                            ToDate = H.ToDate,
                        });

            return temp.ToList();
        }


        public Sch_FeeDueHeaderViewModel GetFeeDueHeaderForEdit(int FeeDueHeaderId)
        {
            var temp = (from H in db.Sch_FeeDueHeader
                        where H.FeeDueHeaderId == FeeDueHeaderId
                        select new Sch_FeeDueHeaderViewModel
                        {
                            FeeDueHeaderId = H.FeeDueHeaderId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            DocTypeId = H.DocTypeId,
                            LastDate = H.LastDate,
                            FromDate = H.FromDate,
                            ToDate = H.ToDate,
                            ProgramId = H.ProgramId,
                            ClassId = H.ClassId,
                            StreamId = H.StreamId,
                            CreatedBy = H.CreatedBy,
                            ModifiedBy = H.ModifiedBy,
                            CreatedDate = H.CreatedDate,
                            ModifiedDate = H.ModifiedDate
                        });

            return temp.FirstOrDefault();
        }


        public Sch_FeeDueHeader Create(Sch_FeeDueHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FeeDueHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FeeDueHeader>().Delete(id);
        }

        public void Delete(Sch_FeeDueHeader pt)
        {
            _unitOfWork.Repository<Sch_FeeDueHeader>().Delete(pt);
        }

        public void Update(Sch_FeeDueHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FeeDueHeader>().Update(pt);
        }

        public IEnumerable<Sch_FeeDueHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_FeeDueHeader>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public Sch_FeeDueHeader Add(Sch_FeeDueHeader pt)
        {
            _unitOfWork.Repository<Sch_FeeDueHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_FeeDueHeader
                        select p.FeeDueHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FeeDueHeader
                        select p.FeeDueHeaderId).FirstOrDefault();
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

                temp = (from p in db.Sch_FeeDueHeader
                        select p.FeeDueHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FeeDueHeader
                        select p.FeeDueHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


      

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_FeeDueHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_FeeDueHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
