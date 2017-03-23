using System.Collections.Generic;
using System.Linq;
using Surya.India.Data;
using Surya.India.Data.Infrastructure;
using Surya.India.Model.Models;
using Surya.India.Model.ViewModels;
using Surya.India.Core.Common;
using System;
using Surya.India.Model;
using System.Threading.Tasks;
using Surya.India.Data.Models;
using Surya.India.Model.ViewModel;


namespace Surya.India.Service
{
    public interface IProductCustomGroupLineService : IDisposable
    {
        ProductCustomGroupLine Create(ProductCustomGroupLine s);
        void Delete(int id);
        void Delete(ProductCustomGroupLine s);
        ProductCustomGroupLine GetProductCustomGroupLine(int id);
        ProductCustomGroupLine Find(int id);
        void Update(ProductCustomGroupLine s);
        IEnumerable<ProductCustomGroupLineIndexViewModel> GetProductCustomGroupLineListForIndex(int id);
    }

    public class ProductCustomGroupLineService : IProductCustomGroupLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public ProductCustomGroupLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ProductCustomGroupLineIndexViewModel> GetProductCustomGroupLineListForIndex(int id)
        {
            return (from p in db.ProductCustomGroupLine
                    where p.ProductCustomGroupHeaderId == id
                    select new ProductCustomGroupLineIndexViewModel
                    {
                        ProductCustomGroupLineId=p.ProductCustomGroupLineId,
                        ProductId=p.ProductId,
                        ProductName=p.Product.ProductName,
                        Qty=p.Qty
                    }
                        );
        }

        public ProductCustomGroupLine Create(ProductCustomGroupLine S)
        {
            S.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ProductCustomGroupLine>().Insert(S);
            return S;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<ProductCustomGroupLine>().Delete(id);
        }

        public void Delete(ProductCustomGroupLine s)
        {
            _unitOfWork.Repository<ProductCustomGroupLine>().Delete(s);
        }

        public void Update(ProductCustomGroupLine s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ProductCustomGroupLine>().Update(s);
        }

        public ProductCustomGroupLine GetProductCustomGroupLine(int id)
        {
            return _unitOfWork.Repository<ProductCustomGroupLine>().Query().Get().Where(m => m.ProductCustomGroupLineId == id).FirstOrDefault();

        }
   
        public ProductCustomGroupLine Find(int id)
        {
            return _unitOfWork.Repository<ProductCustomGroupLine>().Find(id);
        }

        public void Dispose()
        {
        }
    }
}
