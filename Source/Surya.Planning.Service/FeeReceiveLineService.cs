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
    public interface IFeeReceiveLineService : IDisposable
    {
        Sch_FeeReceiveLine Create(Sch_FeeReceiveLine pt);
        void Delete(int id);
        void Delete(Sch_FeeReceiveLine pt);
        Sch_FeeReceiveLine Find(int id);

        IEnumerable<Sch_FeeReceiveLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_FeeReceiveLine pt);
        Sch_FeeReceiveLine Add(Sch_FeeReceiveLine pt);
        IEnumerable<Sch_FeeReceiveLine> GetFeeReceiveLineList();





        // IEnumerable<Sch_FeeReceiveLine> GetFeeReceiveLineList(int buyerId);
        Task<IEquatable<Sch_FeeReceiveLine>> GetAsync();
        Task<Sch_FeeReceiveLine> FindAsync(int id);

        IEnumerable<Sch_FeeReceiveLineViewModel> GetFeeReceiveLineListForIndex(int FeeReceiveHeaderId);


        Sch_FeeReceiveLineViewModel GetFeeReceiveLineForEdit(int FeeReceiveLineId);

        IEnumerable< Sch_FeeReceiveLine> GetFeeReceiveLineForHeader(int FeeReceiveHeaderId);

        List<Sch_FeeReceiveLineViewModel> GetFeeReceiveLineList(int StudentId);
    }

    public class FeeReceiveLineService : IFeeReceiveLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_FeeReceiveLine> _FeeReceiveLineRepository;
        RepositoryQuery<Sch_FeeReceiveLine> FeeReceiveLineRepository;
        public FeeReceiveLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FeeReceiveLineRepository = new Repository<Sch_FeeReceiveLine>(db);
            FeeReceiveLineRepository = new RepositoryQuery<Sch_FeeReceiveLine>(_FeeReceiveLineRepository);
        }
     
      
        public Sch_FeeReceiveLine Find(int id)
        {
            return _unitOfWork.Repository<Sch_FeeReceiveLine>().Find(id);
        }

        public Sch_FeeReceiveLine Create(Sch_FeeReceiveLine pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FeeReceiveLine>().Insert(pt);
            return pt;
        }

        public IEnumerable<Sch_FeeReceiveLine> GetFeeReceiveLineForHeader(int FeeReceiveHeaderId)
        {
            return (from L in db.Sch_FeeReceiveLine where L.FeeReceiveHeaderId == FeeReceiveHeaderId select L).ToList();
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FeeReceiveLine>().Delete(id);
        }

        public void Delete(Sch_FeeReceiveLine pt)
        {
            _unitOfWork.Repository<Sch_FeeReceiveLine>().Delete(pt);
        }

        public void Update(Sch_FeeReceiveLine pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FeeReceiveLine>().Update(pt);
        }

        public IEnumerable<Sch_FeeReceiveLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_FeeReceiveLine>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_FeeReceiveLine> GetFeeReceiveLineList()
        {
            var pt = _unitOfWork.Repository<Sch_FeeReceiveLine>().Query().Get();

            return pt;
        }

        public Sch_FeeReceiveLine Add(Sch_FeeReceiveLine pt)
        {
            _unitOfWork.Repository<Sch_FeeReceiveLine>().Insert(pt);
            return pt;
        }

        public IEnumerable<Sch_FeeReceiveLineViewModel> GetFeeReceiveLineListForIndex(int FeeReceiveHeaderId)
        {
            var pt = (from C in db.Sch_FeeReceiveLine
                      where C.FeeReceiveHeaderId == FeeReceiveHeaderId
                      select new Sch_FeeReceiveLineViewModel
                      {
                          FeeReceiveLineId = C.FeeReceiveLineId,
                          FeeName = C.FeeDueLine.Fee.FeeName,
                          Amount = C.Amount,
                          Remark = C.Remark
                      });


            return pt.ToList();
        }

        public Sch_FeeReceiveLineViewModel GetFeeReceiveLineForEdit(int FeeReceiveLineId)
        {
            var pt = (from C in db.Sch_FeeReceiveLine
                      where C.FeeReceiveLineId == FeeReceiveLineId
                      select new Sch_FeeReceiveLineViewModel
                      {
                          FeeReceiveLineId = C.FeeReceiveLineId,
                          FeeDueLineId = C.FeeDueLineId,
                          FeeName = C.FeeDueLine.Fee.FeeName,
                          Amount = C.Amount,
                          Remark = C.Remark
                      }).FirstOrDefault();
            return pt;
        }

        public List<Sch_FeeReceiveLineViewModel> GetFeeReceiveLineList(int StudentId)
        {
            SqlParameter SQLStudentId = new SqlParameter("@StudentId", StudentId);

            List<Sch_FeeReceiveLineViewModel> FeeReceiveLineList = db.Database.SqlQuery<Sch_FeeReceiveLineViewModel>("Web.GetFeeForDuePendingForReceive @StudentId", SQLStudentId).ToList();

            return FeeReceiveLineList;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_FeeReceiveLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_FeeReceiveLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
