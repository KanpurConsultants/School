﻿using System.Collections.Generic;
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


namespace Surya.India.Service
{
    public interface ISaleInvoiceLineService : IDisposable
    {
        SaleInvoiceLine Create(SaleInvoiceLine s);
        void Delete(int id);
        void Delete(SaleInvoiceLine s);
        SaleInvoiceLine GetSaleInvoiceLine(int id);
        IQueryable<SaleInvoiceLine> GetSaleInvoiceLineList(int SaleInvliceHeaderId);
        SaleInvoiceLine Find(int id);
        void Update(SaleInvoiceLine s);
        SaleInvoiceLineViewModel GetSaleInvoiceLineForLineId(int SaleInvoiceLineId);
        IEnumerable<SaleInvoiceLineViewModel> GetSaleInvoiceLineListForIndex(int SaleInvoiceHeaderId);
        IQueryable<SaleInvoiceLineViewModel> GetSaleInvoiceLineListForIndex(int SaleInvoiceHeaderId,int Iteration);
        bool CheckForProductExists(int ProductId, int SaleInvoiceHEaderId, int SaleInvoiceLineId);
        bool CheckForProductExists(int ProductId, int SaleInvoiceHEaderId);
        IEnumerable<SaleInvoiceLineViewModel> GetPackingLineForProductDetail(int PackingHeaderid);

        IEnumerable<ComboBoxList> GetPackginNoPendingForInvoice(int BuyerId, DateTime DocDate);
        string GetDescriptionOfGoods(int id);
        IEnumerable<SaleOrderProductHelpList> GetProductHelpListForSaleOrder(int Id, string term, int Limit);
        IEnumerable<ComboBoxList> GetProductHelpList(int Id, string term, int Limit);
        IEnumerable<ComboBoxList> GetPendingOrdersForInvoice(int id, string term, int Limit);
        DirectSaleInvoiceLineViewModel GetDirectSaleInvoiceLineForEdit(int id);
        IEnumerable<DirectSaleInvoiceLineViewModel> GetSaleOrdersForFilters(SaleInvoiceFilterViewModel vm);


    }

    public class SaleInvoiceLineService : ISaleInvoiceLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public SaleInvoiceLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SaleInvoiceLine Create(SaleInvoiceLine S)
        {
            S.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleInvoiceLine>().Insert(S);
            return S;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleInvoiceLine>().Delete(id);
        }

        public void Delete(SaleInvoiceLine s)
        {
            _unitOfWork.Repository<SaleInvoiceLine>().Delete(s);
        }

        public void Update(SaleInvoiceLine s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleInvoiceLine>().Update(s);
        }

        public SaleInvoiceLine GetSaleInvoiceLine(int id)
        {
            return _unitOfWork.Repository<SaleInvoiceLine>().Query().Get().Where(m => m.SaleInvoiceLineId == id).FirstOrDefault();
        }


        public SaleInvoiceLine Find(int id)
        {
            return _unitOfWork.Repository<SaleInvoiceLine>().Find(id);
        }

        public IQueryable<SaleInvoiceLine> GetSaleInvoiceLineList(int SaleInvoiceHeaderId)
        {
            return _unitOfWork.Repository<SaleInvoiceLine>().Query().Get().Where(m => m.SaleInvoiceHeaderId == SaleInvoiceHeaderId);
        }

        public SaleInvoiceLineViewModel GetSaleInvoiceLineForLineId(int SaleInvoiceLineId)
        {
            var temp = (from L in db.SaleInvoiceLine
                        join P in db.Product on L.ProductId equals P.ProductId into ProductTable
                        from ProductTab in ProductTable.DefaultIfEmpty()
                        join Du in db.Units on L.DealUnitId equals Du.UnitId into DealUnitTable
                        from DealUnitTab in DealUnitTable.DefaultIfEmpty()
                        orderby L.SaleInvoiceLineId
                        where L.SaleInvoiceLineId == SaleInvoiceLineId
                        select new SaleInvoiceLineViewModel
                        {
                            SaleInvoiceHeaderId = L.SaleInvoiceHeaderId,
                            SaleInvoiceLineId = L.SaleInvoiceLineId,
                            SaleDispatchLineId = L.SaleDispatchLineId,
                            ProductName = ProductTab.ProductName,
                            Qty = L.Qty,
                            DealQty = L.DealQty,
                            DealUnitName = DealUnitTab.UnitName,
                            ProductInvoiceGroupId = L.ProductInvoiceGroupId,
                            Rate = L.Rate,
                            Amount = L.Amount,
                            Remark = L.Remark,
                            CreatedBy = L.CreatedBy,
                            CreatedDate = L.CreatedDate,
                        }).FirstOrDefault();
            return temp;
        }



