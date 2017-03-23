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
    public interface IPromotionHeaderService : IDisposable
    {
        Sch_PromotionHeader Create(Sch_PromotionHeader pt);
        void Delete(int id);
        void Delete(Sch_PromotionHeader pt);
        Sch_PromotionHeader Find(int id);
        IEnumerable<Sch_PromotionHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_PromotionHeader pt);
        Sch_PromotionHeader Add(Sch_PromotionHeader pt);

        IEnumerable<Sch_PromotionHeader> GetPromotionHeaderList();

        IEnumerable<Sch_PromotionHeaderViewModel> GetPromotionHeaderListForIndex();

        Sch_PromotionHeaderViewModel GetPromotionHeaderForEdit(int PromotionHeaderId);

        Task<IEquatable<Sch_PromotionHeader>> GetAsync();
        Task<Sch_PromotionHeader> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class PromotionHeaderService : IPromotionHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_PromotionHeader> _PromotionHeaderRepository;
        RepositoryQuery<Sch_PromotionHeader> PromotionHeaderRepository;
        public PromotionHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _PromotionHeaderRepository = new Repository<Sch_PromotionHeader>(db);
            PromotionHeaderRepository = new RepositoryQuery<Sch_PromotionHeader>(_PromotionHeaderRepository);
        }
       
      

        public Sch_PromotionHeader Find(int id)
        {
            return _unitOfWork.Repository<Sch_PromotionHeader>().Find(id);
        }

        public IEnumerable<Sch_PromotionHeader> GetPromotionHeaderList()
        {
            return _unitOfWork.Repository<Sch_PromotionHeader>().Query().Get();
        }

        public IEnumerable<Sch_PromotionHeaderViewModel> GetPromotionHeaderListForIndex()
        {
            var temp = (from H in db.Sch_PromotionHeader
                        select new Sch_PromotionHeaderViewModel
                        {
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                        });

            return temp.ToList();
        }


        public Sch_PromotionHeaderViewModel GetPromotionHeaderForEdit(int PromotionHeaderId)
        {
            var temp = (from H in db.Sch_PromotionHeader
                        where H.PromotionHeaderId == PromotionHeaderId
                        select new Sch_PromotionHeaderViewModel
                        {
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                        });

            return temp.FirstOrDefault();
        }


        public Sch_PromotionHeader Create(Sch_PromotionHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_PromotionHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_PromotionHeader>().Delete(id);
        }

        public void Delete(Sch_PromotionHeader pt)
        {
            _unitOfWork.Repository<Sch_PromotionHeader>().Delete(pt);
        }

        public void Update(Sch_PromotionHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_PromotionHeader>().Update(pt);
        }

        public IEnumerable<Sch_PromotionHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_PromotionHeader>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public Sch_PromotionHeader Add(Sch_PromotionHeader pt)
        {
            _unitOfWork.Repository<Sch_PromotionHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_PromotionHeader
                        select p.PromotionHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_PromotionHeader
                        select p.PromotionHeaderId).FirstOrDefault();
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

                temp = (from p in db.Sch_PromotionHeader
                        select p.PromotionHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_PromotionHeader
                        select p.PromotionHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


      

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_PromotionHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_PromotionHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
