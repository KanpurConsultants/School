﻿using System.Collections.Generic;
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
    public interface IProductTypeService : IDisposable
    {
        ProductType Create(ProductType pt);
        void Delete(int id);
        void Delete(ProductType pt);
        ProductType GetProductTypeByName(string name);
        ProductType GetProductTypeWithAttributes(int id);        
        ProductType Find(string Name);
        ProductType Find(int id);
        void Update(ProductType pt);
        ProductType Add(ProductType pt);
        IEnumerable<ProductType> GetProductTypeList();
        IEnumerable<ProductType> GetFinishiedProductTypeList();
        IEnumerable<ProductType> GetProductTypeListForNature(int id);
        ProductType GetTypeForProduct(int id);//ProductId

        // IEnumerable<ProductType> GetProductTypeList(int buyerId);
        Task<IEquatable<ProductType>> GetAsync();
        Task<ProductType> FindAsync(int id);

        int NextId(int id);
        int PrevId(int id);
    }

    public class ProductTypeService : IProductTypeService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<ProductType> _ProductTypeRepository;
        RepositoryQuery<ProductType> productTypeRepository;
        public ProductTypeService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ProductTypeRepository = new Repository<ProductType>(db);
            productTypeRepository = new RepositoryQuery<ProductType>(_ProductTypeRepository);
        }

        public ProductType GetProductTypeByName(string name)
        {
            return (from p in db.ProductTypes
                    where p.ProductTypeName == name 
                    select p
                        ).FirstOrDefault();
        }

        public ProductType Find(string Name)
        {
            return productTypeRepository.Get().Where(i => i.ProductTypeName == Name).FirstOrDefault();
        }

        public ProductType GetProductTypeWithAttributes(int id)
        {
            return productTypeRepository.Get().Where(i => i.ProductTypeId == id).FirstOrDefault(); 
        }


        public ProductType Find(int id)
        {
            return _unitOfWork.Repository<ProductType>().Find(id);
        }

        public ProductType Create(ProductType pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ProductType>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<ProductType>().Delete(id);
        }

        public void Delete(ProductType pt)
        {
            pt.ObjectState = ObjectState.Deleted;
            _unitOfWork.Repository<ProductType>().Delete(pt);
        }
        public ProductType GetTypeForProduct(int id)//ProductId
        {
            return (from p in db.Product
                    join t in db.ProductGroups on p.ProductGroupId equals t.ProductGroupId into table
                    from tab in table.DefaultIfEmpty()
                    join t1 in db.ProductTypes on tab.ProductTypeId equals t1.ProductTypeId into table2
                    from tab2 in table2.DefaultIfEmpty()
                    where p.ProductId == id
                    select tab2
                ).FirstOrDefault();
        }

        public void Update(ProductType pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ProductType>().Update(pt);
        }


        public IEnumerable<ProductType> GetProductTypeList()
        {
            var pt = _unitOfWork.Repository<ProductType>().Query().Get();

            return pt;
        }
        public IEnumerable<ProductType> GetProductTypeListForNature(int id)
        {
            return (from p in db.ProductTypes
                    where p.ProductNatureId == id
                    select p
                        ).ToList();
        }

        public IEnumerable<ProductType> GetFinishiedProductTypeList()
        {
            int x = new ProductNatureService(_unitOfWork).GetProductNatureByName(ProductTypeConstants.FinishedMaterial).ProductNatureId;
            var pt = _unitOfWork.Repository<ProductType>().Query().Get().Where(m=>m.ProductNatureId==x);

            return pt;
        }
        public IEnumerable<ProductType> GetFinishedProductTypeListWithNoCustomUI()
        {
            int x = new ProductNatureService(_unitOfWork).GetProductNatureByName(ProductTypeConstants.FinishedMaterial).ProductNatureId;
            var pt = _unitOfWork.Repository<ProductType>().Query().Get().Where(m => m.ProductNatureId == x && m.IsCustomUI==null);

            return pt;
        }

        public IEnumerable<ProductType> GetProductTypeListForGroup()
        {            
            var pt = (from p in db.ProductTypes
                          where p.IsCustomUI==null
                          select p
                          );

            return pt;
        }
        public IEnumerable<ProductType> GetProductTypeListForCategory()
        {
            var pt = (from p in db.ProductTypes
                      where p.IsCustomUI == null && p.ProductNature.ProductNatureName==ProductNatureConstants.FinishedMaterial
                      select p
                          );

            return pt;
        }

        public IEnumerable<ProductType> GetProductTypeListForMaterial(int id)
        {
            var pt = (from p in db.ProductTypes
                      where p.IsCustomUI == null && p.ProductNatureId==id
                      select p
                          );

            return pt;
        }

        public IEnumerable<ProductType> GetRawAndOtherMaterialProductTypes()
        {

            string RawMaterial = ProductNatureConstants.Rawmaterial;
            string OtherMaterial = ProductNatureConstants.OtherMaterial;

            var pt = (from p in db.ProductTypes
                      where p.ProductNature.ProductNatureName == RawMaterial || p.ProductNature.ProductNatureName == OtherMaterial
                      select p
                        );

            return pt;
        }


        public ProductType Add(ProductType pt)
        {
            _unitOfWork.Repository<ProductType>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.ProductTypes
                        orderby p.ProductTypeName
                        select p.ProductTypeId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.ProductTypes
                        orderby p.ProductTypeName
                        select p.ProductTypeId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.ProductTypes
                        orderby p.ProductTypeName
                        select p.ProductTypeId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.ProductTypes
                        orderby p.ProductTypeName
                        select p.ProductTypeId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public void Dispose()
        {
        }


        public Task<IEquatable<ProductType>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductType> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}