        public IEnumerable<SaleInvoiceLineViewModel> GetSaleInvoiceLineListForIndex(int SaleInvoiceHeaderId)
        {

            IEnumerable<SaleInvoiceLineViewModel> SaleInvoiceLineViewModel = (from l in db.ViewSaleInvoiceLine
                                                                              join t in db.Product on l.ProductID equals t.ProductId into table
                                                                              from tab in table.DefaultIfEmpty()
                                                                              join t1 in db.SaleOrderLine on l.SaleOrderLineId equals t1.SaleOrderLineId into table1
                                                                              from tab1 in table1.DefaultIfEmpty()
                                                                              join t2 in db.SaleOrderHeader on tab1.SaleOrderHeaderId equals t2.SaleOrderHeaderId into table2
                                                                              from tab2 in table2.DefaultIfEmpty()
                                                                              join t3 in db.ProductUid on l.ProductUidId equals t3.ProductUIDId into table3
                                                                              from tab3 in table3.DefaultIfEmpty()
                                                                              join t4 in db.ProductInvoiceGroup on l.ProductInvoiceGroupId equals t4.ProductInvoiceGroupId into table4
                                                                              from tab4 in table4.DefaultIfEmpty()
                                                                              join u in db.Units on l.DealUnitId equals u.UnitId into DealUnitTable
                                                                              from DealUnitTab in DealUnitTable.DefaultIfEmpty()
                                                                              where l.SaleInvoiceHeaderId == SaleInvoiceHeaderId
                                                                              orderby l.SaleInvoiceLineId
                                                                              select new SaleInvoiceLineViewModel
                                                                              {
                                                                                  SaleInvoiceLineId = l.SaleInvoiceLineId,
                                                                                  ProductName = tab.ProductName,
                                                                                  Specification = l.Specification,
                                                                                  SaleOrderHeaderDocNo = tab2.DocNo,
                                                                                  ProductUidIdName = tab3.ProductUidName,
                                                                                  BaleNo = l.BaleNo,
                                                                                  ProductInvoiceGroupName = tab4.ProductInvoiceGroupName,
                                                                                  Qty = l.Qty,
                                                                                  UnitId = tab.UnitId,
                                                                                  DealQty = l.DealQty,
                                                                                  DealUnitId = DealUnitTab.UnitName,
                                                                                  DealUnitDecimalPlaces = DealUnitTab.DecimalPlaces,
                                                                                  Rate = l.Rate,
                                                                                  Amount = l.Amount,
                                                                                  Remark = l.Remark,
                                                                              }).Take(2000).ToList();


            double x = 0;
            var p = SaleInvoiceLineViewModel.OrderBy(sx => double.TryParse(sx.BaleNo.Replace("-", "."), out x) ? x : 0);


            return p;
        }

