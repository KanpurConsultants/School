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
using Surya.India.Model.ViewModels;
using System.Data.SqlClient;

namespace Surya.India.Service
{
    public interface IFeeRefundLineService : IDisposable
    {
        Sch_FeeRefundLine Create(Sch_FeeRefundLine pt);
        void Delete(int id);
        void Delete(Sch_FeeRefundLine pt);
        Sch_FeeRefundLine Find(int id);

        IEnumerable<Sch_FeeRefundLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_FeeRefundLine pt);
        Sch_FeeRefundLine Add(Sch_FeeRefundLine pt);
        IEnumerable<Sch_FeeRefundLine> GetFeeRefundLineList();





        // IEnumerable<Sch_FeeRefundLine> GetFeeRefundLineList(int buyerId);
        Task<IEquatable<Sch_FeeRefundLine>> GetAsync();
        Task<Sch_FeeRefundLine> FindAsync(int id);

        IEnumerable<Sch_FeeRefundLineViewModel> GetFeeRefundLineListForIndex(int FeeRefundHeaderId);


        Sch_FeeRefundLineViewModel GetFeeRefundLineForEdit(int FeeRefundLineId);

        IEnumerable< Sch_FeeRefundLine> GetFeeRefundLineForHeader(int FeeRefundHeaderId);

        List<Sch_FeeRefundLineViewModel> GetFeeRefundLineList(int StudentId);
    }

    public class FeeRefundLineService : IFeeRefundLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_FeeRefundLine> _FeeRefundLineRepository;
        RepositoryQuery<Sch_FeeRefundLine> FeeRefundLineRepository;
        public FeeRefundLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FeeRefundLineRepository = new Repository<Sch_FeeRefundLine>(db);
            FeeRefundLineRepository = new RepositoryQuery<Sch_FeeRefundLine>(_FeeRefundLineRepository);
        }
     
      
        public Sch_FeeRefundLine Find(int id)
        {
            return _unitOfWork.Repository<Sch_FeeRefundLine>().Find(id);
        }

        public Sch_FeeRefundLine Create(Sch_FeeRefundLine pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FeeRefundLine>().Insert(pt);
            return pt;
        }

        public IEnumerable<Sch_FeeRefundLine> GetFeeRefundLineForHeader(int FeeRefundHeaderId)
        {
            return (from L in db.Sch_FeeRefundLine where L.FeeRefundHeaderId == FeeRefundHeaderId select L).ToList();
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FeeRefundLine>().Delete(id);
        }

        public void Delete(Sch_FeeRefundLine pt)
        {
            _unitOfWork.Repository<Sch_FeeRefundLine>().Delete(pt);
        }

        public void Update(Sch_FeeRefundLine pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FeeRefundLine>().Update(pt);
        }

        public IEnumerable<Sch_FeeRefundLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_FeeRefundLine>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_FeeRefundLine> GetFeeRefundLineList()
        {
            var pt = _unitOfWork.Repository<Sch_FeeRefundLine>().Query().Get();

            return pt;
        }

        public Sch_FeeRefundLine Add(Sch_FeeRefundLine pt)
        {
            _unitOfWork.Repository<Sch_FeeRefundLine>().Insert(pt);
            return pt;
        }

        public IEnumerable<Sch_FeeRefundLineViewModel> GetFeeRefundLineListForIndex(int FeeRefundHeaderId)
        {
            var pt = (from C in db.Sch_FeeRefundLine
                      where C.FeeRefundHeaderId == FeeRefundHeaderId
                      select new Sch_FeeRefundLineViewModel
                      {
                          FeeRefundLineId = C.FeeRefundLineId,
                          FeeName = C.FeeReceiveLine.FeeDueLine.Fee.FeeName,
                          Amount = C.Amount,
                          Remark = C.Remark
                      });


            return pt.ToList();
        }

        public Sch_FeeRefundLineViewModel GetFeeRefundLineForEdit(int FeeRefundLineId)
        {
            var pt = (from C in db.Sch_FeeRefundLine
                      where C.FeeRefundLineId == FeeRefundLineId
                      select new Sch_FeeRefundLineViewModel
                      {
                          FeeRefundLineId = C.FeeRefundLineId,
                          FeeReceiveLineId = C.FeeReceiveLineId,
                          FeeName = C.FeeReceiveLine.FeeDueLine.Fee.FeeName,
                          Amount = C.Amount,
                          Remark = C.Remark
                      }).FirstOrDefault();
            return pt;
        }

        public List<Sch_FeeRefundLineViewModel> GetFeeRefundLineList(int StudentId)
        {
            SqlParameter SQLStudentId = new SqlParameter("@StudentId", StudentId);

            List<Sch_FeeRefundLineViewModel> FeeRefundLineList = db.Database.SqlQuery<Sch_FeeRefundLineViewModel>("Web.GetFeeForReceivePendingForRefund @StudentId", SQLStudentId).ToList();

            return FeeRefundLineList;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_FeeRefundLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_FeeRefundLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
