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
using Surya.India.Model.ViewModel;
using Surya.India.Model.ViewModels;

namespace Surya.India.Service
{
    public interface IProdOrderLineService : IDisposable
    {
        ProdOrderLine Create(ProdOrderLine pt);
        void Delete(int id);
        void Delete(ProdOrderLine pt);
        ProdOrderLine Find(int id);
        IEnumerable<ProdOrderLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(ProdOrderLine pt);
        ProdOrderLine Add(ProdOrderLine pt);
        IEnumerable<ProdOrderLine> GetProdOrderLineList();
        IEnumerable<ProdOrderLineViewModel> GetProdOrderLineListForIndex(int id);//HeaderId      
        ProdOrderLineViewModel GetProdOrderLine(int id);
        Task<IEquatable<ProdOrderLine>> GetAsync();
        Task<ProdOrderLine> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
        decimal GetProdOrderBalance(int? id);//ProdOrderLineId
        IEnumerable<ProdOrderLine> GetProdOrderLineForMaterialPlan(int id);
        IEnumerable<ProdOrderLineViewModel> GetProdOrderForFilters(ProdOrderLineFilterViewModel vm);
        IQueryable<ComboBoxResult> GetPendingMaterialPlanHelpList(int Id, string term);//PurchaseOrderHeaderId
        ProdOrderLineBalance GetLineDetail(int id);
        int GetMaxSr(int id);
        ProdOrderLineViewModel GetProdOrderForProdUid(int id);
    }

