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
    public interface IProductContentLineService : IDisposable
    {
        ProductContentLine Create(ProductContentLine s);
        void Delete(int id);
        void Delete(ProductContentLine s);
        ProductContentLine GetProductContentLine(int id);
        ProductContentLine Find(int id);
        void Update(ProductContentLine s);
        IEnumerable<ProductContentLineIndexViewModel> GetProductContentLineListForIndex(int id);
    }

    public class ProductContentLineService : IProductContentLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public ProductContentLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ProductContentLineIndexViewModel> GetProductContentLineListForIndex(int id)
        {
            return (from p in db.ProductContentLine
                    where p.ProductContentHeaderId == id
                    select new ProductContentLineIndexViewModel
                    {
                        ProductContentLineId=p.ProductContentLineId,
                        ProductGroupId=p.ProductGroupId,
                         ProductGroupName=p.ProductGroup.ProductGroupName,
                    }
                        );
        }

        public ProductContentLine Create(ProductContentLine S)
        {
            S.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ProductContentLine>().Insert(S);
            return S;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<ProductContentLine>().Delete(id);
        }

        public void Delete(ProductContentLine s)
        {
            _unitOfWork.Repository<ProductContentLine>().Delete(s);
        }

        public void Update(ProductContentLine s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ProductContentLine>().Update(s);
        }

        public ProductContentLine GetProductContentLine(int id)
        {
            return _unitOfWork.Repository<ProductContentLine>().Query().Get().Where(m => m.ProductContentLineId == id).FirstOrDefault();

        }
   
        public ProductContentLine Find(int id)
        {
            return _unitOfWork.Repository<ProductContentLine>().Find(id);
        }

        public void Dispose()
        {
        }
    }
}
