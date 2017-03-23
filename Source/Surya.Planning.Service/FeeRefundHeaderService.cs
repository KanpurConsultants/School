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
    public interface IFeeRefundHeaderService : IDisposable
    {
        Sch_FeeRefundHeader Create(Sch_FeeRefundHeader pt);
        void Delete(int id);
        void Delete(Sch_FeeRefundHeader pt);
        Sch_FeeRefundHeader Find(int id);
        IEnumerable<Sch_FeeRefundHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_FeeRefundHeader pt);
        Sch_FeeRefundHeader Add(Sch_FeeRefundHeader pt);

        IEnumerable<Sch_FeeRefundHeader> GetFeeRefundHeaderList();

        IEnumerable<Sch_FeeRefundHeaderViewModel> GetFeeRefundHeaderListForIndex();

        Sch_FeeRefundHeaderViewModel GetFeeRefundHeaderForEdit(int FeeRefundHeaderId);

        Task<IEquatable<Sch_FeeRefundHeader>> GetAsync();
        Task<Sch_FeeRefundHeader> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class FeeRefundHeaderService : IFeeRefundHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_FeeRefundHeader> _FeeRefundHeaderRepository;
        RepositoryQuery<Sch_FeeRefundHeader> FeeRefundHeaderRepository;
        public FeeRefundHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FeeRefundHeaderRepository = new Repository<Sch_FeeRefundHeader>(db);
            FeeRefundHeaderRepository = new RepositoryQuery<Sch_FeeRefundHeader>(_FeeRefundHeaderRepository);
        }
       
      

        public Sch_FeeRefundHeader Find(int id)
        {
            return _unitOfWork.Repository<Sch_FeeRefundHeader>().Find(id);
        }

        public IEnumerable<Sch_FeeRefundHeader> GetFeeRefundHeaderList()
        {
            return _unitOfWork.Repository<Sch_FeeRefundHeader>().Query().Get();
        }

        public IEnumerable<Sch_FeeRefundHeaderViewModel> GetFeeRefundHeaderListForIndex()
        {
            var temp = (from H in db.Sch_FeeRefundHeader
                        select new Sch_FeeRefundHeaderViewModel
                        {
                            FeeRefundHeaderId = H.FeeRefundHeaderId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            StudentName = H.Student.Person.Name
                        });

            return temp.ToList();
        }


        public Sch_FeeRefundHeaderViewModel GetFeeRefundHeaderForEdit(int FeeRefundHeaderId)
        {
            var temp = (from H in db.Sch_FeeRefundHeader
                        join S in db.ViewStudentCurrentAdmission on H.StudentId equals S.PersonID into StudentTable from StudentTab in StudentTable.DefaultIfEmpty()
                        join A in db.Sch_Admission on StudentTab.AdmissionId equals A.AdmissionId into AdmissionTable from AdmissionTab in AdmissionTable.DefaultIfEmpty()
                        where H.FeeRefundHeaderId == FeeRefundHeaderId
                        select new Sch_FeeRefundHeaderViewModel
                        {
                            FeeRefundHeaderId = H.FeeRefundHeaderId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            DocTypeId = H.DocTypeId,
                            StudentId = H.StudentId,
                            ProgramName = AdmissionTab.ClassSection.Program.ProgramName,
                            ClassName = AdmissionTab.ClassSection.Class.ClassName,
                            StreamName = AdmissionTab.ClassSection.Stream.StreamName,
                            Remark = H.Remark,
                            CreatedBy = H.CreatedBy,
                            ModifiedBy = H.ModifiedBy,
                            CreatedDate = H.CreatedDate,
                            ModifiedDate = H.ModifiedDate
                        });

            return temp.FirstOrDefault();
        }


        public Sch_FeeRefundHeader Create(Sch_FeeRefundHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FeeRefundHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FeeRefundHeader>().Delete(id);
        }

        public void Delete(Sch_FeeRefundHeader pt)
        {
            _unitOfWork.Repository<Sch_FeeRefundHeader>().Delete(pt);
        }

        public void Update(Sch_FeeRefundHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FeeRefundHeader>().Update(pt);
        }

        public IEnumerable<Sch_FeeRefundHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_FeeRefundHeader>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public Sch_FeeRefundHeader Add(Sch_FeeRefundHeader pt)
        {
            _unitOfWork.Repository<Sch_FeeRefundHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_FeeRefundHeader
                        select p.FeeRefundHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FeeRefundHeader
                        select p.FeeRefundHeaderId).FirstOrDefault();
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

                temp = (from p in db.Sch_FeeRefundHeader
                        select p.FeeRefundHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FeeRefundHeader
                        select p.FeeRefundHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


      

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_FeeRefundHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_FeeRefundHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
