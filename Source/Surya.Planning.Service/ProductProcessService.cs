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
    public interface IProductProcessService : IDisposable
    {
        ProductProcess Create(ProductProcess pt);
        void Delete(int id);
        void Delete(ProductProcess pt);
        ProductProcess GetProductProcess(int ptId);
        IEnumerable<ProductProcess> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(ProductProcess pt);
        ProductProcess Add(ProductProcess pt);
        IEnumerable<ProductProcess> GetProductProcessList();
        IEnumerable<ProductProcess> GetProductProcessList(int id);
        ProductProcess Find(int id);
        ProductProcess FindByProductProcess(int ProductId, int? ProcessId, int? Dimension1Id, int? Dimension2Id);
        
        Task<IEquatable<ProductProcess>> GetAsync();
        Task<ProductProcess> FindAsync(int id);
        IEnumerable<ProductProcess> GetProductProcessIdListByProductId(int ProductId);
    }

    public class ProductProcessService : IProductProcessService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<ProductProcess> _ProductProcessRepository;
        RepositoryQuery<ProductProcess> ProductProcessRepository;
        public ProductProcessService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ProductProcessRepository = new Repository<ProductProcess>(db);
            ProductProcessRepository = new RepositoryQuery<ProductProcess>(_ProductProcessRepository);
        }

        public ProductProcess GetProductProcess(int pt)
        {
            return ProductProcessRepository.Include(r => r.Product).Get().Where(i => i.ProductProcessId == pt).FirstOrDefault();
        }

        public ProductProcess Create(ProductProcess pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ProductProcess>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<ProductProcess>().Delete(id);
        }

        public void Delete(ProductProcess pt)
        {
            _unitOfWork.Repository<ProductProcess>().Delete(pt);
        }

        public void Update(ProductProcess pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ProductProcess>().Update(pt);
        }

        public IEnumerable<ProductProcess> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<ProductProcess>()
                .Query()
                //.OrderBy(q => q.OrderBy(c => c.Supplier ))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<ProductProcess> GetProductProcessList()
        {
            var pt = _unitOfWork.Repository<ProductProcess>().Query().Include(p => p.Product).Get();
            return pt;
        }

        public ProductProcess Add(ProductProcess pt)
        {
            _unitOfWork.Repository<ProductProcess>().Insert(pt);
            return pt;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<ProductProcess>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductProcess> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<ProductProcess> GetProductProcessList(int id)
        {
            var pt = _unitOfWork.Repository<ProductProcess>().Query().Include(m=>m.Product).Get().Where(m => m.ProductId == id).ToList();
            return pt;
        }
        public ProductProcess Find(int id)
        {
            return _unitOfWork.Repository<ProductProcess>().Find(id);
        }

        public ProductProcess FindByProductProcess(int ProductId, int? ProcessId, int? Dimension1Id, int? Dimension2Id)
        {
            var pt = _unitOfWork.Repository<ProductProcess>().Query().Get().Where(m => m.ProductId == ProductId && m.ProcessId == ProcessId && m.Dimension1Id == Dimension1Id && m.Dimension2Id == Dimension2Id).FirstOrDefault();
            return pt;
        }

        public IEnumerable<ProductProcess> GetProductProcessIdListByProductId(int ProductId)
        {
            var pt = _unitOfWork.Repository<ProductProcess>().Query().Get().Where(m => m.ProductId == ProductId).ToList();
            return pt;
        }

    }
}