        public IQueryable<SaleInvoiceLineViewModel> GetSaleInvoiceLineListForIndex(int SaleInvoiceHeaderId,int Iteration)
        {

            IQueryable<SaleInvoiceLineViewModel> SaleInvoiceLineViewModel = (from l in db.ViewSaleInvoiceLine
                                                                              join t in db.Product on l.ProductID equals t.ProductId into table
                                                                              from tab in table.DefaultIfEmpty()
                                                                              join t1 in db.SaleOrderLine on l.SaleOrderLineId equals t1.SaleOrderLineId into table1
                                                                              from tab1 in table1.DefaultIfEmpty()
                                                                              join t2 in db.SaleOrderHeader on tab1.SaleOrderHeaderId equals t2.SaleOrderHeaderId into table2
                                                                              from tab2 in table2.DefaultIfEmpty()
                                                                              join t3 in db.ProductUid on l.ProductUidId equals t3.ProductUIDId into table3
                                                                              from tab3 in table3.DefaultIfEmpty()
                                                                              join t4 in db.ProductInvoiceGroup on l.ProductInvoiceGroupId equals t4.ProductInvoiceGroupId into table4
                                                                              from tab4 in table4.DefaultIfEmpty()
                                                                              join u in db.Units on l.DealUnitId equals u.UnitId into DealUnitTable
                                                                              from DealUnitTab in DealUnitTable.DefaultIfEmpty()
                                                                              where l.SaleInvoiceHeaderId == SaleInvoiceHeaderId
                                                                              orderby l.SaleInvoiceLineId
                                                                              select new SaleInvoiceLineViewModel
                                                                              {
                                                                                  SaleInvoiceLineId = l.SaleInvoiceLineId,
                                                                                  ProductName = tab.ProductName,
                                                                                  Specification = l.Specification,
                                                                                  SaleOrderHeaderDocNo = tab2.DocNo,
                                                                                  ProductUidIdName = tab3.ProductUidName,
                                                                                  BaleNo = l.BaleNo,
                                                                                  ProductInvoiceGroupName = tab4.ProductInvoiceGroupName,
                                                                                  Qty = l.Qty,
                                                                                  UnitId = tab.UnitId,
                                                                                  DealQty = l.DealQty,
                                                                                  DealUnitId = DealUnitTab.UnitName,
                                                                                  DealUnitDecimalPlaces = DealUnitTab.DecimalPlaces,
                                                                                  Rate = l.Rate,
                                                                                  Amount = l.Amount,
                                                                                  Remark = l.Remark,
                                                                              });





            return SaleInvoiceLineViewModel;
        }

        public bool CheckForProductExists(int ProductId, int SaleInvoiceHeaderId, int SaleInvoiceLineId)
        {

            SaleInvoiceLine temp = (from p in db.SaleInvoiceLine
                                    where p.SaleInvoiceHeaderId == SaleInvoiceHeaderId && p.SaleInvoiceLineId != SaleInvoiceLineId
                                    select p).FirstOrDefault();
            if (temp != null)
                return true;
            else return false;
        }

        public bool CheckForProductExists(int ProductId, int SaleInvoiceHeaderId)
        {

            SaleInvoiceLine temp = (from p in db.SaleInvoiceLine
                                    where p.SaleInvoiceHeaderId == SaleInvoiceHeaderId
                                    select p).FirstOrDefault();
            if (temp != null)
                return true;
            else return false;
        }

