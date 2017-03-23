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
using Surya.India.Model.ViewModel;
using System.Data.SqlClient;
using System.Configuration;


namespace Surya.India.Service
{
    public interface IJobOrderLineService : IDisposable
    {
        JobOrderLine Create(JobOrderLine s);
        void Delete(int id);
        void Delete(JobOrderLine s);
        JobOrderLineViewModel GetJobOrderLine(int id);
        JobOrderLine Find(int id);
        void Update(JobOrderLine s);
        IQueryable<JobOrderLineViewModel> GetJobOrderLineListForIndex(int JobOrderHeaderId);
        IQueryable<JobOrderBomViewModel> GetConsumptionLineListForIndex(int JobOrderHeaderId);
        IEnumerable<JobOrderLineViewModel> GetJobOrderLineforDelete(int headerid);
        IEnumerable<ProcGetBomForWeavingViewModel> GetBomPostingDataForWeaving(int ProductId, int? Dimension1Id, int? Dimension2Id, int ProcessId, decimal Qty, int DocTypeId, string ProcName);
        List<String> GetProcGenProductUids(int DocTypeId, decimal Qty, int DivisionId, int SiteId);
        JobOrderLineViewModel GetLineDetailFromUId(string UID);
        IEnumerable<JobOrderLineViewModel> GetProdOrdersForFilters(JobOrderLineFilterViewModel vm);
        IQueryable<ComboBoxResult> GetPendingProdOrderHelpList(int Id, string term);//PurchaseOrderHeaderId

        IEnumerable<ProdOrderHeaderListViewModel> GetPendingProdOrdersWithPatternMatch(int Id, string term, int Limiter);
        IEnumerable<ComboBoxList> GetProductHelpList(int Id, string term);
        int GetMaxSr(int id);
        List<ComboBoxResult> GetBarCodesForWeavingWizard(int id, string term);
    }

    public class JobOrderLineService : IJobOrderLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public JobOrderLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public JobOrderLine Create(JobOrderLine S)
        {
            S.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<JobOrderLine>().Insert(S);
            return S;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<JobOrderLine>().Delete(id);
        }

        public void Delete(JobOrderLine s)
        {
            _unitOfWork.Repository<JobOrderLine>().Delete(s);
        }

        public void Update(JobOrderLine s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<JobOrderLine>().Update(s);
        }


        public JobOrderLineViewModel GetJobOrderLine(int id)
        {
            var temp = (from p in db.JobOrderLine
                        where p.JobOrderLineId == id
                        select new JobOrderLineViewModel
                        {
                            ProductId = p.ProductId,
                            ProductUidHeaderId = p.ProductUidHeaderId,
                            DueDate = p.DueDate,
                            Qty = p.Qty,
                            Remark = p.Remark,
                            JobOrderHeaderId = p.JobOrderHeaderId,
                            JobOrderLineId = p.JobOrderLineId,
                            ProductName = p.Product.ProductName,
                            Dimension1Id = p.Dimension1Id,
                            Dimension1Name = p.Dimension1.Dimension1Name,
                            Dimension2Name = p.Dimension2.Dimension2Name,
                            Dimension2Id = p.Dimension2Id,
                            ProductUidId = p.ProductUidId,
                            ProductUidName = p.ProductUid.ProductUidName,
                            ProdOrderLineId = p.ProdOrderLineId,
                            ProdOrderDocNo = p.ProdOrderLine.ProdOrderHeader.DocNo,
                            LotNo = p.LotNo,
                            FromProcessId = p.FromProcessId,
                            DealUnitId = p.DealUnitId,
                            DealQty = p.DealQty,
                            LockReason = p.LockReason,
                            LossQty = p.LossQty,
                            Rate = p.Rate,
                            Amount = p.Amount,
                            NonCountedQty = p.NonCountedQty,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            UnitConversionMultiplier = p.UnitConversionMultiplier,
                            UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                            DealUnitDecimalPlaces = p.DealUnit.DecimalPlaces,
                            Specification = p.Specification,
                        }).FirstOrDefault();

            temp.ProdOrderBalanceQty = (new ProdOrderLineService(_unitOfWork).GetProdOrderBalance(temp.ProdOrderLineId)) + temp.Qty;


            return temp;
        }
        public JobOrderLine Find(int id)
        {
            return _unitOfWork.Repository<JobOrderLine>().Find(id);
        }


