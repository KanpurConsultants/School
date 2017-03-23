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
    public interface IFacilityStopService : IDisposable
    {
        Sch_FacilityStop Create(Sch_FacilityStop pt);
        void Delete(int id);
        void Delete(Sch_FacilityStop pt);
        Sch_FacilityStop Find(int id);

        IEnumerable<Sch_FacilityStop> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Sch_FacilityStop pt);
        Sch_FacilityStop Add(Sch_FacilityStop pt);
        IEnumerable<Sch_FacilityStop> GetFacilityStopList();





        // IEnumerable<Sch_FacilityStop> GetFacilityStopList(int buyerId);
        Task<IEquatable<Sch_FacilityStop>> GetAsync();
        Task<Sch_FacilityStop> FindAsync(int id);

        IEnumerable<Sch_FacilityStopViewModel> GetFacilityStopListForIndex(int FacilityStopHeaderId);


        Sch_FacilityStopViewModel GetFacilityStopForEdit(int FacilityStopHeaderId);
    }

    public class FacilityStopService : IFacilityStopService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Sch_FacilityStop> _FacilityStopRepository;
        RepositoryQuery<Sch_FacilityStop> FacilityStopRepository;
        public FacilityStopService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _FacilityStopRepository = new Repository<Sch_FacilityStop>(db);
            FacilityStopRepository = new RepositoryQuery<Sch_FacilityStop>(_FacilityStopRepository);
        }
     
      
        public Sch_FacilityStop Find(int id)
        {
            return _unitOfWork.Repository<Sch_FacilityStop>().Find(id);
        }

        public Sch_FacilityStop Create(Sch_FacilityStop pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Sch_FacilityStop>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Sch_FacilityStop>().Delete(id);
        }

        public void Delete(Sch_FacilityStop pt)
        {
            _unitOfWork.Repository<Sch_FacilityStop>().Delete(pt);
        }

        public void Update(Sch_FacilityStop pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Sch_FacilityStop>().Update(pt);
        }

        public IEnumerable<Sch_FacilityStop> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Sch_FacilityStop>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Sch_FacilityStop> GetFacilityStopList()
        {
            var pt = _unitOfWork.Repository<Sch_FacilityStop>().Query().Get();

            return pt;
        }

        public Sch_FacilityStop Add(Sch_FacilityStop pt)
        {
            _unitOfWork.Repository<Sch_FacilityStop>().Insert(pt);
            return pt;
        }

        public IEnumerable<Sch_FacilityStopViewModel> GetFacilityStopListForIndex(int FacilityStopHeaderId)
        {
            var pt = (from C in db.Sch_FacilityStop
                      where C.FacilityStopHeaderId == FacilityStopHeaderId
                      select new Sch_FacilityStopViewModel
                      {
                          FacilityStopId = C.FacilityStopId,
                          FacilityName = C.FacilityEnrollment.FacilitySubCategory.Facility.FacilityName,
                          StartDate = C.FacilityEnrollment.StartDate,
                          AvailDays = C.AvailDays,
                          StopReason = C.StopReason
                      });


            return pt.ToList();
        }

        public Sch_FacilityStopViewModel GetFacilityStopForEdit(int FacilityStopHeaderId)
        {
            var pt = (from C in db.Sch_FacilityStop
                      select new Sch_FacilityStopViewModel
                      {
                          FacilityStopId = C.FacilityStopId,
                          FacilityEnrollmentId = C.FacilityEnrollmentId,
                          FacilityName = C.FacilityEnrollment.FacilitySubCategory.Facility.FacilityName,
                          StartDate = C.FacilityEnrollment.StartDate,
                          AvailDays = C.AvailDays,
                          StopReason = C.StopReason,
                          AdmissionId = C.FacilityStopHeader.AdmissionId
                      }).FirstOrDefault();


            return pt;
        }

     

        public void Dispose()
        {
        }


        public Task<IEquatable<Sch_FacilityStop>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Sch_FacilityStop> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
