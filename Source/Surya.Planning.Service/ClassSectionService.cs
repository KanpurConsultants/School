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

namespace Surya.India.Service
{
    public interface IClassSectionService : IDisposable
    {
        Sch_ClassSection Create(Sch_ClassSection pt);
        void Delete(int id);
        void Delete(Sch_ClassSection pt);
        Sch_ClassSection Find(string Name);
        Sch_ClassSection Find(int id);

        IEnumerable<Sch_ClassSection> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_ClassSection pt);
        Sch_ClassSection Add(Sch_ClassSection pt);
        IEnumerable<Sch_ClassSection> GetClassSectionList();

        IEnumerable<Sch_ClassSectionHeaderViewModel> GetClassSectionHeaderList();

        IEnumerable<Sch_ClassSection> GetClassSectionList(int ProgramId, int ClassId, int StreamId);

        // IEnumerable<Sch_ClassSection> GetClassSectionList(int buyerId);
        Task<IEquatable<Sch_ClassSection>> GetAsync();
        Task<Sch_ClassSection> FindAsync(int id);
        Sch_ClassSection GetClassSectionByName(string terms);
        int NextId(int id);
        int PrevId(int id);
    }

    public class ClassSectionService : IClassSectionService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_ClassSection> _ClassSectionRepository;
        RepositoryQuery<Sch_ClassSection> ClassSectionRepository;
        public ClassSectionService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ClassSectionRepository = new Repository<Sch_ClassSection>(db);
            ClassSectionRepository = new RepositoryQuery<Sch_ClassSection>(_ClassSectionRepository);
        }
        public Sch_ClassSection GetClassSectionByName(string terms)
        {
            return (from p in db.Sch_ClassSection
                    where p.SectionName == terms
                    select p).FirstOrDefault();
        }

        public Sch_ClassSection Find(string Name)
        {
            return ClassSectionRepository.Get().Where(i => i.SectionName == Name).FirstOrDefault();
        }


        public Sch_ClassSection Find(int id)
        {
            return _unitOfWork.Repository<Sch_ClassSection>().Find(id);
        }

        public Sch_ClassSection Create(Sch_ClassSection pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_ClassSection>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_ClassSection>().Delete(id);
        }

        public void Delete(Sch_ClassSection pt)
        {
            _unitOfWork.Repository<Sch_ClassSection>().Delete(pt);
        }

        public void Update(Sch_ClassSection pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_ClassSection>().Update(pt);
        }

        public IEnumerable<Sch_ClassSection> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_ClassSection>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.SectionName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_ClassSection> GetClassSectionList()
        {
            var pt = _unitOfWork.Repository<Sch_ClassSection>().Query().Get().OrderBy(m=>m.SectionName);

            return pt;
        }

        public IEnumerable<Sch_ClassSectionHeaderViewModel> GetClassSectionHeaderList()
        {
            var pt = (from L in db.Sch_ClassSection
                      join P in db.Sch_Program on L.ProgramId equals P.ProgramId into ProgramTable
                      from ProgramTab in ProgramTable.DefaultIfEmpty()
                      join C in db.Sch_Class on L.ClassId equals C.ClassId into ClassTable
                      from ClassTab in ClassTable.DefaultIfEmpty()
                      join S in db.Sch_Stream on L.StreamId equals S.StreamId into StreamTable
                      from StreamTab in StreamTable.DefaultIfEmpty()
                      select new Sch_ClassSectionHeaderViewModel
                      {
                          ProgramId = ProgramTab.ProgramId,
                          ClassId = ClassTab.ClassId,
                          StreamId = StreamTab.StreamId,
                          ProgramName = ProgramTab.ProgramName,
                          ClassName = ClassTab.ClassName,
                          StreamName = StreamTab.StreamName,
                      }).Distinct().ToList();
                     

            return pt;
        }

        public IEnumerable<Sch_ClassSection> GetClassSectionList(int ProgramId, int ClassId, int StreamId)
        {
            var pt = _unitOfWork.Repository<Sch_ClassSection>().Query().Get().Where(i => i.ProgramId == ProgramId && i.ClassId == ClassId && i.StreamId == StreamId).OrderBy(m => m.SectionName);

            return pt;
        }



        public Sch_ClassSection Add(Sch_ClassSection pt)
        {
            _unitOfWork.Repository<Sch_ClassSection>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_ClassSection
                        orderby p.SectionName
                        select p.ClassSectionId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_ClassSection
                        orderby p.SectionName
                        select p.ClassSectionId).FirstOrDefault();
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

                temp = (from p in db.Sch_ClassSection
                        orderby p.SectionName
                        select p.ClassSectionId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_ClassSection
                        orderby p.SectionName
                        select p.ClassSectionId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_ClassSection>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_ClassSection> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
