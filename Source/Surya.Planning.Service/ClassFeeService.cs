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
    public interface IClassFeeService : IDisposable
    {
        Sch_ClassFee Create(Sch_ClassFee pt);
        void Delete(int id);
        void Delete(Sch_ClassFee pt);
        Sch_ClassFee Find(int id);
        IEnumerable<Sch_ClassFee> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_ClassFee pt);
        Sch_ClassFee Add(Sch_ClassFee pt);
        IEnumerable<Sch_ClassFeeViewModel> GetClassFeeList();

        // IEnumerable<Sch_ClassFee> GetClassFeeList(int buyerId);
        Task<IEquatable<Sch_ClassFee>> GetAsync();
        Task<Sch_ClassFee> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class ClassFeeService : IClassFeeService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_ClassFee> _ClassFeeRepository;
        RepositoryQuery<Sch_ClassFee> ClassFeeRepository;
        public ClassFeeService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ClassFeeRepository = new Repository<Sch_ClassFee>(db);
            ClassFeeRepository = new RepositoryQuery<Sch_ClassFee>(_ClassFeeRepository);
        }
       
      

        public Sch_ClassFee Find(int id)
        {
            return _unitOfWork.Repository<Sch_ClassFee>().Find(id);
        }

        public Sch_ClassFee Create(Sch_ClassFee pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_ClassFee>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_ClassFee>().Delete(id);
        }

        public void Delete(Sch_ClassFee pt)
        {
            _unitOfWork.Repository<Sch_ClassFee>().Delete(pt);
        }

        public void Update(Sch_ClassFee pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_ClassFee>().Update(pt);
        }

        public IEnumerable<Sch_ClassFee> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_ClassFee>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_ClassFeeViewModel> GetClassFeeList()
        {
            var pt = from Cf in db.Sch_ClassFee
                     join P in db.Sch_Program on Cf.ProgramId equals P.ProgramId into ProgramTable
                     from ProgramTab in ProgramTable.DefaultIfEmpty()
                     join C in db.Sch_Class on Cf.ClassId equals C.ClassId into ClassTable
                     from ClassTab in ClassTable.DefaultIfEmpty()
                     join S in db.Sch_Stream on Cf.StreamId equals S.StreamId into StreamTable
                     from StreamTab in StreamTable.DefaultIfEmpty()
                     select new Sch_ClassFeeViewModel
                     {
                         ClassFeeId = Cf.ClassFeeId,
                         ProgramId = Cf.ProgramId,
                         ProgramName = ProgramTab.ProgramName,
                         AdmissionQuotaId = Cf.AdmissionQuotaId,
                         AdmissionQuotaName  = Cf.AdmissionQuota.AdmissionQuotaName,
                         ClassId = Cf.ClassId,
                         ClassName = ClassTab.ClassName,
                         StreamId = Cf.StreamId,
                         StreamName = StreamTab.StreamName
                     };

            return pt.ToList();
        }

        public Sch_ClassFee Add(Sch_ClassFee pt)
        {
            _unitOfWork.Repository<Sch_ClassFee>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Sch_ClassFee
                        select p.ClassFeeId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_ClassFee
                        select p.ClassFeeId).FirstOrDefault();
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

                temp = (from p in db.Sch_ClassFee
                        select p.ClassFeeId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Sch_ClassFee
                        select p.ClassFeeId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_ClassFee>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_ClassFee> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