        public IQueryable<JobOrderBomViewModel> GetConsumptionLineListForIndex(int JobOrderHeaderId)
        {
            var temp = from p in db.JobOrderBom
                       join t in db.Dimension1 on p.Dimension1Id equals t.Dimension1Id into table
                       from Dim1 in table.DefaultIfEmpty()
                       join t1 in db.Dimension2 on p.Dimension2Id equals t1.Dimension2Id into table1
                       from Dim2 in table1.DefaultIfEmpty()
                       join t2 in db.Product on p.ProductId equals t2.ProductId into table2
                       from ProdTab in table2.DefaultIfEmpty()
                       orderby p.JobOrderLineId
                       where p.JobOrderHeaderId == JobOrderHeaderId
                       select new JobOrderBomViewModel
                       {
                           UnitName = ProdTab.Unit.UnitName,
                           UnitDecimalPlaces = ProdTab.Unit.DecimalPlaces,
                           Dimension1Name = Dim1.Dimension1Name,
                           Dimension2Name = Dim2.Dimension2Name,
                           JobOrderBomId = p.JobOrderBomId,
                           JobOrderHeaderId = p.JobOrderHeaderId,
                           JobOrderLineId = p.JobOrderLineId,
                           ProductName = ProdTab.ProductName,
                           Qty = p.Qty,
                       };
            return temp;
        }

        public IQueryable<JobOrderLineViewModel> GetJobOrderLineListForIndex(int JobOrderHeaderId)
        {
            var temp = from p in db.JobOrderLine
                       join t in db.Dimension1 on p.Dimension1Id equals t.Dimension1Id into table
                       from Dim1 in table.DefaultIfEmpty()
                       join t1 in db.Dimension2 on p.Dimension2Id equals t1.Dimension2Id into table1
                       from Dim2 in table1.DefaultIfEmpty()
                       join t3 in db.JobOrderLineStatus on p.JobOrderLineId equals t3.JobOrderLineId
                       into table3
                       from tab3 in table3.DefaultIfEmpty()
                       where p.JobOrderHeaderId == JobOrderHeaderId
                       orderby p.Sr
                       select new JobOrderLineViewModel
                       {
                           ProductUidName = p.ProductUid.ProductUidName,
                           ProdOrderDocNo = p.ProdOrderLine.ProdOrderHeader.DocNo,
                           LotNo = p.LotNo,
                           FromProcessName = p.FromProcess.ProcessName,
                           DealUnitId = p.DealUnitId,
                           DealUnitName = p.DealUnit.UnitName,
                           DealQty = p.DealQty,
                           NonCountedQty = p.NonCountedQty,
                           LossQty = p.LossQty,
                           Rate = p.Rate,
                           ProductUidHeaderId = p.ProductUidHeaderId,
                           Amount = p.Amount,
                           Dimension1Id = p.Dimension1Id,
                           Dimension2Id = p.Dimension2Id,
                           Dimension1Name = Dim1.Dimension1Name,
                           Dimension2Name = Dim2.Dimension2Name,
                           Specification = p.Specification,
                           UnitConversionMultiplier = p.UnitConversionMultiplier,
                           UnitId = p.Product.UnitId,
                           UnitName = p.Product.Unit.UnitName,
                           UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                           DealUnitDecimalPlaces = p.DealUnit.DecimalPlaces,
                           Remark = p.Remark,
                           DueDate = p.DueDate,
                           ProductId = p.ProductId,
                           ProductName = p.Product.ProductName,
                           Qty = p.Qty,
                           JobOrderHeaderId = p.JobOrderHeaderId,
                           JobOrderLineId = p.JobOrderLineId,
                           ProgressPerc = ((p.Qty == 0 || tab3 == null) ? 0 : (int)(((((tab3.CancelQty ?? 0) + (tab3.ReceiveQty ?? 0)
                           - (tab3.AmendmentQty ?? 0)) / p.Qty) * 100))),
                       };
            return temp;
        }

