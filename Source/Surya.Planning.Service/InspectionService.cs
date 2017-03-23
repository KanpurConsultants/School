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
    public interface IInspectionService : IDisposable
    {
        InspectionHeader Create(InspectionHeader Ih);
        void Delete(int p);
        void Delete(InspectionHeader Ih);
        InspectionHeader GetInspectionHeader(int p);
        InspectionHeader Find(int p);
        void Update(InspectionHeader Ih);
        InspectionHeader Add(InspectionHeader Ih);
        IEnumerable<InspectionHeader> GetInspectionHeaderList();
        string GetMaxDocNo(int DocTypeId);
    }

    public class InspectionService : IInspectionService
    {
        ApplicationDbContext db =new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<InspectionHeader> _InspectionHeaderRepository;
        RepositoryQuery<InspectionHeader> InspectionHeaderRepository;
        public InspectionService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _InspectionHeaderRepository = new Repository<InspectionHeader>(db);
            InspectionHeaderRepository = new RepositoryQuery<InspectionHeader>(_InspectionHeaderRepository);
        }

       
        public InspectionHeader Create(InspectionHeader Ih)
        {
            Ih.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<InspectionHeader>().Insert(Ih);
            return Ih;
        }

        public void Delete(int p)
        {
            _unitOfWork.Repository<InspectionHeader>().Delete(p);
        }

        public void Delete(InspectionHeader Ih)
        {
            _unitOfWork.Repository<InspectionHeader>().Delete(Ih);
        }

        public void Update(InspectionHeader Ih)
        {
            Ih.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<InspectionHeader>().Update(Ih);
        }

        public InspectionHeader GetInspectionHeader(int p)
        {
            return _unitOfWork.Repository<InspectionHeader>().Query().Include(m => m.InspectionLines).Get().Where(m => m.InspectionHeaderId == p).FirstOrDefault();
        }

        public InspectionHeader Find(int p)
        {
            return _unitOfWork.Repository<InspectionHeader>().Find(p);
        }

        public InspectionHeader Add(InspectionHeader Ih)
        {
            _unitOfWork.Repository<InspectionHeader>().Insert(Ih);
            return Ih;
        }

        public IEnumerable<InspectionHeader> GetInspectionHeaderList()
        {
            return _unitOfWork.Repository<InspectionHeader>().Query().Include(M=>M.Supplier).Get();
        }
        public void Dispose()
        {
        }


        public string GetMaxDocNo(int DocTypeId)
        {
            int x;

            var maxVal = db.InspectionHeader.Select(z => z.DocNo).DefaultIfEmpty().ToList()
                        .Select(sx => int.TryParse(sx, out x) ? x : 0).Max();

            return (maxVal + 1).ToString();
            /*
            var temp = db.PurchaseInvoiceHeader.Where(m => m.DocTypeId == DocTypeId).Select(m=>m.DocNo).Max();
            //doc no=> a,b,c,d, 1,2,3,
            var a = int.TryParse(temp, out x);

            return (from m in db.PurchaseInvoiceHeader where int.TryParse(m.DocNo, out x) && m.DocTypeId == DocTypeId select m.DocNo).Max ();
             * */
        }

    }
}
