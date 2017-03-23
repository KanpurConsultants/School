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
    public interface IInspectionLineService : IDisposable
    {
        InspectionLine Create(InspectionLine IL);
        void Delete(int p);
        void Delete(InspectionLine IL);
        InspectionLine GetInspectionLine(int p);
        InspectionLine Find(int p);
        void Update(InspectionLine IL);
        InspectionLine Add(InspectionLine IL);
        IEnumerable<InspectionLine> GetInspectionLineList();
        IEnumerable<InspectionLine> GetInspectionLineList(int InspectionHeaderId);

        InspectionLine GetInspectionLineForRequestId(int RequestId);
    }

    public class InspectionLineService : IInspectionLineService
    {
        ApplicationDbContext db =new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<InspectionLine> _InspectionLineRepository;
        RepositoryQuery<InspectionLine> InspectionLineRepository;
        public InspectionLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _InspectionLineRepository = new Repository<InspectionLine>(db);
            InspectionLineRepository = new RepositoryQuery<InspectionLine>(_InspectionLineRepository);
        }

       
        public InspectionLine Create(InspectionLine IL)
        {
            IL.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<InspectionLine>().Insert(IL);
            return IL;
        }

        public void Delete(int p)
        {
            _unitOfWork.Repository<InspectionLine>().Delete(p);
        }

        public void Delete(InspectionLine IL)
        {
            _unitOfWork.Repository<InspectionLine>().Delete(IL);
        }

        public void Update(InspectionLine IL)
        {
            IL.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<InspectionLine>().Update(IL);
        }

        public InspectionLine GetInspectionLine(int p)
        {
            return _unitOfWork.Repository<InspectionLine>().Query().Include(m => m.InspectionHeader).Get().Where(m => m.InspectionLineId == p).FirstOrDefault();
        }

        public InspectionLine Find(int p)
        {
            return _unitOfWork.Repository<InspectionLine>().Find(p);
        }

        public InspectionLine Add(InspectionLine IL)
        {
            _unitOfWork.Repository<InspectionLine>().Insert(IL);
            return IL;
        }

        public IEnumerable<InspectionLine> GetInspectionLineList()
        {
           return _unitOfWork.Repository<InspectionLine>().Query().Include(m => m.InspectionHeader).Get();

        }

        public IEnumerable<InspectionLine> GetInspectionLineList(int InspectionHeaderId)
        {
            return _unitOfWork.Repository<InspectionLine>().Query()
                .Include(m => m.InspectionHeader)
                .Include(m=>m.InspectionRequestLine)
                .Include(m=>m.InspectionRequestLine.PurchaseOrderLine)
                .Include(m=>m.InspectionRequestLine.PurchaseOrderLine.Product)
                .Include(m=>m.InspectionRequestLine.PurchaseOrderLine.PurchaseOrderHeader)
                .Include(m=>m.InspectionRequestLine.InspectionRequestHeader)
                .Get().Where(m => m.InspectionHeaderId == InspectionHeaderId);
        }

        public InspectionLine GetInspectionLineForRequestId(int requestid)
        {
            return _unitOfWork.Repository<InspectionLine>().Query().Get().Where(m => m.InspectionRequestLineId == requestid).FirstOrDefault();
        }
        public void Dispose()
        {
        }
    }
}
