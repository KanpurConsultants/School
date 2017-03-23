using Surya.India.Data.Infrastructure;
using Surya.India.Data.Models;
using Surya.India.Model;
using Surya.India.Model.Models;
using Surya.India.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surya.India.Model.ViewModels;
using Surya.India.Model.ViewModel;

namespace Surya.India.Service
{
    public interface IFacilityEnrollmentService : IDisposable
    {
        Sch_FacilityEnrollment Create(Sch_FacilityEnrollment s);
        void Delete(int id);
        void Delete(Sch_FacilityEnrollment s);
        Sch_FacilityEnrollment Find(int id);
        void Update(Sch_FacilityEnrollment s);
        string GetMaxDocNo();

        int NextId(int id);
        int PrevId(int id);

        IEnumerable<Sch_FacilityEnrollmentViewModel> GetFacilityEnrollmentListForIndex();

        Sch_FacilityEnrollmentViewModel GetFacilityEnrollmentForEdit(int FacilityEnrollmentId);

        Sch_FacilityEnrollmentViewModel   GetFacilityEnrollmentDetail(int FacilityEnrollmentId);



    }
    public class FacilityEnrollmentService : IFacilityEnrollmentService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public FacilityEnrollmentService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

        public Sch_FacilityEnrollment Create(Sch_FacilityEnrollment s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FacilityEnrollment>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FacilityEnrollment>().Delete(id);
        }
        public void Delete(Sch_FacilityEnrollment s)
        {
            _unitOfWork.Repository<Sch_FacilityEnrollment>().Delete(s);
        }
        public void Update(Sch_FacilityEnrollment s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FacilityEnrollment>().Update(s);
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_FacilityEnrollment
                        select p.FacilityEnrollmentId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FacilityEnrollment
                        select p.FacilityEnrollmentId).FirstOrDefault();
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

                temp = (from p in db.Sch_FacilityEnrollment
                        select p.FacilityEnrollmentId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FacilityEnrollment
                        select p.FacilityEnrollmentId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public Sch_FacilityEnrollment Find(int id)
        {
            return _unitOfWork.Repository<Sch_FacilityEnrollment>().Find(id);
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<Sch_FacilityEnrollment>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public IEnumerable<Sch_FacilityEnrollmentViewModel> GetFacilityEnrollmentListForIndex()
        {
            var temp = (from H in db.Sch_FacilityEnrollment
                        join A in db.Sch_Admission on H.AdmissionId equals A.AdmissionId into AdmissionTable from AdmissionTab in AdmissionTable.DefaultIfEmpty()
                        join P in db.Persons on AdmissionTab.StudentId equals P.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        join Fc in db.Sch_FacilitySubCategory on H.FacilitySubCategoryId equals Fc.FacilitySubCategoryId into FacilitySubCategoryTable
                        from FacilitySubCategoryTab in FacilitySubCategoryTable.DefaultIfEmpty()
                        join F in db.Sch_Facility on FacilitySubCategoryTab.FacilityId equals F.FacilityId into FacilityTable
                        from FacilityTab in FacilityTable.DefaultIfEmpty()
                        select new Sch_FacilityEnrollmentViewModel
                        {
                            FacilityEnrollmentId = H.FacilityEnrollmentId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            StudentName = PersonTab.Name,
                            FacilityName = FacilityTab.FacilityName,
                            FacilitySubCategoryName = FacilitySubCategoryTab.FacilitySubCategoryName,
                            StartDate = H.StartDate
                        });

            return temp.ToList();
        }

        public Sch_FacilityEnrollmentViewModel GetFacilityEnrollmentForEdit(int FacilityEnrollmentId)
        {
            var temp = (from H in db.Sch_FacilityEnrollment
                        join A in db.Sch_Admission on H.AdmissionId equals A.AdmissionId into AdmissionTable from AdmissionTab in AdmissionTable.DefaultIfEmpty()
                        join Cs in db.Sch_ClassSection on AdmissionTab.ClassSectionId equals Cs.ClassSectionId into ClassSectionTable
                        from ClassSectionTab in ClassSectionTable.DefaultIfEmpty()
                        join P in db.Persons on AdmissionTab.StudentId equals P.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        join Fc in db.Sch_FacilitySubCategory on H.FacilitySubCategoryId equals Fc.FacilitySubCategoryId into FacilitySubCategoryTable
                        from FacilitySubCategoryTab in FacilitySubCategoryTable.DefaultIfEmpty()
                        join F in db.Sch_Facility on FacilitySubCategoryTab.FacilityId equals F.FacilityId into FacilityTable
                        from FacilityTab in FacilityTable.DefaultIfEmpty()
                        where H.FacilityEnrollmentId == FacilityEnrollmentId
                        select new Sch_FacilityEnrollmentViewModel
                        {
                            FacilityEnrollmentId = H.FacilityEnrollmentId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            AdmissionId = H.AdmissionId,
                            StudentId = AdmissionTab.StudentId,
                            StudentName = PersonTab.Name,
                            ProgramId = ClassSectionTab.ProgramId,
                            ProgramName = ClassSectionTab.Program.ProgramName,
                            StreamId = ClassSectionTab.StreamId,
                            StreamName = ClassSectionTab.Stream.StreamName,
                            ClassId = ClassSectionTab.ClassId,
                            ClassName = ClassSectionTab.Class.ClassName,
                            FacilityId = FacilityTab.FacilityId,
                            FacilityName = FacilityTab.FacilityName,
                            FacilitySubCategoryId = H.FacilitySubCategoryId,
                            FacilitySubCategoryName = FacilitySubCategoryTab.FacilitySubCategoryName,
                            StartDate = H.StartDate,
                            CreatedBy = PersonTab.CreatedBy,
                            ModifiedBy = PersonTab.ModifiedBy,
                            CreatedDate = PersonTab.CreatedDate,
                            ModifiedDate = PersonTab.ModifiedDate
                        });

            return temp.FirstOrDefault();
        }

        public Sch_FacilityEnrollmentViewModel  GetFacilityEnrollmentDetail(int FacilityEnrollmentId)
        {
            var temp = (from H in db.Sch_FacilityEnrollment
                       where H.FacilityEnrollmentId == FacilityEnrollmentId
                       select new Sch_FacilityEnrollmentViewModel
                       {
                           FacilityEnrollmentId = H.FacilityEnrollmentId,
                           StartDate = H.StartDate
                       }).FirstOrDefault();

            return temp;
        }




        public void Dispose()
        {
        }
    }
}
