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

namespace Surya.India.Service
{
    public interface IPersonProcessService : IDisposable
    {
        PersonProcess Create(PersonProcess pt);
        void Delete(int id);
        void Delete(PersonProcess pt);
        PersonProcess GetPersonProcess(int ptId);
        IEnumerable<PersonProcess> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(PersonProcess pt);
        PersonProcess Add(PersonProcess pt);
        IEnumerable<PersonProcess> GetPersonProcessList();
        IEnumerable<PersonProcess> GetPersonProcessList(int id);
        PersonProcess Find(int id);
        // IEnumerable<PersonProcess> GetPersonProcessList(int buyerId);
        Task<IEquatable<PersonProcess>> GetAsync();
        Task<PersonProcess> FindAsync(int id);
        IEnumerable<PersonProcess> GetPersonProcessIdListByPersonId(int PersonId);
    }

    public class PersonProcessService : IPersonProcessService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<PersonProcess> _PersonProcessRepository;
        RepositoryQuery<PersonProcess> PersonProcessRepository;
        public PersonProcessService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _PersonProcessRepository = new Repository<PersonProcess>(db);
            PersonProcessRepository = new RepositoryQuery<PersonProcess>(_PersonProcessRepository);
        }

        public PersonProcess GetPersonProcess(int pt)
        {
            return PersonProcessRepository.Include(r => r.Person).Get().Where(i => i.PersonProcessId == pt).FirstOrDefault();
        }

        public PersonProcess Create(PersonProcess pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<PersonProcess>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<PersonProcess>().Delete(id);
        }

        public void Delete(PersonProcess pt)
        {
            _unitOfWork.Repository<PersonProcess>().Delete(pt);
        }

        public void Update(PersonProcess pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<PersonProcess>().Update(pt);
        }

        public IEnumerable<PersonProcess> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<PersonProcess>()
                .Query()
                //.OrderBy(q => q.OrderBy(c => c.Supplier ))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<PersonProcess> GetPersonProcessList()
        {
            var pt = _unitOfWork.Repository<PersonProcess>().Query().Include(p => p.Person).Get();
            return pt;
        }

        public PersonProcess Add(PersonProcess pt)
        {
            _unitOfWork.Repository<PersonProcess>().Insert(pt);
            return pt;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<PersonProcess>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PersonProcess> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<PersonProcess> GetPersonProcessList(int id)
        {
            var pt = _unitOfWork.Repository<PersonProcess>().Query().Include(m=>m.Person).Get().Where(m => m.PersonId == id).ToList();
            return pt;
        }
        public PersonProcess Find(int id)
        {
            return _unitOfWork.Repository<PersonProcess>().Find(id);
        }

        public IEnumerable<PersonProcess> GetPersonProcessIdListByPersonId(int PersonId)
        {
            var pt = _unitOfWork.Repository<PersonProcess>().Query().Get().Where(m => m.PersonId == PersonId).ToList();
            return pt;
        }

    }
}
