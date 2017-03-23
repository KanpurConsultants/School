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
    public interface IStudentService : IDisposable
    {
        Sch_Student Create(Sch_Student pt);
        void Delete(int id);
        void Delete(Sch_Student pt);
        Sch_Student Find(int id);
        IEnumerable<Sch_Student> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_Student pt);
        Sch_Student Add(Sch_Student pt);

        IEnumerable<Sch_Student> GetStudentList();

        Sch_StudentViewModel GetStudentDetail(int StudentId);

        Sch_AdmissionViewModel GetStudentClassDetail(int StudentId);

        IEnumerable<Sch_StudentViewModel> GetStudentListForIndex();

        Sch_StudentViewModel GetStudentForEdit(int StudentId);

        Task<IEquatable<Sch_Student>> GetAsync();
        Task<Sch_Student> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class StudentService : IStudentService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_Student> _StudentRepository;
        RepositoryQuery<Sch_Student> StudentRepository;
        public StudentService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _StudentRepository = new Repository<Sch_Student>(db);
            StudentRepository = new RepositoryQuery<Sch_Student>(_StudentRepository);
        }
       
      

        public Sch_Student Find(int id)
        {
            return _unitOfWork.Repository<Sch_Student>().Find(id);
        }

        public IEnumerable<Sch_Student> GetStudentList()
        {
            return _unitOfWork.Repository<Sch_Student>().Query().Get();
        }


        public Sch_Student Create(Sch_Student pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_Student>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_Student>().Delete(id);
        }

        public void Delete(Sch_Student pt)
        {
            _unitOfWork.Repository<Sch_Student>().Delete(pt);
        }

        public void Update(Sch_Student pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_Student>().Update(pt);
        }

        public IEnumerable<Sch_Student> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_Student>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public Sch_Student Add(Sch_Student pt)
        {
            _unitOfWork.Repository<Sch_Student>().Insert(pt);
            return pt;
        }

        public Sch_StudentViewModel GetStudentDetail(int StudentId)
        {
            var temp = (from S in db.Sch_Student
                        join P in db.Persons on S.PersonID equals P.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        join Pa in db.PersonAddress on S.PersonID equals Pa.PersonId into PersonAddressTable
                        from PersonAddressTab in PersonAddressTable.DefaultIfEmpty()
                        join Pc in db.PersonContacts on S.PersonID equals Pc.PersonId into PersonContactTable
                        from PersonContactTab in PersonContactTable.DefaultIfEmpty()
                        join Pcp in db.Persons on PersonContactTab.ContactId equals Pcp.PersonID into PersonContactPersonTable
                        from PersonContactPersonTab in PersonContactPersonTable.DefaultIfEmpty()
                        join C in db.City on PersonAddressTab.CityId equals C.CityId into CityTable
                        from CityTab in CityTable.DefaultIfEmpty()
                        select new Sch_StudentViewModel
                        {
                            Name = PersonTab.Name,
                            Code = PersonTab.Code,
                            Suffix = PersonTab.Suffix,
                            Gender = S.Gender,
                            DOB = S.DOB,
                            Religion = S.Religion,
                            CastCategory = S.CastCategory,
                            Address = PersonAddressTab.Address,
                            CityId = PersonAddressTab.CityId,
                            CityName = CityTab.CityName,
                            Zipcode = PersonAddressTab.Zipcode,
                            Mobile = PersonTab.Mobile,
                            Email = PersonTab.Email,
                            FatherName = S.FatherName,
                            MotherName = S.MotherName,
                            GuardianName = PersonContactPersonTab.Name,
                            GuardianMobile = PersonContactPersonTab.Mobile,
                            GuardianEMail = PersonContactPersonTab.Email,
                        }).FirstOrDefault();

            return temp;
        }

        public Sch_AdmissionViewModel GetStudentClassDetail(int StudentId)
        {
            var temp = (from S in db.ViewStudentCurrentAdmission
                        join A in db.Sch_Admission on S.PersonID equals A.StudentId into AdmissionTable
                        from AdmissionTab in AdmissionTable.DefaultIfEmpty()
                        where S.PersonID == StudentId
                        select new Sch_AdmissionViewModel
                        {
                            ProgramId = AdmissionTab.ClassSection.Class.ProgramId,
                            ProgramName = AdmissionTab.ClassSection.Class.Program.ProgramName,
                            ClassId = AdmissionTab.ClassSection.ClassId,
                            ClassName = AdmissionTab.ClassSection.Class.ClassName,
                            StreamId = AdmissionTab.ClassSection.Stream.StreamId,
                            StreamName = AdmissionTab.ClassSection.Stream.StreamName
                        }).FirstOrDefault();

            return temp;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_Student
                        select p.PersonID).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_Student
                        select p.PersonID).FirstOrDefault();
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

                temp = (from p in db.Sch_Student
                        select p.PersonID).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_Student
                        select p.PersonID).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public IEnumerable<Sch_StudentViewModel> GetStudentListForIndex()
        {
            var temp = (from H in db.Sch_Student
                        join P in db.Persons on H.PersonID equals P.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        select new Sch_StudentViewModel
                        {
                            PersonId = H.PersonID,
                            Name = PersonTab.Name,
                            FatherName = H.FatherName
                        });

            return temp.ToList();
        }

        public Sch_StudentViewModel GetStudentForEdit(int StudentId)
        {
            var temp = (from H in db.Sch_Student
                        join S in db.Persons on H.PersonID equals S.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        join Pa in db.PersonAddress on H.PersonID equals Pa.PersonId into PersonAddressTable
                        from PersonAddressTab in PersonAddressTable.DefaultIfEmpty()
                        join Pc in db.PersonContacts on H.PersonID equals Pc.PersonId into PersonContactTable
                        from PersonContactTab in PersonContactTable.DefaultIfEmpty()
                        join Pcp in db.Persons on PersonContactTab.ContactId equals Pcp.PersonID into PersonContactPersonTable
                        from PersonContactPersonTab in PersonContactPersonTable.DefaultIfEmpty()
                        where H.PersonID == StudentId
                        select new Sch_StudentViewModel
                        {
                            PersonId = H.PersonID,
                            Name = PersonTab.Name,
                            Suffix = PersonTab.Suffix,
                            Code = PersonTab.Code,
                            Gender = H.Gender,
                            DOB = H.DOB,
                            CastCategory = H.CastCategory,
                            Religion = H.Religion,
                            Address = PersonAddressTab.Address,
                            CityId = PersonAddressTab.CityId,
                            Zipcode = PersonAddressTab.Zipcode,
                            Mobile = PersonTab.Mobile,
                            Email = PersonTab.Email,
                            FatherName = H.FatherName,
                            MotherName = H.MotherName,
                            GuardianName = PersonContactPersonTab.Name,
                            GuardianMobile = PersonContactPersonTab.Mobile,
                            GuardianEMail = PersonContactPersonTab.Email,
                            CreatedBy = PersonTab.CreatedBy,
                            ModifiedBy = PersonTab.ModifiedBy,
                            CreatedDate = PersonTab.CreatedDate,
                            ModifiedDate = PersonTab.ModifiedDate
                        });

            return temp.FirstOrDefault();
        }


        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_Student>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_Student> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
