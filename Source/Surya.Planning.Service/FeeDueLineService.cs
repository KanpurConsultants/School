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
    public interface IFeeDueLineService : IDisposable
    {
        Sch_FeeDueLine Create(Sch_FeeDueLine pt);
        void Delete(int id);
        void Delete(Sch_FeeDueLine pt);
        Sch_FeeDueLine Find(int id);

        IEnumerable<Sch_FeeDueLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_FeeDueLine pt);
        Sch_FeeDueLine Add(Sch_FeeDueLine pt);
        IEnumerable<Sch_FeeDueLine> GetFeeDueLineList();





        // IEnumerable<Sch_FeeDueLine> GetFeeDueLineList(int buyerId);
        Task<IEquatable<Sch_FeeDueLine>> GetAsync();
        Task<Sch_FeeDueLine> FindAsync(int id);

        IEnumerable<Sch_FeeDueLineViewModel> GetFeeDueLineListForIndex(int FeeDueHeaderId);


        Sch_FeeDueLineViewModel GetFeeDueLineForEdit(int FeeDueLineId);

        IEnumerable< Sch_FeeDueLine> GetFeeDueLineForHeader(int FeeDueHeaderId);

        List<Sch_FeeDueLineViewModel> GetFeeDueLineList(int ProgramId, int ClassId, int StreamId, DateTime FromDate, DateTime ToDate);
    }

    public class FeeDueLineService : IFeeDueLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_FeeDueLine> _FeeDueLineRepository;
        RepositoryQuery<Sch_FeeDueLine> FeeDueLineRepository;
        public FeeDueLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FeeDueLineRepository = new Repository<Sch_FeeDueLine>(db);
            FeeDueLineRepository = new RepositoryQuery<Sch_FeeDueLine>(_FeeDueLineRepository);
        }
     
      
        public Sch_FeeDueLine Find(int id)
        {
            return _unitOfWork.Repository<Sch_FeeDueLine>().Find(id);
        }

        public Sch_FeeDueLine Create(Sch_FeeDueLine pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FeeDueLine>().Insert(pt);
            return pt;
        }

        public IEnumerable<Sch_FeeDueLine> GetFeeDueLineForHeader(int FeeDueHeaderId)
        {
            return (from L in db.Sch_FeeDueLine where L.FeeDueHeaderId == FeeDueHeaderId select L).ToList();
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FeeDueLine>().Delete(id);
        }

        public void Delete(Sch_FeeDueLine pt)
        {
            _unitOfWork.Repository<Sch_FeeDueLine>().Delete(pt);
        }

        public void Update(Sch_FeeDueLine pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FeeDueLine>().Update(pt);
        }

        public IEnumerable<Sch_FeeDueLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_FeeDueLine>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_FeeDueLine> GetFeeDueLineList()
        {
            var pt = _unitOfWork.Repository<Sch_FeeDueLine>().Query().Get();

            return pt;
        }

        public Sch_FeeDueLine Add(Sch_FeeDueLine pt)
        {
            _unitOfWork.Repository<Sch_FeeDueLine>().Insert(pt);
            return pt;
        }

        public IEnumerable<Sch_FeeDueLineViewModel> GetFeeDueLineListForIndex(int FeeDueHeaderId)
        {
            var pt = (from C in db.Sch_FeeDueLine
                      where C.FeeDueHeaderId == FeeDueHeaderId
                      select new Sch_FeeDueLineViewModel
                      {
                          FeeDueLineId = C.FeeDueLineId,
                          StudentName = C.Admission.Student.Person.Name,
                          FeeName = C.Fee.FeeName,
                          Recurrence = C.Recurrence,
                          Amount = C.Amount,
                          Remark = C.Remark
                      });


            return pt.ToList();
        }

        public Sch_FeeDueLineViewModel GetFeeDueLineForEdit(int FeeDueLineId)
        {
            var pt = (from C in db.Sch_FeeDueLine
                      where C.FeeDueLineId == FeeDueLineId
                      select new Sch_FeeDueLineViewModel
                      {
                          FeeDueLineId = C.FeeDueLineId,
                          AdmissionId = C.AdmissionId,
                          FeeId = C.FeeId,
                          Recurrence = C.Recurrence,
                          Amount = C.Amount,
                          Remark = C.Remark
                      }).FirstOrDefault();
            return pt;
        }

        public List<Sch_FeeDueLineViewModel> GetFeeDueLineList(int ProgramId, int ClassId, int StreamId, DateTime FromDate, DateTime ToDate)
        {
            SqlParameter SQLProgramId = new SqlParameter("@ProgramId", ProgramId);
            SqlParameter SQLClassId = new SqlParameter("@ClassId", ClassId);
            SqlParameter SQLStreamId = new SqlParameter("@StreamId", StreamId);
            SqlParameter SQLFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SQLToDate = new SqlParameter("@ToDate", ToDate);

            List<Sch_FeeDueLineViewModel> FeeDueLineList = db.Database.SqlQuery<Sch_FeeDueLineViewModel>("Web.GetFeeForDue @ProgramId, @ClassId, @StreamId,@FromDate,@ToDate", SQLProgramId, SQLClassId, SQLStreamId, SQLFromDate, SQLToDate).ToList();

            return FeeDueLineList;


        }

     

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_FeeDueLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_FeeDueLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
