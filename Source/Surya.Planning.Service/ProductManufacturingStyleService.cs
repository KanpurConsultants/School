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
    public interface IProductManufacturingStyleService : IDisposable
    {
        ProductStyle Create(ProductStyle pt);
        void Delete(int id);
        void Delete(ProductStyle pt);
        ProductStyle GetProductManufacturingStyle(int ptId);
        IEnumerable<ProductStyle> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(ProductStyle pt);
        ProductStyle Add(ProductStyle pt);
        IEnumerable<ProductStyle> GetProductManufacturingStyleList();

        // IEnumerable<ProductManufacturingStyle> GetProductManufacturingStyleList(int buyerId);
        Task<IEquatable<ProductStyle>> GetAsync();
        Task<ProductStyle> FindAsync(int id);
    }

    public class ProductManufacturingStyleService : IProductManufacturingStyleService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<ProductStyle> _ProductManufacturingStyleRepository;
        RepositoryQuery<ProductStyle> ProductManufacturingStyleRepository;
        public ProductManufacturingStyleService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ProductManufacturingStyleRepository = new Repository<ProductStyle>(db);
            ProductManufacturingStyleRepository = new RepositoryQuery<ProductStyle>(_ProductManufacturingStyleRepository);
        }

        public ProductStyle GetProductManufacturingStyle(int pt)
        {
            //return ProductManufacturingStyleRepository.Include(r => r.SalesOrder).Get().Where(i => i.SalesOrderDetailId == pt).FirstOrDefault();
            return _unitOfWork.Repository<ProductStyle>().Find(pt);
        }

        public ProductStyle Create(ProductStyle pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ProductStyle>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<ProductStyle>().Delete(id);
        }

        public void Delete(ProductStyle pt)
        {
            _unitOfWork.Repository<ProductStyle>().Delete(pt);
        }

        public void Update(ProductStyle pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ProductStyle>().Update(pt);
        }

        public IEnumerable<ProductStyle> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<ProductStyle>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.ProductStyleName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<ProductStyle> GetProductManufacturingStyleList()
        {
            var pt = _unitOfWork.Repository<ProductStyle>().Query().Get();

            return pt;
        }

        public ProductStyle Add(ProductStyle pt)
        {
            _unitOfWork.Repository<ProductStyle>().Insert(pt);
            return pt;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<ProductStyle>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductStyle> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
