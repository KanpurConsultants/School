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
    public interface IPurchaseTransportGRService : IDisposable
    {
        PurchaseWaybill Create(PurchaseWaybill pt);
        void Delete(int id);
        void Delete(PurchaseWaybill s);
        PurchaseWaybill Find(int? Id);
        void Update(PurchaseWaybill s);
        IEnumerable<PurchaseWaybill> GetPurchaseTransportGRList();
        PurchaseWaybill GetPurchaseTransportGR(int id);

    }

    public class PurchaseTransportGRService : IPurchaseTransportGRService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        public PurchaseTransportGRService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PurchaseWaybill Find(int? id)
        {
            return _unitOfWork.Repository<PurchaseWaybill>().Find(id);
        }

        public PurchaseWaybill GetPurchaseTransportGR(int id)
        {
            return _unitOfWork.Repository<PurchaseWaybill>().Query().Get().Where(m => m.PurchaseWaybillId == id).FirstOrDefault();
        }

        public PurchaseWaybill Create(PurchaseWaybill s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<PurchaseWaybill>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<PurchaseWaybill>().Delete(id);
        }

        public void Delete(PurchaseWaybill s)
        {
            _unitOfWork.Repository<PurchaseWaybill>().Delete(s);
        }

        public void Update(PurchaseWaybill s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<PurchaseWaybill>().Update(s);
        }


        public IEnumerable<PurchaseWaybill> GetPurchaseTransportGRList()
        {
            var pt = _unitOfWork.Repository<PurchaseWaybill>().Query().Get();

            return pt;
        }

      

        public void Dispose()
        {
        }

    }
}