        public IEnumerable<JobOrderLineViewModel> GetJobOrderLineforDelete(int headerid)
        {
            return (from p in db.JobOrderLine
                    where p.JobOrderHeaderId == headerid
                    select new JobOrderLineViewModel
                    {
                        JobOrderLineId = p.JobOrderLineId,
                        StockId = p.StockId,
                        StockProcessId = p.StockProcessId,
                        ProductUidId = p.ProductUidId,
                        ProductUidHeaderId = p.ProductUidHeaderId,
                    }

                        );


        }

        public IEnumerable<ProcGetBomForWeavingViewModel> GetBomPostingDataForWeaving(int ProductId, int? Dimension1Id, int? Dimension2Id, int ProcessId, decimal Qty, int DocTypeId, string ProcName)
        {
            SqlParameter SQLDocTypeID = new SqlParameter("@DocTypeId", DocTypeId);
            SqlParameter SQLProductID = new SqlParameter("@ProductId", ProductId);
            SqlParameter SQLProcessId = new SqlParameter("@ProcessId", ProcessId);
            SqlParameter SQLQty = new SqlParameter("@Qty", Qty);
            SqlParameter SQLDime1 = new SqlParameter("@Dimension1Id", Dimension1Id);
            SqlParameter SQLDime2 = new SqlParameter("@Dimension2Id", Dimension2Id);

            List<ProcGetBomForWeavingViewModel> PendingOrderQtyForPacking = new List<ProcGetBomForWeavingViewModel>();

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            //string ProcName = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId).SqlProcConsumption;

            //PendingOrderQtyForPacking = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".ProcGetBomForWeaving @DocTypeId, @ProductId, @ProcessId,@Qty"+(Dimension1Id==null?"":",@Dimension1Id")+(Dimension2Id==null?"":",@Dimension2Id"), SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, (Dimension1Id==null? "" : Dimension1Id), (Dimension2Id)).ToList();


            if (Dimension1Id == null && Dimension2Id == null)
            {
                PendingOrderQtyForPacking = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty).ToList();
            }
            else if (Dimension1Id == null && Dimension2Id != null)
            {
                PendingOrderQtyForPacking = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty,@Dimension2Id", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, SQLDime2).ToList();
            }
            else if (Dimension1Id != null && Dimension2Id == null)
            {
                PendingOrderQtyForPacking = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty,@Dimension1Id", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, SQLDime1).ToList();
            }
            else
            {
                PendingOrderQtyForPacking = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty,@Dimension1Id, @Dimension2Id", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, SQLDime1, SQLDime2).ToList();
            }