        public IEnumerable<SaleInvoiceLineViewModel> GetPackingLineForProductDetail(int PackingHeaderid)
        {
            var saledispatchline = from L in db.SaleDispatchLine
                                   join H in db.SaleDispatchHeader on L.SaleDispatchHeaderId equals H.SaleDispatchHeaderId into SaleDispatchHeaderTable
                                   from SaleDispatchHeaderTab in SaleDispatchHeaderTable.DefaultIfEmpty()
                                   join PL in db.PackingLine on L.PackingLineId equals PL.PackingLineId into PackingLineTable
                                   from PackingLineTab in PackingLineTable.DefaultIfEmpty()
                                   group new { PackingLineTab } by new { PackingLineTab.PackingLineId } into Result
                                   select new
                                   {
                                       PackingLineId = Result.Key.PackingLineId,
                                       DispatchQty = Result.Sum(m => m.PackingLineTab.Qty)
                                   };


            var packingline = from L in db.PackingLine
                              join H in db.PackingHeader on L.PackingHeaderId equals H.PackingHeaderId into PackingHeaderTable
                              from PackingHeaderTab in PackingHeaderTable.DefaultIfEmpty()
                              join D in saledispatchline on L.PackingLineId equals D.PackingLineId into DispatchTable
                              from DispatchTab in DispatchTable.DefaultIfEmpty()
                              join P in db.FinishedProduct on L.ProductId equals P.ProductId into ProductTable
                              from ProductTab in ProductTable.DefaultIfEmpty()
                              join Pig in db.ProductInvoiceGroup on ProductTab.ProductInvoiceGroupId equals Pig.ProductInvoiceGroupId into ProductInvoiceGroupTable
                              from ProductInvoiceGroupTab in ProductInvoiceGroupTable.DefaultIfEmpty()
                              join Dog in db.DescriptionOfGoods on ProductInvoiceGroupTab.DescriptionOfGoodsId equals Dog.DescriptionOfGoodsId into DescriptionOfGoodsTable
                              from DescriptionOfGoodsTab in DescriptionOfGoodsTable.DefaultIfEmpty()
                              join Fc in db.ProductContentHeader on ProductTab.FaceContentId equals Fc.ProductContentHeaderId into FaceContentTable
                              from FaceContentTab in FaceContentTable.DefaultIfEmpty()
                              join Sol in db.SaleOrderLine on L.SaleOrderLineId equals Sol.SaleOrderLineId into SaleOrderLineTable
                              from SaleOrderLineTab in SaleOrderLineTable.DefaultIfEmpty()
                              where L.PackingHeaderId == PackingHeaderid && L.Qty - ((Decimal?)DispatchTab.DispatchQty ?? 0) > 0
                              select new SaleInvoiceLineViewModel
                              {
                                  PackingLineId = L.PackingLineId,
                                  LotNo = PackingHeaderTab.DocNo,
                                  GodownId = PackingHeaderTab.GodownId,
                                  SalesTaxGroupId = ProductTab.SalesTaxGroupProductId,
                                  SaleOrderLineId = L.SaleOrderLineId,
                                  ProductId = L.ProductId,
                                  DescriptionOfGoodsName = DescriptionOfGoodsTab.DescriptionOfGoodsName,
                                  FaceContentName = FaceContentTab.ProductContentName,
                                  ProductInvoiceGroupId = ProductInvoiceGroupTab.ProductInvoiceGroupId,
                                  ProductInvoiceGroupName = ProductInvoiceGroupTab.ProductInvoiceGroupName,
                                  BaleNo = L.BaleNo,
                                  Qty = L.Qty,
                                  DealQty = L.DealQty,
                                  UnitConversionMultiplier = L.DealQty / L.Qty,
                                  UnitId = ProductTab.UnitId,
                                  DealUnitId = L.DealUnitId,
                                  SaleOrderDealUnitId = SaleOrderLineTab.DealUnitId,
                                  SaleOrderRate = (Decimal?)SaleOrderLineTab.Rate ?? 0,
                                  Rate = (Decimal?)ProductInvoiceGroupTab.Rate ?? 0,
                                  Amount = (Decimal?)(L.DealQty * ProductInvoiceGroupTab.Rate) ?? 0
                              };

            return packingline;

        }

        public IEnumerable<ComboBoxList> GetPackginNoPendingForInvoice(int BuyerId, DateTime DocDate)
        {
            //SaleDispatchHeaderTab.SaleToBuyerId == BuyerId && 
            //PackingHeaderTab.BuyerId == BuyerId && 


            var saledispatchline = from L in db.SaleDispatchLine
                                   join H in db.SaleDispatchHeader on L.SaleDispatchHeaderId equals H.SaleDispatchHeaderId into SaleDispatchHeaderTable
                                   from SaleDispatchHeaderTab in SaleDispatchHeaderTable.DefaultIfEmpty()
                                   join PL in db.PackingLine on L.PackingLineId equals PL.PackingLineId into PackingLineTable
                                   from PackingLineTab in PackingLineTable.DefaultIfEmpty()
                                   group new { PackingLineTab } by new { PackingLineTab.PackingLineId } into Result
                                   select new
                                   {
                                       PackingLineId = Result.Key.PackingLineId,
                                       DispatchQty = Result.Sum(m => m.PackingLineTab.Qty)
                                   };


            var packingline = from L in db.PackingLine
                              join H in db.PackingHeader on L.PackingHeaderId equals H.PackingHeaderId into PackingHeaderTable
                              from PackingHeaderTab in PackingHeaderTable.DefaultIfEmpty()
                              join D in saledispatchline on L.PackingLineId equals D.PackingLineId into DispatchTable
                              from DispatchTab in DispatchTable.DefaultIfEmpty()
                              where PackingHeaderTab.DocDate <= DocDate && PackingHeaderTab.Status == (int)StatusConstants.Approved && L.Qty - ((Decimal?)DispatchTab.DispatchQty ?? 0) > 0
                              select new
                              {
                                  PackginLineId = L.PackingLineId,
                                  PackingHeaderId = L.PackingHeaderId,
                                  PackingNo = PackingHeaderTab.DocNo
                              };


            IEnumerable<ComboBoxList> packingheader = from H in packingline
                                                      group new { H } by new { H.PackingHeaderId } into Result
                                                      select new ComboBoxList
                                                      {
                                                          Id = Result.Key.PackingHeaderId,
                                                          PropFirst = Result.Max(m => m.H.PackingNo)
                                                      };


            return packingheader;
        }


