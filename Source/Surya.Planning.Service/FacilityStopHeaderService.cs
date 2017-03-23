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
    public interface IFacilityStopHeaderService : IDisposable
    {
        Sch_FacilityStopHeader Create(Sch_FacilityStopHeader s);
        void Delete(int id);
        void Delete(Sch_FacilityStopHeader s);
        Sch_FacilityStopHeader Find(int id);
        void Update(Sch_FacilityStopHeader s);
        string GetMaxDocNo();

        int NextId(int id);
        int PrevId(int id);

        IEnumerable<Sch_FacilityStopHeaderViewModel> GetFacilityStopHeaderListForIndex();

        Sch_FacilityStopHeaderViewModel GetFacilityStopHeaderForEdit(int FacilityStopHeaderId);



    }
    public class FacilityStopHeaderService : IFacilityStopHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public FacilityStopHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

        public Sch_FacilityStopHeader Create(Sch_FacilityStopHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FacilityStopHeader>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FacilityStopHeader>().Delete(id);
        }
        public void Delete(Sch_FacilityStopHeader s)
        {
            _unitOfWork.Repository<Sch_FacilityStopHeader>().Delete(s);
        }
        public void Update(Sch_FacilityStopHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FacilityStopHeader>().Update(s);
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_FacilityStopHeader
                        select p.FacilityStopHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FacilityStopHeader
                        select p.FacilityStopHeaderId).FirstOrDefault();
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

                temp = (from p in db.Sch_FacilityStopHeader
                        select p.FacilityStopHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_FacilityStopHeader
                        select p.FacilityStopHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public Sch_FacilityStopHeader Find(int id)
        {
            return _unitOfWork.Repository<Sch_FacilityStopHeader>().Find(id);
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<Sch_FacilityStopHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public IEnumerable<Sch_FacilityStopHeaderViewModel> GetFacilityStopHeaderListForIndex()
        {
            var temp = (from H in db.Sch_FacilityStopHeader
                        join A in db.Sch_Admission on H.AdmissionId equals A.AdmissionId into AdmissionTable from AdmissionTab in AdmissionTable.DefaultIfEmpty()
                        join P in db.Persons on AdmissionTab.StudentId equals P.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        select new Sch_FacilityStopHeaderViewModel
                        {
                            FacilityStopHeaderId = H.FacilityStopHeaderId,
                            DocNo = H.DocNo,
                            DocDate = H.DocDate,
                            StudentName = PersonTab.Name,
                        });

            return temp.ToList();
        }

        public Sch_FacilityStopHeaderViewModel GetFacilityStopHeaderForEdit(int FacilityStopHeaderId)
        {
            var temp = (from H in db.Sch_FacilityStopHeader
                        join A in db.Sch_Admission on H.AdmissionId equals A.AdmissionId into AdmissionTable from AdmissionTab in AdmissionTable.DefaultIfEmpty()
                        join Cs in db.Sch_ClassSection on AdmissionTab.ClassSectionId equals Cs.ClassSectionId into ClassSectionTable
                        from ClassSectionTab in ClassSectionTable.DefaultIfEmpty()
                        join P in db.Persons on AdmissionTab.StudentId equals P.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        where H.FacilityStopHeaderId == FacilityStopHeaderId
                        select new Sch_FacilityStopHeaderViewModel
                        {
                            FacilityStopHeaderId = H.FacilityStopHeaderId,
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
    }
}
