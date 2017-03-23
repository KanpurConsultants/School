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
    public interface IAdmissionService : IDisposable
    {
        Sch_Admission Create(Sch_Admission pt);
        void Delete(int id);
        void Delete(Sch_Admission pt);
        Sch_Admission Find(int id);
        IEnumerable<Sch_Admission> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_Admission pt);
        Sch_Admission Add(Sch_Admission pt);

        IEnumerable<Sch_Admission> GetAdmissionList();

        IEnumerable<Sch_AdmissionViewModel> GetAdmissionListForIndex();

        Sch_AdmissionViewModel GetAdmissionForEdit(int AdmissionId);

        Sch_AdmissionViewModel GetAdmissionDetail(int AdmissionId);
        Task<IEquatable<Sch_Admission>> GetAsync();
        Task<Sch_Admission> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class AdmissionService : IAdmissionService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_Admission> _AdmissionRepository;
        RepositoryQuery<Sch_Admission> AdmissionRepository;
        public AdmissionService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _AdmissionRepository = new Repository<Sch_Admission>(db);
            AdmissionRepository = new RepositoryQuery<Sch_Admission>(_AdmissionRepository);
        }
       
      

        public Sch_Admission Find(int id)
        {
            return _unitOfWork.Repository<Sch_Admission>().Find(id);
        }

        public IEnumerable<Sch_Admission> GetAdmissionList()
        {
            return _unitOfWork.Repository<Sch_Admission>().Query().Get();
        }

        public IEnumerable<Sch_AdmissionViewModel> GetAdmissionListForIndex()
        {
            var temp = (from H in db.Sch_Admission
                        join S in db.Persons on H.StudentId equals S.PersonID into StudentTable
                        from StudentTab in StudentTable.DefaultIfEmpty()
                        join Cs in db.Sch_ClassSection on H.ClassSectionId equals Cs.ClassSectionId into ClassSectionTable
                        from ClassSectionTab in ClassSectionTable.DefaultIfEmpty()
                        join C in db.Sch_Class on ClassSectionTab.ClassId equals C.ClassId into ClassTable
                        from ClassTab in ClassTable.DefaultIfEmpty()
                        select new Sch_AdmissionViewModel
                        {
                            AdmissionId = H.AdmissionId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            StudentName = StudentTab.Name,
                            ClassSectionName = ClassTab.ClassName + "-" + ClassSectionTab.SectionName
                        });

            return temp.ToList();
        }


        public Sch_AdmissionViewModel GetAdmissionForEdit(int AdmissionId)
        {
            var temp = (from H in db.Sch_Admission
                        join Aq in db.Sch_AdmissionQuota  on H.AdmissionQuotaId equals Aq.AdmissionQuotaId into AdmissionQuotaTable from AdmissionQuotaTab in AdmissionQuotaTable.DefaultIfEmpty()
                        join S in db.Persons on H.StudentId equals S.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        join S in db.Sch_Student on H.StudentId equals S.PersonID into StudentTable
                        from StudentTab in StudentTable.DefaultIfEmpty()
                        join Cs in db.Sch_ClassSection on H.ClassSectionId equals Cs.ClassSectionId into ClassSectionTable
                        from ClassSectionTab in ClassSectionTable.DefaultIfEmpty()
                        join C in db.Sch_Class on ClassSectionTab.ClassId equals C.ClassId into ClassTable
                        from ClassTab in ClassTable.DefaultIfEmpty()
                        join Pa in db.PersonAddress on H.StudentId equals Pa.PersonId into PersonAddressTable
                        from PersonAddressTab in PersonAddressTable.DefaultIfEmpty()
                        join Pc in db.PersonContacts on H.StudentId equals Pc.PersonId into PersonContactTable
                        from PersonContactTab in PersonContactTable.DefaultIfEmpty()
                        join Pcp in db.Persons on PersonContactTab.ContactId equals Pcp.PersonID into PersonContactPersonTable
                        from PersonContactPersonTab in PersonContactPersonTable.DefaultIfEmpty()
                        where H.AdmissionId == AdmissionId
                        select new Sch_AdmissionViewModel
                        {
                            AdmissionId = H.AdmissionId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            AdmissionQuotaId = H.AdmissionQuotaId,
                            AdmissionQuotaName = AdmissionQuotaTab.AdmissionQuotaName,
                            ClassSectionId = H.ClassSectionId,
                            ClassSectionName = ClassTab.ClassName + "-" + ClassSectionTab.SectionName,
                            RollNo = H.RollNo,
                            PersonId = H.StudentId,
                            StudentName = PersonTab.Name,
                            Suffix = PersonTab.Suffix,
                            Code = PersonTab.Code,
                            Gender = StudentTab.Gender,
                            DOB = StudentTab.DOB,
                            CastCategory = StudentTab.CastCategory,
                            Religion = StudentTab.Religion,
                            Address = PersonAddressTab.Address,
                            CityId = PersonAddressTab.CityId,
                            Zipcode = PersonAddressTab.Zipcode,
                            Mobile = PersonTab.Mobile,
                            EMail = PersonTab.Email,
                            FatherName = StudentTab.FatherName,
                            MotherName = StudentTab.MotherName,
                            GuardianName = PersonContactPersonTab.Name,
                            GuardianMobile = PersonContactPersonTab.Mobile,
                            GuardianEMail = PersonContactPersonTab.Email,
                            CreatedBy = H.CreatedBy,
                            ModifiedBy = H.ModifiedBy,
                            CreatedDate = H.CreatedDate,
                            ModifiedDate = H.ModifiedDate
                        });

            return temp.FirstOrDefault();
        }


        public Sch_Admission Create(Sch_Admission pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_Admission>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_Admission>().Delete(id);
        }

        public void Delete(Sch_Admission pt)
        {
            _unitOfWork.Repository<Sch_Admission>().Delete(pt);
        }

        public void Update(Sch_Admission pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_Admission>().Update(pt);
        }

        public IEnumerable<Sch_Admission> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_Admission>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public Sch_Admission Add(Sch_Admission pt)
        {
            _unitOfWork.Repository<Sch_Admission>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_Admission
                        select p.AdmissionId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_Admission
                        select p.AdmissionId).FirstOrDefault();
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

                temp = (from p in db.Sch_Admission
                        select p.AdmissionId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_Admission
                        select p.AdmissionId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public Sch_AdmissionViewModel GetAdmissionDetail(int AdmissionId)
        {
            var temp = (from S in db.Sch_Admission
                        where S.AdmissionId == AdmissionId
                        select new Sch_AdmissionViewModel
                        {
                            ProgramId = S.ClassSection.Class.ProgramId,
                            ProgramName = S.ClassSection.Class.Program.ProgramName,
                            ClassId = S.ClassSection.ClassId,
                            ClassName = S.ClassSection.Class.ClassName,
                            StreamId = S.ClassSection.Stream.StreamId,
                            StreamName = S.ClassSection.Stream.StreamName
                        }).FirstOrDefault();

            return temp;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_Admission>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_Admission> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
