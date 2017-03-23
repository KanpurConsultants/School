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
    public interface IInspectionRequestLineService : IDisposable
    {
        InspectionRequestLine Create(InspectionRequestLine p);
        void Delete(int id);
        void Delete(InspectionRequestLine p);
        InspectionRequestLine GetInspectionRequestLine(int p);
        IEnumerable<InspectionRequestLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(InspectionRequestLine p);
        InspectionRequestLine Add(InspectionRequestLine p);
        IEnumerable<InspectionRequestLine> GetInspectionRequestLineList();
        IEnumerable<InspectionRequestLine> GetInspectionRequestLineListForHeader(int HeaderId);
        IEnumerable<InspectionRequestLine> GetInspectionRequestLineList(int ProductId, int PurchaseOrderLineId);
        Task<IEquatable<InspectionRequestLine>> GetAsync();
        Task<InspectionRequestLine> FindAsync(int id);        
    }

    public class InspectionRequestLineService : IInspectionRequestLineService
    {
        ApplicationDbContext db =new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<InspectionRequestLine> _InspectionRequestLineRepository;
        RepositoryQuery<InspectionRequestLine> InspectionRequestLineRepository;
        public InspectionRequestLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _InspectionRequestLineRepository = new Repository<InspectionRequestLine>(db);
            InspectionRequestLineRepository = new RepositoryQuery<InspectionRequestLine>(_InspectionRequestLineRepository);
        }

        public InspectionRequestLine GetInspectionRequestLine(int pId) 
        {
            return InspectionRequestLineRepository                
                .Include(i => i.PurchaseOrderLine).Include(i => i.PurchaseOrderLine.Product).Include(i => i.PurchaseOrderLine.PurchaseOrderHeader).Include(i => i.InspectionRequestHeader)
                .Get().Where(i => i.InspectionRequestLineId == pId).FirstOrDefault();
           //return _unitOfWork.Repository<SalesOrder>().Find(soId);
        }

        public InspectionRequestLine Find(int id)
        {
            return _unitOfWork.Repository<InspectionRequestLine>().Find(id);
        }


        public InspectionRequestLine Create(InspectionRequestLine p)
        {
            p.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<InspectionRequestLine>().Insert(p);
            return p;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<InspectionRequestLine>().Delete(id);
        }

        public void Delete(InspectionRequestLine p)
        {
            _unitOfWork.Repository<InspectionRequestLine>().Delete(p);
        }

        public void Update(InspectionRequestLine p)
        {
            p.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<InspectionRequestLine>().Update(p);
        }

        public IEnumerable<InspectionRequestLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<InspectionRequestLine>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.InspectionRequestLineId))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }


        public IEnumerable<InspectionRequestLine> GetInspectionRequestLineList()
        {
            var p = _unitOfWork.Repository<InspectionRequestLine>().Query().Get();           
            
            return p;
        }

        public IEnumerable<InspectionRequestLine> GetInspectionRequestLineListForHeader(int HeaderId)
        {
            var p = _unitOfWork.Repository<InspectionRequestLine>().Query()
                .Include(i => i.PurchaseOrderLine)                
                .Include(i => i.PurchaseOrderLine.PurchaseOrderHeader)
                .Include(i=>i.InspectionLines)
                .Include(m=>m.InspectionRequestHeader)
                                    .Get().Where(i => i.InspectionRequestHeaderId == HeaderId);
            return p;
        }

        public IEnumerable<InspectionRequestLine> GetInspectionRequestLineList(int prodId,int POLineId)
        {
            return _unitOfWork.Repository<InspectionRequestLine>().Query().Include(i=>i.PurchaseOrderLine).Get().Where(i => i.PurchaseOrderLineId == POLineId && i.PurchaseOrderLine.ProductId == prodId);
        }


        public InspectionRequestLine Add(InspectionRequestLine p)
        {
            _unitOfWork.Repository<InspectionRequestLine>().Insert(p);
            return p;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<InspectionRequestLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<InspectionRequestLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }


    }
}
