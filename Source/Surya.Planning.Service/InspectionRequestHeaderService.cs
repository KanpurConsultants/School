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
    public interface IInspectionRequestHeaderService : IDisposable
    {
        InspectionRequestHeader Create(InspectionRequestHeader p);
        void Delete(int id);
        void Delete(InspectionRequestHeader p);
        InspectionRequestHeader Find(int p);
        InspectionRequestHeader GetInspectionRequestHeader(int p);
        IEnumerable<InspectionRequestHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(InspectionRequestHeader p);
        InspectionRequestHeader Add(InspectionRequestHeader p);
        IEnumerable<InspectionRequestHeader> GetPendingInspectionRequestHeaderList();
        IEnumerable<InspectionRequestHeader> GetPendingInspectionRequestHeaderList(int SupplierId);
        IEnumerable<InspectionRequestHeader> GetAllInspectionRequestHeaderList();
        IEnumerable<InspectionRequestHeader> GetAllInspectionRequestHeaderList(int SupplierId);
        Task<IEquatable<InspectionRequestHeader>> GetAsync();
        Task<InspectionRequestHeader> FindAsync(int id);
        string CheckDuplicateDocNo(string DocId);
        string CheckDuplicateDocNo(string DocId, string HeaderId);
        string GetMaxDocNo();
    }

    public class InspectionRequestHeaderService : IInspectionRequestHeaderService
    {
        ApplicationDbContext db =new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<InspectionRequestHeader> _InspectionRequestHeaderRepository;
        RepositoryQuery<InspectionRequestHeader> InspectionRequestHeaderRepository;

        public InspectionRequestHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _InspectionRequestHeaderRepository = new Repository<InspectionRequestHeader>(db);
            InspectionRequestHeaderRepository = new RepositoryQuery<InspectionRequestHeader>(_InspectionRequestHeaderRepository);
        }

        public InspectionRequestHeader GetInspectionRequestHeader(int pId)
        {
            return InspectionRequestHeaderRepository
                .Include(i => i.Supplier )     
                .Include(i=>i.PurchaseOrderHeader)
                .Get().Where(i => i.InspectionRequestHeaderId == pId).FirstOrDefault();
           //return _unitOfWork.Repository<SalesOrder>().Find(soId);
        }

        public InspectionRequestHeader Find(int pId)
        {
            return _unitOfWork.Repository<InspectionRequestHeader>().Find(pId);
        }



        public InspectionRequestHeader Create(InspectionRequestHeader p)
        {
            p.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<InspectionRequestHeader>().Insert(p);
            return p;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<InspectionRequestHeader>().Delete(id);
        }

        public void Delete(InspectionRequestHeader p)
        {
            _unitOfWork.Repository<InspectionRequestHeader>().Delete(p);
        }

        public void Update(InspectionRequestHeader p)
        {
            p.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<InspectionRequestHeader>().Update(p);
        }

        public IEnumerable<InspectionRequestHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<InspectionRequestHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.InspectionRequestHeaderId))
                .Filter(q => !string.IsNullOrEmpty(q.DocNo ))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }


        public IEnumerable<InspectionRequestHeader> GetPendingInspectionRequestHeaderList()
        {
            List<InspectionRequestHeader> rl = new List<InspectionRequestHeader>();
            //var InspectionRequests = _unitOfWork.Repository<InspectionRequestHeader>().Query()
            //            .Include(i => i.Supplier).Include(m => m.ProductType).Get();

            var InspectionRequests = _unitOfWork.Repository<InspectionRequestHeader>().Query()
                       .Include(i => i.InspectionRequestLines)
                       .Include(i => i.InspectionRequestLines.Select(m => m.InspectionLines))
                       .Include(i => i.Supplier).Include(m => m.ProductType).Get();

            foreach (InspectionRequestHeader o in InspectionRequests)
            {
                if (o.IsCompleted == false)
                {
                    rl.Add(o);
                }
            }
            return rl;
        }

        public IEnumerable<InspectionRequestHeader> GetPendingInspectionRequestHeaderList(int SupplierId)
        {
            List<InspectionRequestHeader> rl = new List<InspectionRequestHeader>();
            //var InspectionRequests = _unitOfWork.Repository<InspectionRequestHeader>().Query().Get().Where(i => i.SupplierId == SupplierId);


            var InspectionRequests = _unitOfWork.Repository<InspectionRequestHeader>().Query()
                        .Include(i => i.InspectionRequestLines)
                        .Include(i => i.InspectionRequestLines.Select(m => m.InspectionLines))
                        .Get().Where(i => i.SupplierId == SupplierId);

            foreach (InspectionRequestHeader o in InspectionRequests)
            {
                if (o.IsCompleted == false)
                {
                    rl.Add(o);
                }
            }
            return rl;
        }

        public IEnumerable<InspectionRequestHeader> GetAllInspectionRequestHeaderList()
        {
            var p = _unitOfWork.Repository<InspectionRequestHeader>().Query().Include(i => i.Supplier).Include(m => m.ProductType).Get();
            return p;
        }

        public IEnumerable<InspectionRequestHeader> GetAllInspectionRequestHeaderList(int SupplierId)
        {
            var p = _unitOfWork.Repository<InspectionRequestHeader>().Query().Get().Where(i => i.SupplierId == SupplierId);
            return p;
        }


        public InspectionRequestHeader Add(InspectionRequestHeader p)
        {
            _unitOfWork.Repository<InspectionRequestHeader>().Insert(p);
            return p;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<InspectionRequestHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<InspectionRequestHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public string CheckDuplicateDocNo(string DocId)
        {
            if (string.IsNullOrEmpty(DocId))
            {
                return "Field Empty";
            }
            int mCount = InspectionRequestHeaderRepository.Get()
                        .Where(i => i.DocNo == DocId)
                        .Count();
            if (mCount == 0)
            {
                return "Valid";
            }
            else
            {
                return "Already Exists";
            }

        }

        public string CheckDuplicateDocNo(string DocId, string HeaderId)
        {
            if (string.IsNullOrEmpty(DocId))
            {
                return "Field Empty";
            }
            int Id;
            bool stat = int.TryParse(HeaderId, out Id);

            int mCount = InspectionRequestHeaderRepository.Get()
                        .Where(i => i.InspectionRequestHeaderId != Id && i.DocNo == DocId)
                        .Count();
            if (mCount == 0)
            {
                return "Valid";
            }
            else
            {
                return "Already Exists";
            }

        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<InspectionRequestHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }
    }
}