        public string GetDescriptionOfGoods(int id)
        {
            string DescriptionOfGoods = "";

            var saleinvoicegoods = (from L in db.SaleInvoiceLine
                                    join dl in db.SaleDispatchLine on L.SaleDispatchLineId equals dl.SaleDispatchLineId into SaleDispatchLineTable
                                    from SaleDispatchLineTab in SaleDispatchLineTable.DefaultIfEmpty()
                                    join Pl in db.PackingLine on SaleDispatchLineTab.PackingLineId equals Pl.PackingLineId into PackingLineTable
                                    from PackingLineTab in PackingLineTable.DefaultIfEmpty()
                                    join P in db.FinishedProduct on PackingLineTab.ProductId equals P.ProductId into ProductTable
                                    from ProductTab in ProductTable.DefaultIfEmpty()
                                    join D in db.DescriptionOfGoods on ProductTab.DescriptionOfGoodsId equals D.DescriptionOfGoodsId into DescriptionOfGoodsTable
                                    from DescriptionOfGoodsTab in DescriptionOfGoodsTable.DefaultIfEmpty()
                                    where L.SaleInvoiceLineId == id
                                    group new { L, DescriptionOfGoodsTab } by new { L.SaleInvoiceHeaderId, DescriptionOfGoodsTab.DescriptionOfGoodsId } into Result
                                    select new
                                    {
                                        SaleInvoiceHeaderId = Result.Key.SaleInvoiceHeaderId,
                                        DescriptionOfGoodsName = Result.Max(m => m.DescriptionOfGoodsTab.DescriptionOfGoodsName)
                                    }).ToList();


            if (saleinvoicegoods != null)
            {
                foreach (var item in saleinvoicegoods)
                {
                    if (DescriptionOfGoods == "")
                    {
                        DescriptionOfGoods = item.DescriptionOfGoodsName;
                    }
                    else
                    {
                        DescriptionOfGoods = DescriptionOfGoods + "," + item.DescriptionOfGoodsName;
                    }
                }
            }
            return DescriptionOfGoods;
        }


        public IEnumerable<SaleOrderProductHelpList> GetProductHelpListForSaleOrder(int Id, string term, int Limit)
        {
            var SaleInvoice = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(Id);

            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(SaleInvoice.DocTypeId, SaleInvoice.DivisionId, SaleInvoice.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var list = (from p in db.ViewSaleOrderBalance
                        join t in db.SaleOrderHeader on p.SaleOrderHeaderId equals t.SaleOrderHeaderId
                        join t2 in db.SaleOrderLine on p.SaleOrderLineId equals t2.SaleOrderLineId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : t2.Specification.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.SaleOrderNo.ToLower().Contains(term.ToLower())
                        )
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == SaleInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == SaleInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        orderby t.DocDate, t.DocNo
                        select new SaleOrderProductHelpList
                        {
                            ProductName = p.Product.ProductName,
                            ProductId = p.ProductId,
                            Specification = t2.Specification,
                            Dimension1Name = p.Dimension1.Dimension1Name,
                            Dimension2Name = p.Dimension2.Dimension2Name,
                            SaleOrderDocNo = p.SaleOrderNo,
                            SaleOrderLineId = p.SaleOrderLineId,
                            Qty = p.BalanceQty,
                        }
                          ).Take(Limit);

            return list.ToList();
        }

