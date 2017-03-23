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
using Surya.India;
using Surya.India.Model.ViewModels;
using System.Configuration;
using System.Data.SqlClient;

namespace Surya.India.Service
{
    public interface IProductService : IDisposable
    {
        Product Create(Product p);
        void Delete(int id);
        void Delete(Product p);
        ProductViewModel GetProduct(int p);
        ProductViewModel GetFinishedProduct(int p);
        IEnumerable<Product> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Product p);
        Product Add(Product p);
        IQueryable<Product> GetProductList();

        IEnumerable<Product> GetProductList(int prodyctTypeId);
        Task<IEquatable<Product>> GetAsync();
        Task<Product> FindAsync(int id);
        Product Find(string ProductName);
        Product FindProduct(string ProductName);
        Product Find(int id);
        IEnumerable<Product> GetAccessoryList();
        IQueryable<ProductIndexViewModel> GetProductListForIndex(int ProductTypeId);
        IQueryable<ProductIndexViewModel> GetProductListForIndex();
        IQueryable<ProductViewModel> GetProductListForGroup(int id);
        IQueryable<ProductIndexViewModel> GetProductListForMaterial(int ProductTypeId);
        MaterialViewModel GetMaterialProduct(int id);
        int NextId(int id);
        int PrevId(int id);
        int NextMaterialId(int id, int nid);
        int PrevMaterialId(int id, int nid);
        ProductPrevProcess FGetProductPrevProcess(int ProductId, int ProcessId);
    }



    public class ProductService : IProductService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public ProductService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public MaterialViewModel GetMaterialProduct(int id)//ProductCategoryId
        {
            return (from p in db.Product
                    where p.ProductId == id
                    select new MaterialViewModel
                    {
                        ProductGroupName = p.ProductGroup.ProductGroupName,
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        ProductCode = p.ProductCode,
                        ProductGroupId = (int)p.ProductGroupId,
                        StandardCost = p.StandardCost,
                        Tags = p.Tags,
                        UnitId = p.UnitId,
                        IsActive = p.IsActive,
                        SalesTaxGroupProductId = p.SalesTaxGroupProductId,
                        ImageFileName = p.ImageFileName,
                        ImageFolderName = p.ImageFolderName
                    }
                        ).FirstOrDefault();
        }

        public IQueryable<ProductViewModel> GetProductListForGroup(int id)
        {
            return (from p in db.Product
                    where p.ProductGroupId == id
                    select new ProductViewModel
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                    });
        }


        public ProductViewModel GetProduct(int pId)
        {
            var temp = from p in db.Product
                       where p.ProductId == pId
                       select new ProductViewModel
                       {

                           ProductId = p.ProductId,
                           ProductDescription = p.ProductDescription,
                           StandardCost = p.StandardCost,
                           UnitId = p.UnitId,
                           UnitName = p.Unit.UnitName,
                           ProductName = p.ProductName,
                           ProductCode = p.ProductCode,
                           SalesTaxGroupProductId = p.SalesTaxGroupProductId,
                           DivisionId = p.DivisionId,
                           DivisionName = p.Division.DivisionName,
                           ProductGroupId = (int)p.ProductGroupId,
                           ProductGroupName = p.ProductGroup.ProductGroupName,
                           IsActive = p.IsActive,
                           IsSystemDefine = p.IsSystemDefine,
                           StandardWeight = p.StandardWeight,
                           ProductSpecification = p.ProductSpecification,
                           //ColourId = p.ColourId,
                           //DescriptionOfGoodsId = p.DescriptionOfGoodsId,
                           //OriginCountryId = p.OriginCountryId
                       };



            return (temp).FirstOrDefault();


        }

        public ProductViewModel GetFinishedProduct(int pId)
        {
            var temp = from p in db.Product
                       join t in db.FinishedProduct on p.ProductId equals t.ProductId into table
                       from tab in table.DefaultIfEmpty()
                       where p.ProductId == pId
                       select new ProductViewModel
                       {
                           ProductCategoryId = tab.ProductCategoryId ?? 0,
                           ProductCollectionId = tab.ProductCollectionId,
                           ProductQualityId = tab.ProductQualityId,
                           ProductDesignId = tab.ProductDesignId,
                           ProductId = p.ProductId,
                           ProductDescription = p.ProductDescription,
                           StandardCost = p.StandardCost,
                           UnitId = p.UnitId,
                           UnitName = p.Unit.UnitName,
                           ProductName = p.ProductName,
                           ProductCode = p.ProductCode,
                           SalesTaxGroupProductId = p.SalesTaxGroupProductId,
                           DivisionId = p.DivisionId,
                           DivisionName = p.Division.DivisionName,
                           ProductGroupId = (int)p.ProductGroupId,
                           ProductGroupName = p.ProductGroup.ProductGroupName,
                           IsActive = p.IsActive,
                           IsSystemDefine = p.IsSystemDefine,
                           StandardWeight = p.StandardWeight,
                           ColourId = tab.ColourId,
                           DescriptionOfGoodsId = tab.DescriptionOfGoodsId,
                           ProductShapeId = tab.ProductShapeId,
                           OriginCountryId = tab.OriginCountryId,
                           TraceType = tab.TraceType,
                           MapType = tab.MapType,
                           MapScale = tab.MapScale

                       };


            return (temp).FirstOrDefault();


        }

        public Product Create(Product p)
        {
            p.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Product>().Insert(p);
            return p;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Product>().Delete(id);
        }

        public void Delete(Product p)
        {
            _unitOfWork.Repository<Product>().Delete(p);
        }

        public void Update(Product p)
        {
            p.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Product>().Update(p);
        }

        public IEnumerable<Product> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Product>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.ProductCode))
                .Filter(q => !string.IsNullOrEmpty(q.ProductName))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }


        public IQueryable<Product> GetProductList()
        {
            var p = _unitOfWork.Repository<Product>().Query().Get().OrderBy(m => m.ProductName);

            return p;

        }

        public IEnumerable<Product> GetAccessoryList()
        {
            var p = _unitOfWork.Repository<Product>().Query().Get();
            return p;
        }



        public Product Find(string ProductName)
        {

            Product p = _unitOfWork.Repository<Product>().Query().Get().Where(i => i.ProductName == ProductName).FirstOrDefault();

            return p;
        }

        public Product FindProduct(string ProductName)
        {

            //Product p = _unitOfWork.Repository<Product>().Query().Get().Where(i => i.ProductName == ProductName).FirstOrDefault();

            //return p;

            int ProductNatureRawMaterial = 1;
            int ProductNatureFinishedMaterial = 2;
            int ProductNatureOtherMaterial = 3;
            var temp = from P in db.Product
                       join PG in db.ProductGroups on P.ProductGroupId equals PG.ProductGroupId into tableProductGroup
                       from tabProductGroup in tableProductGroup.DefaultIfEmpty()
                       join PT in db.ProductTypes on tabProductGroup.ProductTypeId equals PT.ProductTypeId into tableTypeGroup
                       from tabProductType in tableTypeGroup.DefaultIfEmpty()
                       where P.ProductName == ProductName && ((int)tabProductType.ProductNatureId == ProductNatureRawMaterial || (int)tabProductType.ProductNatureId == ProductNatureFinishedMaterial || (int)tabProductType.ProductNatureId == ProductNatureOtherMaterial)
                       select P;

            return (temp).FirstOrDefault();

        }

        public Product Find(int id)
        {
            return _unitOfWork.Repository<Product>().Find(id);
        }


        public IEnumerable<Product> GetProductList(int productTypeId)
        {
            return _unitOfWork.Repository<Product>().Query().Include(i => i.ProductGroup).Get().OrderBy(m => m.ProductCode);
        }

        public IQueryable<ProductIndexViewModel> GetProductListForIndex(int ProductTypeId)
        {
            //Using LINQ
            var temp = (from p in db.Product
                        join p3 in db.ProductGroups on p.ProductGroupId equals p3.ProductGroupId
                        join p4 in db.Divisions on p.DivisionId equals p4.DivisionId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join p5 in db.Units on p.UnitId equals p5.UnitId into table3
                        from tab3 in table3.DefaultIfEmpty()
                        orderby p.ProductName
                        select new ProductIndexViewModel
                        {
                            ProductId = p.ProductId,
                            ProductCode = p.ProductCode,
                            ProductName = p.ProductName,
                            ProductGroupName = p3.ProductGroupName,
                            UnitName = tab3.UnitName,
                            DivisionName = tab2.DivisionName,
                        });
            return temp;
        }

        public IQueryable<ProductIndexViewModel> GetProductListForIndex()
        {
            //Using LINQ
            var temp = (from p in db.Product
                        join p3 in db.ProductGroups on p.ProductGroupId equals p3.ProductGroupId
                        join p4 in db.Divisions on p.DivisionId equals p4.DivisionId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join p5 in db.Units on p.UnitId equals p5.UnitId into table3
                        from tab3 in table3.DefaultIfEmpty()
                        orderby p.ProductName
                        select new ProductIndexViewModel
                        {
                            ProductId = p.ProductId,
                            ProductCode = p.ProductCode,
                            ProductName = p.ProductName,
                            ProductGroupName = p3.ProductGroupName,
                            UnitName = tab3.UnitName,
                            DivisionName = tab2.DivisionName,
                            IsActive = p.IsActive,
                            IsSystemDefine = p.IsSystemDefine

                        });
            return temp;
        }

        public IQueryable<ProductIndexViewModel> GetProductListForMaterial(int ProductTypeId)
        {

            var temp = (from p in db.Product
                        join t in db.ProductGroups on p.ProductGroupId equals t.ProductGroupId into table
                        from tab in table.DefaultIfEmpty()
                        where tab.ProductTypeId == ProductTypeId
                        orderby p.ProductName
                        select new ProductIndexViewModel
                        {
                            ProductId = p.ProductId,
                            ProductCode = p.ProductCode,
                            ProductName = p.ProductName,
                            ProductGroupName = tab.ProductGroupName,
                            IsActive = p.IsActive,
                            IsSystemDefine = p.IsSystemDefine

                        });
            return temp;

        }

        public Product Add(Product p)
        {
            _unitOfWork.Repository<Product>().Add(p);
            return p;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Product>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product> FindAsync(int id)
        {
            throw new NotImplementedException();
        }




        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Product
                        orderby p.ProductName
                        select p.ProductId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Product
                        orderby p.ProductName
                        select p.ProductId).FirstOrDefault();
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

                temp = (from p in db.Product
                        orderby p.ProductName
                        select p.ProductId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Product
                        orderby p.ProductName
                        select p.ProductId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public int NextMaterialId(int id, int ProductGroupId)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Product
                        where p.ProductGroupId == ProductGroupId
                        orderby p.ProductName
                        select p.ProductId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Product
                        where p.ProductGroupId == ProductGroupId
                        orderby p.ProductName
                        select p.ProductId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevMaterialId(int id, int ProductGroupId)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.Product
                        where p.ProductGroupId == ProductGroupId
                        orderby p.ProductName
                        select p.ProductId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Product
                        where p.ProductGroupId == ProductGroupId
                        orderby p.ProductName
                        select p.ProductId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        //public IQueryable<ProductPrevProcess> FGetProductPrevProcess(int ProductId, int ProcessId)
        //{


        //    var ProcessSequence = (from L in db.ProcessSequenceLine
        //                           where L.ProcessSequenceHeaderId == (from P in db.FinishedProduct where P.ProductId == ProductId select new { ProcessSequenceHeaderId = P.ProcessSequenceHeaderId }).FirstOrDefault().ProcessSequenceHeaderId
        //                           && L.ProcessId == ProcessId
        //                           select new
        //                           {
        //                               Sequence = L.Sequence
        //                           }).FirstOrDefault();


        //    if (ProcessSequence != null)
        //    {
        //        IQueryable<ProductPrevProcess> productprevprocess = (from P in db.FinishedProduct
        //                                                             join Ps in db.ProcessSequenceLine on P.ProcessSequenceHeaderId equals Ps.ProcessSequenceHeaderId into ProcessSequenceLineTable
        //                                                             from ProcessSequenceLineTab in ProcessSequenceLineTable.DefaultIfEmpty()
        //                                                             join Pr in db.Process on ProcessSequenceLineTab.ProcessId equals Pr.ProcessId into ProcessTable
        //                                                             from ProcessTab in ProcessTable.DefaultIfEmpty()
        //                                                             where P.ProductId == ProductId && ProcessSequenceLineTab.Sequence == ProcessSequence.Sequence
        //                                                             select new ProductPrevProcess
        //                                                             {
        //                                                                 ProductId = P.ProductId,
        //                                                                 PrevProcessId = ProcessSequenceLineTab.ProcessId,
        //                                                                 PrevProcessName = ProcessTab.ProcessName
        //                                                             });

        //        return productprevprocess;

        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}




        public ProductPrevProcess FGetProductPrevProcess(int ProductId, int ProcessId)
        {
            SqlParameter SqlParameterProductId = new SqlParameter("@ProductId", ProductId);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);

            ProductPrevProcess ProductPrevProcess = db.Database.SqlQuery<ProductPrevProcess>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".ProcGetPrevProcessForProduct @ProductId, @ProcessId", SqlParameterProductId, SqlParameterProcessId).FirstOrDefault();

            return ProductPrevProcess;
        }

        public void CreateTraceMap(int ProductGroupId)
        {
            string ColourWaysName = "";
            string SizeName = "";
            string TraceName = "";
            string MapName = "";
            int i = -1;

            var ProductList = (from P in db.FinishedProduct
                               join Vrs in db.ViewRugSize on P.ProductId equals Vrs.ProductId into ViewRugSizeTable
                               from ViewRugSizeTab in ViewRugSizeTable.DefaultIfEmpty()
                               where P.ProductName == TraceName
                               select new
                               {
                                   ProductId = P.ProductId,
                                   ProductName = P.ProductName,
                                   ProductDesignId = P.ProductDesignId,
                                   StandardSizeId = ViewRugSizeTab.StandardSizeID,
                                   StencilSizeId = ViewRugSizeTab.StencilSizeId,
                                   StencilAreaSqFeet = ViewRugSizeTab.StencilSizeArea,
                                   MapAreaSqFeet = ViewRugSizeTab.MapSizeArea,
                                   CreatedBy = P.CreatedBy,
                               }).ToList();

            foreach (var item in ProductList)
            {
                ColourWaysName = (from H in db.ProductDesigns where H.ProductDesignId == item.ProductDesignId select H).FirstOrDefault().ProductDesignName;
                SizeName = (from H in db.Size where H.SizeId == item.StandardSizeId select H).FirstOrDefault().SizeName;
                TraceName = ColourWaysName + "-Trace-" + SizeName;
                MapName = item.ProductName + "-Map-";

                var temp = (from P in db.Product where P.ProductName == TraceName select P).FirstOrDefault();

                if (temp == null)
                {
                    Product ProductTrace = new Product();
                    ProductTrace.ProductId = i;
                    ProductTrace.ProductName = TraceName;
                    if (TraceName.Length <= 20)
                    {
                        ProductTrace.ProductCode = TraceName;
                    }
                    else
                    {
                        ProductTrace.ProductCode = TraceName.Substring(0, 20);
                    }

                    ProductTrace.ProductGroupId = new ProductGroupService(_unitOfWork).Find(ProductGroupConstants.Trace).ProductGroupId;
                    ProductTrace.UnitId = UnitConstants.Pieces;
                    ProductTrace.IsActive = true;
                    ProductTrace.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    ProductTrace.CreatedDate = DateTime.Now;
                    ProductTrace.ModifiedDate = DateTime.Now;
                    ProductTrace.CreatedBy = item.CreatedBy;
                    ProductTrace.ModifiedBy = item.CreatedBy;
                    ProductTrace.IsActive = true;

                    ProductTrace.ObjectState = Model.ObjectState.Added;
                    new ProductService(_unitOfWork).Create(ProductTrace);


                    UnitConversion UnitConvTrace = new UnitConversion();
                    UnitConvTrace.CreatedBy = item.CreatedBy;
                    UnitConvTrace.CreatedDate = DateTime.Now;
                    UnitConvTrace.ModifiedBy = item.CreatedBy;
                    UnitConvTrace.ModifiedDate = DateTime.Now;
                    UnitConvTrace.ProductId = ProductTrace.ProductId;
                    UnitConvTrace.FromQty = 1;
                    UnitConvTrace.FromUnitId = UnitConstants.Pieces;
                    UnitConvTrace.ToUnitId = UnitConstants.SqFeet;
                    UnitConvTrace.UnitConversionForId = (byte)UnitConversionFors.Standard;
                    UnitConvTrace.ToQty = item.StencilAreaSqFeet ?? 0;
                    new UnitConversionService(_unitOfWork).Create(UnitConvTrace);



                    

                    Product ProductMap = new Product();
                    ProductMap.ProductId = i - 1;
                    ProductMap.ProductName = MapName;
                    if (MapName.Length <= 20)
                    {
                        ProductMap.ProductCode = MapName;
                    }
                    else
                    {
                        ProductMap.ProductCode = MapName.Substring(0, 20);
                    }
                    ProductMap.ProductGroupId = new ProductGroupService(_unitOfWork).Find(ProductGroupConstants.Map).ProductGroupId;
                    ProductMap.UnitId = UnitConstants.Pieces;
                    ProductMap.IsActive = true;
                    ProductMap.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    ProductMap.CreatedDate = DateTime.Now;
                    ProductMap.ModifiedDate = DateTime.Now;
                    ProductMap.CreatedBy = item.CreatedBy;
                    ProductMap.ModifiedBy = item.CreatedBy;
                    ProductMap.IsActive = true;
                    ProductMap.ObjectState = Model.ObjectState.Added;
                    new ProductService(_unitOfWork).Create(ProductMap);



                    UnitConversion UnitConvMap = new UnitConversion();
                    UnitConvMap.CreatedBy = item.CreatedBy;
                    UnitConvMap.CreatedDate = DateTime.Now;
                    UnitConvMap.ModifiedBy = item.CreatedBy;
                    UnitConvMap.ModifiedDate = DateTime.Now;
                    UnitConvMap.ProductId = ProductMap.ProductId;
                    UnitConvMap.FromQty = 1;
                    UnitConvMap.FromUnitId = UnitConstants.Pieces;
                    UnitConvMap.ToUnitId = UnitConstants.SqFeet;
                    UnitConvMap.UnitConversionForId = (byte)UnitConversionFors.Standard;
                    UnitConvMap.ToQty = item.MapAreaSqFeet ?? 0;
                    new UnitConversionService(_unitOfWork).Create(UnitConvMap);



                }
            }
        }





    }

    public class ProductPrevProcess
    {
        public int ProcessId { get; set; }
    }
}