    public class ProdOrderLineService : IProdOrderLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<ProdOrderLine> _ProdOrderLineRepository;
        RepositoryQuery<ProdOrderLine> ProdOrderLineRepository;
        public ProdOrderLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ProdOrderLineRepository = new Repository<ProdOrderLine>(db);
            ProdOrderLineRepository = new RepositoryQuery<ProdOrderLine>(_ProdOrderLineRepository);
        }


        public ProdOrderLine Find(int id)
        {
            return _unitOfWork.Repository<ProdOrderLine>().Find(id);
        }

        public ProdOrderLine Create(ProdOrderLine pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ProdOrderLine>().Insert(pt);
            return pt;
        }
        public decimal GetProdOrderBalance(int? id)//ProdOrderLineId
        {
            return (from p in db.ViewProdOrderBalance
                    where p.ProdOrderLineId == id
                    where p.BalanceQty > 0
                    select p.BalanceQty
                        ).FirstOrDefault();
        }
        public IEnumerable<ProdOrderLineViewModel> GetProdOrderLineListForIndex(int id)
        {
            return (from p in db.ProdOrderLine
                    where p.ProdOrderHeaderId == id
                    join t in db.Dimension1 on p.Dimension1Id equals t.Dimension1Id into table
                    from tab in table.DefaultIfEmpty()
                    join t2 in db.Dimension2 on p.Dimension2Id equals t2.Dimension2Id into table2
                    from tab2 in table2.DefaultIfEmpty()
                    join t3 in db.Product on p.ProductId equals t3.ProductId into table3
                    from tab3 in table3.DefaultIfEmpty()
                    orderby p.Sr
                    select new ProdOrderLineViewModel
                    {
                        Dimension1Name = tab.Dimension1Name,
                        Dimension2Name = tab2.Dimension2Name,
                        MaterialPlanLineId = p.MaterialPlanLineId,
                        ProdOrderHeaderId = p.ProdOrderHeaderId,
                        ProdOrderLineId = p.ProdOrderLineId,
                        ProductName = tab3.ProductName,
                        Qty = p.Qty,
                        Remark = p.Remark,
                    }
                        );
        }
        public ProdOrderLineViewModel GetProdOrderLine(int id)
        {
            return (from p in db.ProdOrderLine
                    where p.ProdOrderLineId == id
                    select new ProdOrderLineViewModel
                    {
                        Dimension1Id = p.Dimension1Id,
                        Dimension2Id = p.Dimension2Id,
                        MaterialPlanLineId = p.MaterialPlanLineId,
                        ProdOrderHeaderId = p.ProdOrderHeaderId,
                        ProdOrderLineId = p.ProdOrderLineId,
                        ProductId = p.ProductId,
                        Qty = p.Qty,
                        Remark = p.Remark,
                    }

                        ).FirstOrDefault();
        }
        public void Delete(int id)
        {
            _unitOfWork.Repository<ProdOrderLine>().Delete(id);
        }

        public void Delete(ProdOrderLine pt)
        {
            _unitOfWork.Repository<ProdOrderLine>().Delete(pt);
        }

        public void Update(ProdOrderLine pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ProdOrderLine>().Update(pt);
        }

        public IEnumerable<ProdOrderLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<ProdOrderLine>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.ProdOrderLineId))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<ProdOrderLine> GetProdOrderLineList()
        {
            var pt = _unitOfWork.Repository<ProdOrderLine>().Query().Get().OrderBy(m => m.ProdOrderLineId);

            return pt;
        }

        public ProdOrderLine Add(ProdOrderLine pt)
        {
            _unitOfWork.Repository<ProdOrderLine>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.ProdOrderLine
                        orderby p.ProdOrderLineId
                        select p.ProdOrderLineId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.ProdOrderLine
                        orderby p.ProdOrderLineId
                        select p.ProdOrderLineId).FirstOrDefault();
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

                temp = (from p in db.ProdOrderLine
                        orderby p.ProdOrderLineId
                        select p.ProdOrderLineId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.ProdOrderLine
                        orderby p.ProdOrderLineId
                        select p.ProdOrderLineId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }
        public ProdOrderLineViewModel GetProdOrderDetailBalance(int id)
        {
            return (from b in db.ViewProdOrderBalance
                    join p in db.ProdOrderLine on b.ProdOrderLineId equals p.ProdOrderLineId
                    where b.ProdOrderLineId == id

                    select new ProdOrderLineViewModel
                    {
                        Qty = b.BalanceQty,
                        Specification = p.Specification,
                        Remark = p.Remark,
                        //Rate = tab.Rate,
                        //Amount = tab.Rate * (b.BalanceQty * p.UnitConversionMultiplier),
                        ProductId = p.ProductId,
                        UnitId = p.Product.UnitId,
                        UnitName = p.Product.Unit.UnitName,
                        ProductName = p.Product.ProductName,
                        Dimension1Id = p.Dimension1Id,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Id = p.Dimension2Id,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        unitDecimalPlaces = p.Product.Unit.DecimalPlaces,

                        //LotNo = p.LotNo,
                        //UnitConversionMultiplier = p.UnitConversionMultiplier,
                        //UnitId = p.Product.UnitId,
                        //DealUnitId = p.DealUnitId,
                        //DealQty = b.BalanceQty * p.UnitConversionMultiplier,
                    }).FirstOrDefault();

        }


        public IEnumerable<ProdOrderLine> GetProdOrderLineForMaterialPlan(int id)
        {
            return (from p in db.ProdOrderLine
                    where p.MaterialPlanLineId == id
                    select p
                        );

        }


        public IEnumerable<ProdOrderLineViewModel> GetProdOrderForFilters(ProdOrderLineFilterViewModel vm)
        {

            var ProdOrderHeader = new ProdOrderHeaderService(_unitOfWork).Find(vm.ProdOrderHeaderId);

            var settings = new ProdOrderSettingsService(_unitOfWork).GetProdOrderSettingsForDocument(ProdOrderHeader.DocTypeId, ProdOrderHeader.DivisionId, ProdOrderHeader.SiteId);

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            string[] filterProducts = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { filterProducts = settings.filterProducts.Split(",".ToCharArray()); }
            else { filterProducts = new string[] { "NA" }; }

            string[] filterProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { filterProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { filterProductTypes = new string[] { "NA" }; }

            string[] filterProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { filterProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { filterProductGroups = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.MaterialPlanHeaderId)) { SaleOrderIdArr = vm.MaterialPlanHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            var temp = (from p in db.ViewMaterialPlanBalance
                        join t in db.MaterialPlanHeader on p.MaterialPlanHeaderId equals t.MaterialPlanHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        join t1 in db.MaterialPlanLine on p.MaterialPlanLineId equals t1.MaterialPlanLineId into table1
                        from tab1 in table1.DefaultIfEmpty()
                        from tab2 in table2.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.MaterialPlanHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.MaterialPlanHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                        && p.BalanceQty > 0
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(p.DocTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : filterProducts.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : filterProductGroups.Contains(tab2.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : filterProductTypes.Contains(tab2.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? tab.SiteId == CurrentSiteId : contraSites.Contains(tab.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? tab.DivisionId == CurrentDivisionId : contraDivisions.Contains(tab.DivisionId.ToString()))
                        select new ProdOrderLineViewModel
                        {
                            Dimension1Name = tab1.Dimension1.Dimension1Name,
                            Dimension2Name = tab1.Dimension2.Dimension2Name,
                            Specification = tab1.Specification,
                            Dimension1Id = tab1.Dimension1Id,
                            Dimension2Id = tab1.Dimension2Id,
                            PlanBalanceQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            MaterialPlanHeaderDocNo = tab.DocNo,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            ProdOrderHeaderId = vm.ProdOrderHeaderId,
                            MaterialPlanLineId = p.MaterialPlanLineId,
                            //UnitId = tab2.UnitId,
                        }

                    );
            return temp;
        }

        public IQueryable<ComboBoxResult> GetPendingMaterialPlanHelpList(int Id, string term)
        {

            //var PurchaseIndent = new PurchaseIndentHeaderService(_unitOfWork).Find(Id);

            //var settings = new PurchaseIndentSettingService(_unitOfWork).GetPurchaseIndentSettingForDocument(PurchaseIndent.DocTypeId, PurchaseIndent.DivisionId, PurchaseIndent.SiteId);

            //string[] contraDocTypes = null;
            //if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            //else { contraDocTypes = new string[] { "NA" }; }

            //var list = (from p in db.ViewMaterialPlanBalance
            //            where (string.IsNullOrEmpty(term) ? 1 == 1 : p.MaterialPlanNo.ToLower().Contains(term.ToLower())) && p.BalanceQty > 0
            //           // && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(p.DocTypeId.ToString()))
            //            group new { p } by p.MaterialPlanHeaderId into g
            //            select new MaterialPlanLineHelpListViewModel
            //            {
            //                DocNo = g.Max(m => m.p.MaterialPlanNo),
            //                MaterialPlanHeaderId = g.Key,
            //                DocumentTypeName = g.Max(m => m.p.DocType.DocumentTypeName)
            //                //    DocumentTypeName=g.Max(p=>p.p.DocumentTypeShortName)
            //            }
            //              ).Take(20);

            //return list.ToList();


            var ProdOrderHeader = new ProdOrderHeaderService(_unitOfWork).Find(Id);

            var settings = new ProdOrderSettingsService(_unitOfWork).GetProdOrderSettingsForDocument(ProdOrderHeader.DocTypeId, ProdOrderHeader.DivisionId, ProdOrderHeader.SiteId);

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            string[] filterProducts = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { filterProducts = settings.filterProducts.Split(",".ToCharArray()); }
            else { filterProducts = new string[] { "NA" }; }

            string[] filterProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { filterProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { filterProductTypes = new string[] { "NA" }; }

            string[] filterProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { filterProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { filterProductGroups = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var list = (from p in db.ViewMaterialPlanBalance
                        join t in db.MaterialPlanHeader on p.MaterialPlanHeaderId equals t.MaterialPlanHeaderId
                        join Prod in db.Product on p.ProductId equals Prod.ProductId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.MaterialPlanNo.ToLower().Contains(term.ToLower())) && p.BalanceQty > 0
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(p.DocTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : filterProducts.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : filterProductGroups.Contains(Prod.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : filterProductTypes.Contains(Prod.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? t.SiteId == CurrentSiteId : contraSites.Contains(t.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? t.DivisionId == CurrentDivisionId : contraDivisions.Contains(t.DivisionId.ToString()))
                        group new { p } by p.MaterialPlanHeaderId into g
                        orderby g.Max(m => m.p.MaterialPlanDate)
                        select new ComboBoxResult
                        {
                            text = g.Max(m => m.p.MaterialPlanNo) + " | " + g.Max(m => m.p.DocType.DocumentTypeShortName),
                            id = g.Key.ToString(),
                        }
                          );

            return list;

        }

        public int GetMaxSr(int id)
        {
            var Max = (from p in db.ProdOrderLine
                       where p.ProdOrderHeaderId == id
                       select p.Sr
                        );

            if (Max.Count() > 0)
                return Max.Max(m => m ?? 0) + 1;
            else
                return (1);
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<ProdOrderLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProdOrderLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ProdOrderLineBalance GetLineDetail(int id)
        {
            return (from p in db.ViewProdOrderBalance
                    join t2 in db.Product on p.ProductId equals t2.ProductId
                    join t3 in db.Dimension1 on p.Dimension1Id equals t3.Dimension1Id into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t4 in db.Dimension2 on p.Dimension2Id equals t4.Dimension2Id into table4
                    from tab4 in table4.DefaultIfEmpty()
                    where p.ProdOrderLineId == id
                    select new ProdOrderLineBalance
                    {
                        Dimension1Name = tab3.Dimension1Name,
                        Dimension2Name = tab4.Dimension2Name,
                        ProductId = p.ProductId,
                        ProductName = t2.ProductName,
                        ProdOrderLineId = p.ProdOrderLineId,
                        ProdOrderDocNo = p.ProdOrderNo,
                        BalanceQty = p.BalanceQty,
                    }).FirstOrDefault();
        }

        public ProdOrderLineViewModel GetProdOrderForProdUid(int id)
        {

            return (from p in db.ProductUid
                    where p.ProductUIDId == id
                    join t in db.JobOrderLine on p.ProductUidHeaderId equals t.ProductUidHeaderId
                    join t2 in db.ProdOrderLine on t.JobOrderLineId equals t2.ReferenceDocLineId
                    select new ProdOrderLineViewModel
                    {
                        ProdOrderLineId = t2.ProdOrderLineId,
                        ProdOrderDocNo = t2.ProdOrderHeader.DocNo
                    }
                       ).FirstOrDefault();


        }
    }
}