        public IEnumerable<ComboBoxList> GetProductHelpList(int Id, string term, int Limit)
        {

            var SaleInvoice = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(Id);

            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(SaleInvoice.DocTypeId, SaleInvoice.DivisionId, SaleInvoice.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }


            var list = (from p in db.Product
                        where (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductName.ToLower().Contains(term.ToLower()))
                        select new ComboBoxList
                        {
                            Id = p.ProductId,
                            PropFirst = p.ProductName,
                        }
                ).Take(Limit);

            return list.ToList();

        }

        public IEnumerable<ComboBoxList> GetPendingOrdersForInvoice(int id, string Term, int Limit)
        {


            var SaleInvoiceHeader = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(id);

            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(SaleInvoiceHeader.DocTypeId, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            return (from p in db.ViewSaleOrderBalance
                    join t in db.SaleOrderHeader on p.SaleOrderHeaderId equals t.SaleOrderHeaderId into table
                    from tab in table.DefaultIfEmpty()
                    where p.BalanceQty > 0
                    && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(tab.DocTypeId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? tab.SiteId == CurrentSiteId : contraSites.Contains(tab.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? tab.DivisionId == CurrentDivisionId : contraDivisions.Contains(tab.DivisionId.ToString()))
                    group p by p.SaleOrderHeaderId into g
                    select new ComboBoxList
                    {
                        Id = g.Key,
                        PropFirst = g.Max(m => m.SaleOrderNo),
                    }
                        ).Take(Limit);
        }

        public DirectSaleInvoiceLineViewModel GetDirectSaleInvoiceLineForEdit(int id)
        {

            return (from p in db.SaleInvoiceLine
                    join t in db.SaleDispatchLine on p.SaleDispatchLineId equals t.SaleDispatchLineId into table
                    from Dl in table.DefaultIfEmpty()
                    join t2 in db.PackingLine on Dl.PackingLineId equals t2.PackingLineId into table2
                    from Pl in table2.DefaultIfEmpty()
                    join t3 in db.ViewSaleOrderBalance on p.SaleOrderLineId equals t3.SaleOrderLineId into table3
                    from tab3 in table3.DefaultIfEmpty()
                    where p.SaleInvoiceLineId == id
                    select new DirectSaleInvoiceLineViewModel
                    {
                        ProductUidId = Pl.ProductUidId,
                        ProductUidIdName = Pl.ProductUid.ProductUidName,
                        ProductId = Pl.ProductId,
                        ProductName = Pl.Product.ProductName,
                        SaleOrderHeaderDocNo = p.SaleOrderLine.SaleOrderHeader.DocNo,
                        Qty = Pl.Qty,
                        BalanceQty = (tab3 == null ? p.Qty : tab3.BalanceQty + p.Qty),
                        BaleNo = Pl.BaleNo,
                        UnitId = p.Product.UnitId,
                        UnitName = p.Product.Unit.UnitName,
                        DealUnitId = Pl.DealUnitId,
                        DealUnitName = Pl.DealUnit.UnitName,
                        DealQty = Pl.DealQty,
                        Remark = Pl.Remark,
                        Specification = Pl.Specification,
                        Dimension1Id = Pl.Dimension1Id,
                        Dimension2Id = Pl.Dimension2Id,
                        LotNo = Pl.LotNo,
                        GodownId = Dl.GodownId,
                        DiscountPer = p.DiscountPer,
                        Amount = p.Amount,
                        UnitConversionMultiplier = p.UnitConversionMultiplier,
                        Rate = p.Rate,
                        SaleInvoiceLineId = p.SaleInvoiceLineId,
                        SaleInvoiceHeaderId = p.SaleInvoiceHeaderId,
                        SaleDispatchLineId = p.SaleDispatchLineId,
                        PackingLineId = Pl.PackingLineId,
                        SaleOrderLineId = p.SaleOrderLineId,
                    }
                        ).FirstOrDefault();

        }


        public IEnumerable<DirectSaleInvoiceLineViewModel> GetSaleOrdersForFilters(SaleInvoiceFilterViewModel vm)
        {
            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.SaleOrderHeaderId)) { SaleOrderIdArr = vm.SaleOrderHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            var temp = (from p in db.ViewSaleOrderBalance
                        join t in db.SaleOrderHeader on p.SaleOrderHeaderId equals t.SaleOrderHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join t1 in db.SaleOrderLine on p.SaleOrderLineId equals t1.SaleOrderLineId into table1
                        from tab1 in table1.DefaultIfEmpty()
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.SaleOrderHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.SaleOrderHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                        && p.BalanceQty > 0
                        orderby p.SaleOrderLineId
                        select new DirectSaleInvoiceLineViewModel
                        {
                            //ProductUidIdName = tab1.ProductUid != null ? tab1.ProductUid.ProductUidName : "",
                            Dimension1Name = tab1.Dimension1.Dimension1Name,
                            Dimension2Name = tab1.Dimension2.Dimension2Name,
                            Specification = tab1.Specification,
                            BalanceQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            SaleOrderHeaderDocNo = tab.DocNo,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            SaleInvoiceHeaderId = vm.SaleInvoiceHeaderId,
                            SaleOrderLineId = p.SaleOrderLineId,
                            UnitId = tab2.UnitId,
                            UnitName = tab2.Unit.UnitName,
                            DealUnitId = tab1.DealUnitId,
                            DealUnitName = tab1.DealUnit.UnitName,
                            unitDecimalPlaces = tab2.Unit.DecimalPlaces,
                            DealunitDecimalPlaces = tab1.DealUnit.DecimalPlaces,
                            DealQty = (!tab1.UnitConversionMultiplier.HasValue || tab1.UnitConversionMultiplier <= 0) ? p.BalanceQty : p.BalanceQty * tab1.UnitConversionMultiplier.Value,
                            UnitConversionMultiplier = tab1.UnitConversionMultiplier,
                            Rate = tab1.Rate,
                            DiscountPer = tab1.DiscountPer,
                        }

                        );
            return temp;
        }


        public SaleInvoiceLineViewModel GetSaleInvoiceLineBalance(int id)
        {
            return (from b in db.ViewSaleInvoiceBalance
                    join p in db.SaleInvoiceLine on b.SaleInvoiceLineId equals p.SaleInvoiceLineId
                    join t in db.SaleDispatchLine on p.SaleDispatchLineId equals t.SaleDispatchLineId into table
                    from tab in table.DefaultIfEmpty()
                    join t2 in db.SaleInvoiceHeader on p.SaleInvoiceHeaderId equals t2.SaleInvoiceHeaderId into table2
                    from tab2 in table2.DefaultIfEmpty()
                    join t in db.SaleDispatchHeader on tab.SaleDispatchHeaderId equals t.SaleDispatchHeaderId into table1
                    from tab1 in table1.DefaultIfEmpty()

                    where p.SaleInvoiceLineId == id
                    select new SaleInvoiceLineViewModel
                    {
                        Amount = p.Amount,
                        ProductId = p.ProductId,
                        SaleDispatchLineId = p.SaleDispatchLineId,
                        // SaleDispatchHeaderDocNo = tab1.DocNo,
                        SaleInvoiceHeaderId = p.SaleInvoiceHeaderId,
                        SaleInvoiceLineId = p.SaleInvoiceLineId,
                        Qty = b.BalanceQty,
                        Rate = p.Rate,
                        Remark = p.Remark,
                        UnitConversionMultiplier = p.UnitConversionMultiplier,
                        DealUnitId = p.DealUnitId,
                        DealQty = p.DealQty,
                        UnitId = p.Product.UnitId,
                        //Dimension1Id = p.Dimension1Id,
                        //Dimension1Name = p.Dimension1.Dimension1Name,
                        //Dimension2Id = p.Dimension2Id,
                        //Dimension2Name = p.Dimension2.Dimension2Name,
                        //Specification = p.Specification,
                        //LotNo = tab.LotNo,
                        //DiscountPer = p.DiscountPer

                    }
                        ).FirstOrDefault();
        }

        public void Dispose()
        {
        }
    }
}