            return PendingOrderQtyForPacking;

        }

        public JobOrderRateAmendmentLineViewModel GetLineDetail(int id)
        {
            return (from p in db.ViewJobOrderBalanceForInvoice
                    join t1 in db.JobOrderLine on p.JobOrderLineId equals t1.JobOrderLineId
                    join t2 in db.Product on p.ProductId equals t2.ProductId
                    join t3 in db.Dimension1 on t1.Dimension1Id equals t3.Dimension1Id into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t4 in db.Dimension2 on t1.Dimension2Id equals t4.Dimension2Id into table4
                    from tab4 in table4.DefaultIfEmpty()
                    join t5 in db.JobWorker on p.JobWorkerId equals t5.PersonID
                    where p.JobOrderLineId == id
                    select new JobOrderRateAmendmentLineViewModel
                    {
                        Dimension1Name = tab3.Dimension1Name,
                        Dimension2Name = tab4.Dimension2Name,
                        LotNo = t1.LotNo,
                        Qty = p.BalanceQty,
                        Specification = t1.Specification,
                        UnitId = t2.UnitId,
                        DealUnitId = t1.DealUnitId,
                        DealQty = p.BalanceQty * t1.UnitConversionMultiplier,
                        UnitConversionMultiplier = t1.UnitConversionMultiplier,
                        UnitName = t2.Unit.UnitName,
                        DealUnitName = t1.DealUnit.UnitName,
                        ProductId = p.ProductId,
                        ProductName = t1.Product.ProductName,
                        unitDecimalPlaces = t2.Unit.DecimalPlaces,
                        DealunitDecimalPlaces = t1.DealUnit.DecimalPlaces,
                        JobWorkerId = p.JobWorkerId,
                        JobWorkerName = t5.Person.Name,
                        Rate = p.Rate,

                    }
                        ).FirstOrDefault();

        }

        public JobOrderLineViewModel GetLineDetailForCancel(int id)
        {
            var temp = (from p in db.ViewJobOrderBalance
                        join t1 in db.JobOrderLine on p.JobOrderLineId equals t1.JobOrderLineId
                        join t2 in db.Product on p.ProductId equals t2.ProductId
                        join t3 in db.Dimension1 on t1.Dimension1Id equals t3.Dimension1Id into table3
                        from tab3 in table3.DefaultIfEmpty()
                        join t4 in db.Dimension2 on t1.Dimension2Id equals t4.Dimension2Id into table4
                        from tab4 in table4.DefaultIfEmpty()
                        where p.JobOrderLineId == id
                        select new JobOrderLineViewModel
                        {
                            Dimension1Name = tab3.Dimension1Name,
                            Dimension2Name = tab4.Dimension2Name,
                            LotNo = t1.LotNo,
                            Qty = p.BalanceQty,
                            Specification = t1.Specification,
                            UnitId = t2.UnitId,
                            DealUnitId = t1.DealUnitId,
                            JobOrderLineId = t1.JobOrderLineId,
                            DealQty = p.BalanceQty * t1.UnitConversionMultiplier,
                            UnitConversionMultiplier = t1.UnitConversionMultiplier,
                            UnitName = t2.Unit.UnitName,
                            DealUnitName = t1.DealUnit.UnitName,
                            ProductId = p.ProductId,
                            ProductName = t1.Product.ProductName,
                            UnitDecimalPlaces = t2.Unit.DecimalPlaces,
                            DealUnitDecimalPlaces = t1.DealUnit.DecimalPlaces,
                            Rate = p.Rate,
                            IsUidGenerated = (t1.ProductUidHeaderId == null ? false : true),
                            ProductUidHeaderId = t1.ProductUidHeaderId
                        }
                        ).FirstOrDefault();

            if (temp.IsUidGenerated)
            {
                List<ComboBoxList> Barcodes = new List<ComboBoxList>();
                var Temp = (from p in db.ProductUid
                            where p.ProductUidHeaderId == temp.ProductUidHeaderId && ((p.LastTransactionDocNo == null || p.GenDocNo == p.LastTransactionDocNo) && (p.LastTransactionDocTypeId == null || p.GenDocTypeId == p.LastTransactionDocTypeId))
                            select new { Id = p.ProductUIDId, Name = p.ProductUidName }).ToList();
                foreach (var item in Temp)
                {
                    Barcodes.Add(new ComboBoxList
                    {
                        Id = item.Id,
                        PropFirst = item.Name,
                    });
                }
                temp.BarCodes = Barcodes;
            }


            return temp;

        }

        public JobOrderLineViewModel GetLineDetailForReceive(int id, int ReceiveId)
        {

            var Receipt = new JobReceiveHeaderService(_unitOfWork).Find(ReceiveId);

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(Receipt.DocTypeId, Receipt.DivisionId, Receipt.SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            return (from p in db.ViewJobOrderBalance
                    join t1 in db.JobOrderLine on p.JobOrderLineId equals t1.JobOrderLineId
                    join t2 in db.Product on p.ProductId equals t2.ProductId
                    join t3 in db.Dimension1 on t1.Dimension1Id equals t3.Dimension1Id into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t4 in db.Dimension2 on t1.Dimension2Id equals t4.Dimension2Id into table4
                    from tab4 in table4.DefaultIfEmpty()
                    where p.JobOrderLineId == id
                      && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == Receipt.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == Receipt.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                    select new JobOrderLineViewModel
                    {
                        Dimension1Name = tab3.Dimension1Name,
                        Dimension2Name = tab4.Dimension2Name,
                        LotNo = t1.LotNo,
                        Qty = p.BalanceQty,
                        Specification = t1.Specification,
                        UnitId = t2.UnitId,
                        DealUnitId = t1.DealUnitId,
                        DealQty = p.BalanceQty * t1.UnitConversionMultiplier,
                        UnitConversionMultiplier = t1.UnitConversionMultiplier,
                        UnitName = t2.Unit.UnitName,
                        DealUnitName = t1.DealUnit.UnitName,
                        ProductId = p.ProductId,
                        ProductName = t1.Product.ProductName,
                        UnitDecimalPlaces = t2.Unit.DecimalPlaces,
                        DealUnitDecimalPlaces = t1.DealUnit.DecimalPlaces,
                        Rate = p.Rate,

                    }
                        ).FirstOrDefault();

        }


        public JobOrderLineViewModel GetLineDetailForInvoice(int id, int InvoiceId)
        {

            var Invoice = new JobInvoiceHeaderService(_unitOfWork).Find(InvoiceId);

            var OrderLine = new JobOrderLineService(_unitOfWork).Find(id);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(Invoice.DocTypeId, Invoice.DivisionId, Invoice.SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            var temp = (from p in db.ViewJobOrderBalance
                        join t1 in db.JobOrderLine on p.JobOrderLineId equals t1.JobOrderLineId
                        join t2 in db.Product on p.ProductId equals t2.ProductId
                        join t3 in db.Dimension1 on t1.Dimension1Id equals t3.Dimension1Id into table3
                        from tab3 in table3.DefaultIfEmpty()
                        join t4 in db.Dimension2 on t1.Dimension2Id equals t4.Dimension2Id into table4
                        from tab4 in table4.DefaultIfEmpty()
                        where p.JobOrderLineId == id
                          && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == Invoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                            && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == Invoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        select new JobOrderLineViewModel
                        {
                            Dimension1Name = tab3.Dimension1Name,
                            Dimension2Name = tab4.Dimension2Name,
                            LotNo = t1.LotNo,
                            Qty = p.BalanceQty,
                            Specification = t1.Specification,
                            UnitId = t2.UnitId,
                            DealUnitId = t1.DealUnitId,
                            Amount = t1.Amount,
                            DealQty = p.BalanceQty * t1.UnitConversionMultiplier,
                            UnitConversionMultiplier = t1.UnitConversionMultiplier,
                            UnitName = t2.Unit.UnitName,
                            DealUnitName = t1.DealUnit.UnitName,
                            ProductId = p.ProductId,
                            ProductName = t1.Product.ProductName,
                            UnitDecimalPlaces = t2.Unit.DecimalPlaces,
                            DealUnitDecimalPlaces = t1.DealUnit.DecimalPlaces,
                            Rate = p.Rate,
                            JobOrderHeaderDocNo = p.JobOrderNo,
                            CostCenterId = t1.JobOrderHeader.CostCenterId != null ? t1.JobOrderHeader.CostCenterId : null,
                            CostCenterName = t1.JobOrderHeader.CostCenterId != null ? t1.JobOrderHeader.CostCenter.CostCenterName : null,

                        }
                        ).FirstOrDefault();

            var Charges = (from p in db.JobOrderLineCharge
                           where p.LineTableId == OrderLine.JobOrderLineId
                           join t in db.Charge on p.ChargeId equals t.ChargeId
                           select new LineCharges
                           {
                               ChargeCode = t.ChargeCode,
                               Rate = p.Rate,
                           }).ToList();

            var HeaderCharges = (from p in db.JobOrderHeaderCharges
                                 where p.HeaderTableId == OrderLine.JobOrderHeaderId
                                 join t in db.Charge on p.ChargeId equals t.ChargeId
                                 select new HeaderCharges
                                 {
                                     ChargeCode = t.ChargeCode,
                                     Rate = p.Rate,
                                 }).ToList();

            temp.RHeaderCharges = HeaderCharges;
            temp.RLineCharges = Charges;


            return temp;

        }

        public bool ValidateJobOrder(int lineid, int headerid)
        {
            var temp = (from p in db.JobOrderRateAmendmentLine
                        where p.JobOrderLineId == lineid && p.JobOrderAmendmentHeaderId == headerid
                        select p).FirstOrDefault();
            if (temp != null)
                return false;
            else
                return true;

        }

        public JobOrderLineViewModel GetLineDetailFromUId(string UID)
        {
            return (from p in db.ViewJobOrderBalance
                    join t1 in db.JobOrderLine on p.JobOrderLineId equals t1.JobOrderLineId
                    join t in db.ProductUid on t1.ProductUidId equals t.ProductUIDId into uidtable
                    from uidtab in uidtable.DefaultIfEmpty()
                    join t2 in db.Product on p.ProductId equals t2.ProductId
                    join t3 in db.Dimension1 on t1.Dimension1Id equals t3.Dimension1Id into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t4 in db.Dimension2 on t1.Dimension2Id equals t4.Dimension2Id into table4
                    from tab4 in table4.DefaultIfEmpty()
                    where uidtab.ProductUidName == UID
                    select new JobOrderLineViewModel
                    {
                        Dimension1Name = tab3.Dimension1Name,
                        Dimension2Name = tab4.Dimension2Name,
                        LotNo = t1.LotNo,
                        Qty = p.BalanceQty,
                        Specification = t1.Specification,
                        UnitId = t2.UnitId,
                        DealUnitId = t1.DealUnitId,
                        DealQty = p.BalanceQty * t1.UnitConversionMultiplier,
                        UnitConversionMultiplier = t1.UnitConversionMultiplier,
                        UnitName = t2.Unit.UnitName,
                        DealUnitName = t1.DealUnit.UnitName,
                        Rate = p.Rate,

                    }
                        ).FirstOrDefault();

        }


        public List<String> GetProcGenProductUids(int DocTypeId, decimal Qty, int DivisionId, int SiteId)
        {
            string ProcName = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId).SqlProcGenProductUID;

            List<string> CalculationLineList = new List<String>();


            using (SqlConnection sqlConnection = new SqlConnection((string)System.Web.HttpContext.Current.Session["DefaultConnectionString"]))
            {
                sqlConnection.Open();

                int TypeId = DocTypeId;

                SqlCommand Totalf = new SqlCommand("SELECT * FROM " + ProcName + "( " + TypeId + ", " + Qty + ")", sqlConnection);

                SqlDataReader ExcessStockQty = (Totalf.ExecuteReader());
                while (ExcessStockQty.Read())
                {
                    CalculationLineList.Add((string)ExcessStockQty.GetValue(0));
                }
            }

            //IEnumerable<string> CalculationLineList = db.Database.SqlQuery<string>("SELECT * FROM " + ProcName + " ("+ SqlParameterDocType+"," +SqlParameterQty+") ").ToList();

            return CalculationLineList.ToList();

        }

        public IEnumerable<JobOrderLineViewModel> GetProdOrdersForFilters(JobOrderLineFilterViewModel vm)
        {
            byte? UnitConvForId = new JobOrderHeaderService(_unitOfWork).Find(vm.JobOrderHeaderId).UnitConversionForId;

            var joborder = new JobOrderHeaderService(_unitOfWork).Find(vm.JobOrderHeaderId);

            var Settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(joborder.DocTypeId, joborder.DivisionId, joborder.SiteId);


            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(Settings.filterContraSites)) { ContraSites = Settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(Settings.filterContraDivisions)) { ContraDivisions = Settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProdOrderHeaderId)) { SaleOrderIdArr = vm.ProdOrderHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] Dimension1 = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dimension1 = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dimension1 = new string[] { "NA" }; }

            string[] Dimension2 = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dimension2 = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dimension2 = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            if (!string.IsNullOrEmpty(vm.DealUnitId))
            {
                Unit Dealunit = new UnitService(_unitOfWork).Find(vm.DealUnitId);

                var temp = (from p in db.ViewProdOrderBalance
                            join t in db.ProdOrderHeader on p.ProdOrderHeaderId equals t.ProdOrderHeaderId into table
                            from tab in table.DefaultIfEmpty()
                            join product in db.Product on p.ProductId equals product.ProductId into table2
                            join t1 in db.ProdOrderLine on p.ProdOrderLineId equals t1.ProdOrderLineId into table1
                            from tab1 in table1.DefaultIfEmpty()
                            from tab2 in table2.DefaultIfEmpty()
                            join t3 in db.UnitConversion on new { p1 = p.ProductId, DU1 = vm.DealUnitId, U1 = UnitConvForId ?? 0 } equals new { p1 = t3.ProductId ?? 0, DU1 = t3.ToUnitId, U1 = t3.UnitConversionForId } into table3
                            from tab3 in table3.DefaultIfEmpty()
                            where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                            && (string.IsNullOrEmpty(vm.ProdOrderHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.ProdOrderHeaderId.ToString()))
                            && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dimension1.Contains(p.Dimension1Id.ToString()))
                            && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dimension2.Contains(p.Dimension2Id.ToString()))
                            && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                            && (string.IsNullOrEmpty(Settings.filterContraSites) ? p.SiteId == joborder.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                            && (string.IsNullOrEmpty(Settings.filterContraDivisions) ? p.DivisionId == joborder.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                            && p.BalanceQty > 0
                            orderby tab.DocDate, tab.DocNo, tab1.Sr
                            select new JobOrderLineViewModel
                            {
                                Dimension1Name = tab1.Dimension1.Dimension1Name,
                                Dimension2Name = tab1.Dimension2.Dimension2Name,
                                Dimension1Id = p.Dimension1Id,
                                Dimension2Id = p.Dimension2Id,
                                Specification = tab1.Specification,
                                ProdOrderBalanceQty = p.BalanceQty,
                                Qty = p.BalanceQty,
                                Rate = vm.Rate,
                                ProductName = tab2.ProductName,
                                ProductId = p.ProductId,
                                JobOrderHeaderId = vm.JobOrderHeaderId,
                                ProdOrderLineId = p.ProdOrderLineId,
                                UnitId = tab2.UnitId,
                                LossQty = Settings.LossQty,
                                NonCountedQty = Settings.NonCountedQty,
                                ProdOrderDocNo = p.ProdOrderNo,
                                DealUnitId = (tab3 == null ? tab2.UnitId : vm.DealUnitId),
                                UnitConversionMultiplier = (tab3 == null ? 1 : tab3.ToQty / tab3.FromQty),
                                UnitConversionException = tab3 == null ? true : false,
                                UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                                DealUnitDecimalPlaces = (tab3 == null ? tab2.Unit.DecimalPlaces : Dealunit.DecimalPlaces)
                            }

                        );
                return temp;
            }
            else
            {
                var temp = (from p in db.ViewProdOrderBalance
                            join t in db.ProdOrderHeader on p.ProdOrderHeaderId equals t.ProdOrderHeaderId into table
                            from tab in table.DefaultIfEmpty()
                            join product in db.Product on p.ProductId equals product.ProductId into table2
                            join t1 in db.ProdOrderLine on p.ProdOrderLineId equals t1.ProdOrderLineId into table1
                            from tab1 in table1.DefaultIfEmpty()
                            from tab2 in table2.DefaultIfEmpty()
                            where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                            && (string.IsNullOrEmpty(vm.ProdOrderHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.ProdOrderHeaderId.ToString()))
                             && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dimension1.Contains(p.Dimension1Id.ToString()))
                            && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dimension2.Contains(p.Dimension2Id.ToString()))
                            && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                            && p.BalanceQty > 0
                            select new JobOrderLineViewModel
                            {
                                Dimension1Name = tab1.Dimension1.Dimension1Name,
                                Dimension1Id = p.Dimension1Id,
                                Dimension2Name = tab1.Dimension2.Dimension2Name,
                                Dimension2Id = p.Dimension2Id,
                                Specification = tab1.Specification,
                                ProdOrderBalanceQty = p.BalanceQty,
                                Qty = p.BalanceQty,
                                Rate = vm.Rate,
                                LossQty = Settings.LossQty,
                                NonCountedQty = Settings.NonCountedQty,
                                ProdOrderDocNo = tab.DocNo,
                                ProductName = tab2.ProductName,
                                ProductId = p.ProductId,
                                JobOrderHeaderId = vm.JobOrderHeaderId,
                                ProdOrderLineId = p.ProdOrderLineId,
                                UnitId = tab2.UnitId,
                                DealUnitId = tab2.UnitId,
                                UnitConversionMultiplier = 1,
                                UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                                DealUnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                            }

                        );
                return temp;
            }

        }


        public IQueryable<ComboBoxResult> GetPendingProdOrderHelpList(int Id, string term)
        {

            var JobOrderHeader = new JobOrderHeaderService(_unitOfWork).Find(Id);

            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(JobOrderHeader.DocTypeId, JobOrderHeader.DivisionId, JobOrderHeader.SiteId);

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

            var list = (from p in db.ViewProdOrderBalance
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProdOrderNo.ToLower().Contains(term.ToLower())) && p.BalanceQty > 0
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(p.DocTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                        group new { p } by p.ProdOrderHeaderId into g
                        orderby g.Max(m => m.p.IndentDate)
                        select new ComboBoxResult
                        {
                            text = g.Max(m => m.p.ProdOrderNo) + " | " + g.Max(m => m.p.DocType.DocumentTypeShortName),
                            id = g.Key.ToString(),
                        }
                          );

            return list;
        }


        public IEnumerable<ProdOrderHeaderListViewModel> GetPendingProdOrdersWithPatternMatch(int Id, string term, int Limiter)
        {
            var JobOrderHeader = new JobOrderHeaderService(_unitOfWork).Find(Id);

            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(JobOrderHeader.DocTypeId, JobOrderHeader.DivisionId, JobOrderHeader.SiteId);

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


            var list = (from p in db.ViewProdOrderBalance
                        where (
                        string.IsNullOrEmpty(term) ? 1 == 1 : p.ProdOrderNo.ToLower().Contains(term.ToLower()) ||
                        string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower()) ||
                        string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower()) ||
                        string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower())
                        ) && p.BalanceQty > 0
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(p.DocTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                        orderby p.ProdOrderNo
                        select new ProdOrderHeaderListViewModel
                        {
                            DocNo = p.ProdOrderNo,
                            ProdOrderLineId = p.ProdOrderLineId,
                            ProductName = p.Product.ProductName,
                            Dimension1Name = p.Dimension1.Dimension1Name,
                            Dimension2Name = p.Dimension2.Dimension2Name,

                        }
                        ).Take(Limiter);

            return (list);
        }

        public IEnumerable<ComboBoxList> GetProductHelpList(int Id, string term)
        {
            var JobOrder = new JobOrderHeaderService(_unitOfWork).Find(Id);

            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(JobOrder.DocTypeId, JobOrder.DivisionId, JobOrder.SiteId);

            string settingProductTypes = "";
            string settingProductDivision = "";


            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { settingProductTypes = "|" + settings.filterProductTypes.Replace(",", "|,|") + "|"; }
            if (!string.IsNullOrEmpty(settings.FilterProductDivision)) { settingProductDivision = "|" + settings.FilterProductDivision.Replace(",", "|,|") + "|"; }



            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settingProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ProductDivision = null;
            if (!string.IsNullOrEmpty(settings.FilterProductDivision)) { ProductDivision = settingProductDivision.Split(",".ToCharArray()); }
            else { ProductDivision = new string[] { "NA" }; }

            var list = (from p in db.Product
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductName.ToLower().Contains(term.ToLower()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains("|" + p.ProductGroup.ProductTypeId.ToString() + "|"))
                        && (string.IsNullOrEmpty(settings.FilterProductDivision) ? 1 == 1 : ProductDivision.Contains("|" + p.DivisionId.ToString() + "|"))
                        group new { p } by p.ProductId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.p.ProductName),
                            Id = g.Key,
                        }
                          ).Take(20);

            return list.ToList();
        }

        public int GetMaxSr(int id)
        {
            var Max = (from p in db.JobOrderLine
                       where p.JobOrderHeaderId == id
                       select p.Sr
                        );

            if (Max.Count() > 0)
                return Max.Max(m => m ?? 0) + 1;
            else
                return (1);
        }

        public List<ComboBoxResult> GetBarCodesForWeavingWizard(int id, string term)
        {

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var Site = new SiteService(_unitOfWork).Find(SiteId);
            var JobOrder = Find(id);

            //return (from p in db.ViewRequisitionBalance
            //        where p.PersonId == id && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenter.CostCenterName.ToLower().Contains(term.ToLower()))
            //        && p.SiteId == SiteId && p.DivisionId == DivisionId
            //        group p by p.CostCenterId into g
            //        orderby g.Max(m => m.CostCenter.CostCenterName)
            //        select new ComboBoxResult
            //        {
            //            text = g.Max(m => m.CostCenter.CostCenterName),
            //            id = g.Key.Value.ToString(),
            //        });

            var temp = from p in db.ProductUid
                       where p.ProductUidHeaderId == JobOrder.ProductUidHeaderId && p.ProductId == JobOrder.ProductId && (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductUidName.ToLower().Contains(term.ToLower()))
                       && p.CurrenctGodownId == Site.DefaultGodownId
                       orderby p.ProductUidName
                       select new ComboBoxResult
                       {
                           text = p.ProductUidName,
                           id = p.ProductUIDId.ToString(),
                       };

            return temp.ToList();
        }

        public void Dispose()
        {
        }
    }
}
