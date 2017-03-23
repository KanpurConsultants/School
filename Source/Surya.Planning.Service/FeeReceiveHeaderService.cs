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
    public interface IFeeReceiveHeaderService : IDisposable
    {
        Sch_FeeReceiveHeader Create(Sch_FeeReceiveHeader pt);
        void Delete(int id);
        void Delete(Sch_FeeReceiveHeader pt);
        Sch_FeeReceiveHeader Find(int id);
        IEnumerable<Sch_FeeReceiveHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_FeeReceiveHeader pt);
        Sch_FeeReceiveHeader Add(Sch_FeeReceiveHeader pt);

        IEnumerable<Sch_FeeReceiveHeader> GetFeeReceiveHeaderList();

        IEnumerable<Sch_FeeReceiveHeaderViewModel> GetFeeReceiveHeaderListForIndex();

        Sch_FeeReceiveHeaderViewModel GetFeeReceiveHeaderForEdit(int FeeReceiveHeaderId);

        Task<IEquatable<Sch_FeeReceiveHeader>> GetAsync();
        Task<Sch_FeeReceiveHeader> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class FeeReceiveHeaderService : IFeeReceiveHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_FeeReceiveHeader> _FeeReceiveHeaderRepository;
        RepositoryQuery<Sch_FeeReceiveHeader> FeeReceiveHeaderRepository;
        public FeeReceiveHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FeeReceiveHeaderRepository = new Repository<Sch_FeeReceiveHeader>(db);
            FeeReceiveHeaderRepository = new RepositoryQuery<Sch_FeeReceiveHeader>(_FeeReceiveHeaderRepository);
        }
       
      

        public Sch_FeeReceiveHeader Find(int id)
        {
            return _unitOfWork.Repository<Sch_FeeReceiveHeader>().Find(id);
        }

        public IEnumerable<Sch_FeeReceiveHeader> GetFeeReceiveHeaderList()
        {
            return _unitOfWork.Repository<Sch_FeeReceiveHeader>().Query().Get();
        }

        public IEnumerable<Sch_FeeReceiveHeaderViewModel> GetFeeReceiveHeaderListForIndex()
        {
            var temp = (from H in db.Sch_FeeReceiveHeader
                        select new Sch_FeeReceiveHeaderViewModel
                        {
                            FeeReceiveHeaderId = H.FeeReceiveHeaderId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            StudentName = H.Student.Person.Name
                        });

            return temp.ToList();
        }


        public Sch_FeeReceiveHeaderViewModel GetFeeReceiveHeaderForEdit(int FeeReceiveHeaderId)
        {
            var temp = (from H in db.Sch_FeeReceiveHeader
                        join S in db.ViewStudentCurrentAdmission on H.StudentId equals S.PersonID into StudentTable from StudentTab in StudentTable.DefaultIfEmpty()
                        join A in db.Sch_Admission on StudentTab.AdmissionId equals A.AdmissionId into AdmissionTable from AdmissionTab in AdmissionTable.DefaultIfEmpty()
                        where H.FeeReceiveHeaderId == FeeReceiveHeaderId
                        select new Sch_FeeReceiveHeaderViewModel
                        {
                            FeeReceiveHeaderId = H.FeeReceiveHeaderId,
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


        public Sch_FeeReceiveHeader Create(Sch_FeeReceiveHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FeeReceiveHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FeeReceiveHeader>().Delete(id);
        }

        public void Delete(Sch_FeeReceiveHeader pt)
        {
            _unitOfWork.Repository<Sch_FeeReceiveHeader>().Delete(pt);
        }

        public void Update(Sch_FeeReceiveHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FeeReceiveHeader>().Update(pt);
        }

        public IEnumerable<Sch_FeeReceiveHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_FeeReceiveHeader>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public Sch_FeeReceiveHeader Add(Sch_FeeReceiveHeader pt)
        {
            _unitOfWork.Repository<Sch_FeeReceiveHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_FeeReceiveHeader
                        select p.FeeReceiveHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FeeReceiveHeader
                        select p.FeeReceiveHeaderId).FirstOrDefault();
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

                temp = (from p in db.Sch_FeeReceiveHeader
                        select p.FeeReceiveHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FeeReceiveHeader
                        select p.FeeReceiveHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


      

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_FeeReceiveHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_FeeReceiveHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
