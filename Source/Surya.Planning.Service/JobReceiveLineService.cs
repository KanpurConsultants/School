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
using Surya.India.Model.ViewModels;
using Surya.India.Model.ViewModel;
using System.Data.SqlClient;

namespace Surya.India.Service
{
    public interface IJobReceiveLineService : IDisposable
    {
        JobReceiveLine Create(JobReceiveLine pt);
        void Delete(int id);
        void Delete(JobReceiveLine pt);
        JobReceiveLine Find(int id);
        IEnumerable<JobReceiveLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(JobReceiveLine pt);
        JobReceiveLine Add(JobReceiveLine pt);
        IEnumerable<JobReceiveLine> GetJobReceiveLineList();
        IEnumerable<JobReceiveLineViewModel> GetLineListForIndex(int headerId);//HeaderId
        Task<IEquatable<JobReceiveLine>> GetAsync();
        Task<JobReceiveLine> FindAsync(int id);
        JobReceiveLineViewModel GetJobReceiveLine(int id);
        IEnumerable<JobReceiveLineViewModel> GetJobOrdersForFilters(JobReceiveLineFilterViewModel vm);
        IQueryable<ProductHelpListViewModel> GetPendingProductsForJobReceive(int Id, string term);
        IQueryable<ComboBoxList> GetPendingJobOrders(int id, string term);//JobReceive HeaderId,DocTypes,Search term
        //IEnumerable<ProcGetBomForWeavingViewModel> GetBomPostingDataForWeaving(int ProductId, int? Dimension1Id, int? Dimension2Id, int ProcessId, decimal Qty, int DocTypeId, string ProcName, int? JobOrderLineId);

        IEnumerable<ProcGetBomForWeavingViewModel> GetBomPostingDataForWeaving(int ProductId, int? Dimension1Id, int? Dimension2Id, int ProcessId, decimal Qty, int DocTypeId, string ProcName, int? JobOrderLineId, Decimal? Weight);
        IQueryable<JobReceiveBomViewModel> GetConsumptionLineListForIndex(int JobOrderHeaderId);
        IQueryable<JobReceiveByProductViewModel> GetByProductListForIndex(int JobOrderHeaderId);
        IEnumerable<ComboBoxList> GetProductHelpList(int Id, string term);
        string GetFirstBarCodeForCancel(int JobOrderLineId);
        List<ComboBoxList> GetPendingBarCodesList(int id);
        List<ComboBoxList> GetPendingBarCodesList(int[] id);
        int GetMaxSr(int id);
        IQueryable<ComboBoxResult> GetPendingCostCenters(int id, string term);

    }

    public class JobReceiveLineService : IJobReceiveLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<JobReceiveLine> _JobReceiveLineRepository;
        RepositoryQuery<JobReceiveLine> JobReceiveLineRepository;
        public JobReceiveLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _JobReceiveLineRepository = new Repository<JobReceiveLine>(db);
            JobReceiveLineRepository = new RepositoryQuery<JobReceiveLine>(_JobReceiveLineRepository);
        }

        public JobReceiveLine Find(int id)
        {
            return _unitOfWork.Repository<JobReceiveLine>().Find(id);
        }

        public JobReceiveLine Create(JobReceiveLine pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<JobReceiveLine>().Insert(pt);
            return pt;
        }

        public IEnumerable<JobReceiveLineViewModel> GetJobOrdersForFilters(JobReceiveLineFilterViewModel vm)
        {

            var JobReceive = new JobReceiveHeaderService(_unitOfWork).Find(vm.JobReceiveHeaderId);

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(JobReceive.DocTypeId, JobReceive.DivisionId, JobReceive.SiteId);

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.JobOrderHeaderId)) { SaleOrderIdArr = vm.JobOrderHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] CostCenterIdArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterIdArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var temp = (from p in db.ViewJobOrderBalance
                        join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join t1 in db.JobOrderLine on p.JobOrderLineId equals t1.JobOrderLineId into table1
                        from tab1 in table1.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.JobOrderHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.JobOrderHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.CostCenterId) ? 1 == 1 : CostCenterIdArr.Contains(tab.CostCenterId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobReceive.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobReceive.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && p.BalanceQty > 0 && p.JobWorkerId == vm.JobWorkerId
                        orderby tab.DocDate, tab.DocNo, tab1.Sr
                        select new JobReceiveLineViewModel
                        {
                            Dimension1Name = tab1.Dimension1.Dimension1Name,
                            Dimension2Name = tab1.Dimension2.Dimension2Name,
                            Specification = tab1.Specification,
                            OrderBalanceQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            DocQty = p.BalanceQty,
                            ReceiveQty = p.BalanceQty,
                            PassQty = p.BalanceQty,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            JobReceiveHeaderId = vm.JobReceiveHeaderId,
                            JobOrderLineId = p.JobOrderLineId,
                            UnitId = tab2.UnitId,
                            JobOrderHeaderDocNo = p.JobOrderNo,
                            DealUnitId = tab2.UnitId,
                            UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                            DealUnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                            JobOrderUidHeaderId = tab1.ProductUidHeaderId,
                            ProductUidId = tab1.ProductUidId,
                            CostCenterName = tab.CostCenter.CostCenterName,
                            ProdOrderLineId = tab1.ProdOrderLineId,
                            JobOrderHeaderId = tab1.JobOrderHeaderId,
                        }
                        );
            return temp;
        }
        public void Delete(int id)
        {
            _unitOfWork.Repository<JobReceiveLine>().Delete(id);
        }

        public void Delete(JobReceiveLine pt)
        {
            _unitOfWork.Repository<JobReceiveLine>().Delete(pt);
        }

        public void Update(JobReceiveLine pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<JobReceiveLine>().Update(pt);
        }
        public IEnumerable<JobReceiveLineViewModel> GetLineListForIndex(int headerId)
        {
            var pt = (from p in db.JobReceiveLine
                      where p.JobReceiveHeaderId == headerId
                      join t in db.JobOrderLine on p.JobOrderLineId equals t.JobOrderLineId
                      into table
                      from tab in table.DefaultIfEmpty()
                      orderby p.Sr
                      select new JobReceiveLineViewModel
                      {
                          JobReceiveLineId = p.JobReceiveLineId,
                          Qty = p.Qty + p.LossQty,
                          Remark = p.Remark,
                          UnitConversionMultiplier = p.JobOrderLine.UnitConversionMultiplier,
                          JobOrderHeaderDocNo = p.JobOrderLine.JobOrderHeader.DocNo,
                          Dimension1Name = p.JobOrderLine.Dimension1.Dimension1Name,
                          Dimension2Name = p.JobOrderLine.Dimension2.Dimension2Name,
                          Specification = p.JobOrderLine.Specification,
                          JobReceiveHeaderId = p.JobReceiveHeaderId,
                          LossQty = p.LossQty,
                          LotNo = p.LotNo,
                          PassQty = p.PassQty,
                          PenaltyAmt = p.PenaltyAmt,
                          ProductId = p.JobOrderLine.ProductId,
                          ProductName = p.JobOrderLine.Product.ProductName,
                          UnitName = p.JobOrderLine.Product.Unit.UnitName,
                          ProductUidId = p.ProductUidId,
                          ProductUidName = p.ProductUid.ProductUidName,
                          ReceiveQty = p.Qty,
                          StockId = p.StockId,
                          StockProcessId = p.StockProcessId,
                          UnitDecimalPlaces = p.JobOrderLine.Product.Unit.DecimalPlaces,
                          JobOrderLineId = p.JobOrderLineId,
                          OrderDocTypeId = tab.JobOrderHeader.DocTypeId,
                          OrderHeaderId = tab.JobOrderHeaderId,
                      }
                        );

            return pt;


        }
        public JobReceiveLineViewModel GetJobReceiveLine(int id)
        {
            return (from p in db.JobReceiveLine
                    join t in db.ViewJobOrderBalance on p.JobOrderLineId equals t.JobOrderLineId into table
                    from tab in table.DefaultIfEmpty()
                    where p.JobReceiveLineId == id
                    select new JobReceiveLineViewModel
                    {
                        JobOrderHeaderDocNo = p.JobOrderLine.JobOrderHeader.DocNo,
                        JobOrderLineId = p.JobOrderLineId,
                        JobReceiveHeaderDocNo = p.JobReceiveHeader.DocNo,
                        JobReceiveHeaderId = p.JobReceiveHeaderId,
                        JobReceiveLineId = p.JobReceiveLineId,
                        ProductId = p.JobOrderLine.ProductId,
                        Dimension1Name = p.JobOrderLine.Dimension1.Dimension1Name,
                        Dimension2Name = p.JobOrderLine.Dimension2.Dimension2Name,
                        Specification = p.JobOrderLine.Specification,
                        OrderBalanceQty = tab.BalanceQty + p.Qty + p.LossQty,
                        UnitDecimalPlaces = p.JobOrderLine.Product.Unit.DecimalPlaces,
                        DealUnitDecimalPlaces = p.JobOrderLine.DealUnit.DecimalPlaces,
                        UnitConversionMultiplier = p.JobOrderLine.UnitConversionMultiplier,
                        UnitId = p.JobOrderLine.Product.UnitId,
                        DealUnitId = p.JobOrderLine.DealUnitId,
                        DealQty = p.PassQty * p.JobOrderLine.UnitConversionMultiplier,
                        ProductUidId = p.ProductUidId,
                        ProductUidName = p.ProductUid.ProductUidName,
                        IncentiveAmt = p.IncentiveAmt,
                        LossQty = p.LossQty,
                        LotNo = p.LotNo,
                        PassQty = p.PassQty,
                        PenaltyAmt = p.PenaltyAmt,
                        DocQty = p.Qty + p.LossQty,
                        ReceiveQty = p.Qty,
                        Weight = p.Weight,
                        Qty = p.Qty + p.LossQty,
                        Remark = p.Remark,
                        JobWorkerId = p.JobOrderLine.JobOrderHeader.JobWorkerId
                    }
                        ).FirstOrDefault();
        }
        public IEnumerable<JobReceiveLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<JobReceiveLine>()
                .Query()
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<JobReceiveLine> GetJobReceiveLineList()
        {
            var pt = _unitOfWork.Repository<JobReceiveLine>().Query().Get();

            return pt;
        }

        public JobReceiveLine Add(JobReceiveLine pt)
        {
            _unitOfWork.Repository<JobReceiveLine>().Insert(pt);
            return pt;
        }


        public JobReceiveLineViewModel GetJobReceiveDetailBalance(int id)
        {
            var temp = (from b in db.ViewJobReceiveBalance
                        join p in db.JobReceiveLine on b.JobReceiveLineId equals p.JobReceiveLineId
                        join t in db.JobOrderLine on p.JobOrderLineId equals t.JobOrderLineId into table
                        from tab in table.DefaultIfEmpty()
                        where b.JobReceiveLineId == id
                        select new JobReceiveLineViewModel
                        {
                            JobReceiveHeaderDocNo = b.JobReceiveNo,
                            JobReceiveLineId = b.JobReceiveLineId,
                            Qty = b.BalanceQty,
                            Remark = p.Remark,
                            Rate = tab.Rate,
                            Amount = tab.Rate * (b.BalanceQty * tab.UnitConversionMultiplier),
                            Dimension1Id = tab.Dimension1Id,
                            Dimension1Name = tab.Dimension1.Dimension1Name,
                            Dimension2Id = tab.Dimension2Id,
                            Dimension2Name = tab.Dimension2.Dimension2Name,
                            ProductId = tab.ProductId,
                            ProductName = tab.Product.ProductName,
                            Specification = tab.Specification,
                            LotNo = p.LotNo,
                            UnitConversionMultiplier = tab.UnitConversionMultiplier,
                            UnitId = tab.Product.UnitId,
                            UnitName = tab.Product.Unit.UnitName,
                            DealUnitId = tab.DealUnitId,
                            DealUnitName = tab.DealUnit.UnitName,
                            DealQty = b.BalanceQty * tab.UnitConversionMultiplier,
                            Weight = (p.Weight / p.Qty) * b.BalanceQty,
                        }).FirstOrDefault();
            return temp;

        }

        public JobReceiveLineViewModel GetJobReceiveDetailBalanceForInvoice(int id)
        {

            var temp = (from b in db.ViewJobReceiveBalanceForInvoice
                        join p in db.JobReceiveLine on b.JobReceiveLineId equals p.JobReceiveLineId
                        join t in db.JobOrderLine on p.JobOrderLineId equals t.JobOrderLineId into table
                        from tab in table.DefaultIfEmpty()
                        where b.JobReceiveLineId == id
                        select new JobReceiveLineViewModel
                        {
                            JobReceiveHeaderDocNo = b.JobReceiveNo,
                            JobReceiveLineId = b.JobReceiveLineId,
                            Qty = b.BalanceQty,
                            Remark = p.Remark,
                            Rate = tab.Rate,
                            Amount = tab.Rate * (b.BalanceQty * tab.UnitConversionMultiplier),
                            Dimension1Id = tab.Dimension1Id,
                            Dimension1Name = tab.Dimension1.Dimension1Name,
                            Dimension2Id = tab.Dimension2Id,
                            Dimension2Name = tab.Dimension2.Dimension2Name,
                            ProductId = tab.ProductId,
                            ProductName = tab.Product.ProductName,
                            Specification = tab.Specification,
                            LotNo = p.LotNo,
                            UnitConversionMultiplier = tab.UnitConversionMultiplier,
                            UnitId = tab.Product.UnitId,
                            UnitName = tab.Product.Unit.UnitName,
                            DealUnitId = tab.DealUnitId,
                            DealUnitName = tab.DealUnit.UnitName,
                            DealQty = b.BalanceQty * tab.UnitConversionMultiplier,
                            JobWorkerId = b.JobWorkerId,
                            CostCenterId = tab.JobOrderHeader.CostCenterId != null ? tab.JobOrderHeader.CostCenterId : null,
                            CostCenterName = tab.JobOrderHeader.CostCenterId != null ? tab.JobOrderHeader.CostCenter.CostCenterName : null,
                        }).FirstOrDefault();

            var JobOrderLineId = (from p in db.JobReceiveLine
                                  where p.JobReceiveLineId == temp.JobReceiveLineId
                                  join t in db.JobOrderLine on p.JobOrderLineId equals t.JobOrderLineId
                                  select new { LineId = p.JobOrderLineId, HeaderId = t.JobOrderHeaderId }).FirstOrDefault();


            var Charges = (from p in db.JobOrderLineCharge
                           where p.LineTableId == JobOrderLineId.LineId
                           join t in db.Charge on p.ChargeId equals t.ChargeId
                           select new LineCharges
                           {
                               ChargeCode = t.ChargeCode,
                               Rate = p.Rate,
                           }).ToList();

            var HeaderCharges = (from p in db.JobOrderHeaderCharges
                                 where p.HeaderTableId == JobOrderLineId.HeaderId
                                 join t in db.Charge on p.ChargeId equals t.ChargeId
                                 select new HeaderCharges
                                 {
                                     ChargeCode = t.ChargeCode,
                                     Rate = p.Rate,
                                 }).ToList();

            temp.RHeaderCharges = HeaderCharges;
            temp.RLineCharges = Charges;

            return (temp);

        }


        public IQueryable<ProductHelpListViewModel> GetPendingProductsForJobReceive(int Id, string term)//DocTypeId
        {

            var JobReceive = new JobReceiveHeaderService(_unitOfWork).Find(Id);

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(JobReceive.DocTypeId, JobReceive.DivisionId, JobReceive.SiteId);

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            ////int[] ProdTypes = settings.filterProductTypes.Split(',').Select(Int32.Parse).ToList().ToArray();

            //string[] ContraSites = null;
            //if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            //else { ContraSites = new string[] { "NA" }; }

            //// var ConSites = settings.filterContraSites.Split(',').Select(Int32.Parse).ToList().ToArray();

            //string[] ContraDivisions = null;
            //if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            //else { ContraDivisions = new string[] { "NA" }; }

            ////var ConDivisions = settings.filterContraDivisions.Split(',').Select(Int32.Parse).ToList().ToArray();

            var ProductNatures = (from p in db.ProductNature
                                  where p.ProductNatureName == ProductNatureConstants.FinishedMaterial || p.ProductNatureName == ProductNatureConstants.Rawmaterial
                                  select p.ProductNatureId).ToArray();

            return (from p in db.Product
                    where (string.IsNullOrEmpty(settings.filterProductTypes) ? ProductNatures.Contains(p.ProductGroup.ProductType.ProductNatureId) : ProductTypes.Contains(p.ProductGroup.ProductTypeId.ToString()))
                    && p.ProductName.ToLower().Contains(term.ToLower())
                    group new { p } by p.ProductId into g
                    orderby g.Key
                    select new ProductHelpListViewModel
                    {
                        ProductId = g.Key,
                        ProductName = g.Max(m => m.p.ProductName),
                        //Dimension1Name=g.Max(m=>m.p.Dimension1.Dimension1Name),
                        //Dimension2Name=g.Max(m=>m.p.Dimension2.Dimension2Name),
                        //Specification="Test specification",
                    }
                        );
        }

        public IQueryable<ComboBoxList> GetPendingJobOrders(int id, string term)//DocTypeId
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var header = new JobReceiveHeaderService(_unitOfWork).Find(id);

            var Settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(header.DocTypeId, DivisionId, SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(Settings.filterContraSites)) { ContraSites = Settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            // var ConSites = settings.filterContraSites.Split(',').Select(Int32.Parse).ToList().ToArray();

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(Settings.filterContraDivisions)) { ContraDivisions = Settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(Settings.filterContraDocTypes)) { ContraDocTypes = Settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            return (from p in db.ViewJobOrderBalance
                    join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId into ProdTable
                    from ProTab in ProdTable.DefaultIfEmpty()
                    where (string.IsNullOrEmpty(Settings.filterContraDocTypes) ? 1 == 1 : ContraDocTypes.Contains(ProTab.DocTypeId.ToString())) && p.BalanceQty > 0 && ProTab.DocNo.ToLower().Contains(term.ToLower()) && p.JobWorkerId == header.JobWorkerId
                     && (string.IsNullOrEmpty(Settings.filterContraSites) ? p.SiteId == header.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                     && (string.IsNullOrEmpty(Settings.filterContraDivisions) ? p.DivisionId == header.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                    group new { p, ProTab } by p.JobOrderHeaderId into g
                    orderby g.Key
                    select new ComboBoxList
                    {
                        Id = g.Key,
                        PropFirst = g.Max(m => m.ProTab.DocNo),
                        PropSecond = g.Sum(m => m.p.BalanceQty).ToString(),
                    }
                        );
        }

        public IQueryable<ComboBoxResult> GetPendingCostCenters(int id, string term)//DocTypeId
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var header = new JobReceiveHeaderService(_unitOfWork).Find(id);

            var Settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(header.DocTypeId, DivisionId, SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(Settings.filterContraSites)) { ContraSites = Settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            // var ConSites = settings.filterContraSites.Split(',').Select(Int32.Parse).ToList().ToArray();

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(Settings.filterContraDivisions)) { ContraDivisions = Settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(Settings.filterContraDocTypes)) { ContraDocTypes = Settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            return (from p in db.ViewJobOrderBalance
                    join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId into ProdTable
                    from ProTab in ProdTable.DefaultIfEmpty()
                    where (string.IsNullOrEmpty(Settings.filterContraDocTypes) ? 1 == 1 : ContraDocTypes.Contains(ProTab.DocTypeId.ToString())) && p.BalanceQty > 0 && ProTab.CostCenter.CostCenterName.ToLower().Contains(term.ToLower()) && p.JobWorkerId == header.JobWorkerId
                     && (string.IsNullOrEmpty(Settings.filterContraSites) ? p.SiteId == header.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                     && (string.IsNullOrEmpty(Settings.filterContraDivisions) ? p.DivisionId == header.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                    && ProTab.CostCenterId != null
                    group new { p, ProTab } by ProTab.CostCenterId into g
                    orderby g.Key
                    select new ComboBoxResult
                    {
                        id = g.Key.ToString(),
                        text = g.Max(m => m.ProTab.CostCenter.CostCenterName),
                    }
                        );
        }

        public IEnumerable<ProcGetBomForWeavingViewModel> GetBomPostingDataForWeaving(int ProductId, int? Dimension1Id, int? Dimension2Id, int ProcessId, decimal Qty, int DocTypeId, string ProcName, int? JobOrderLineId, Decimal? Weight)
        {
            SqlParameter SQLDocTypeID = new SqlParameter("@DocTypeId", DocTypeId);
            SqlParameter SQLProductID = new SqlParameter("@ProductId", ProductId);
            SqlParameter SQLProcessId = new SqlParameter("@ProcessId", ProcessId);
            SqlParameter SQLQty = new SqlParameter("@Qty", Qty);
            SqlParameter SQLDime1 = new SqlParameter("@Dimension1Id", Dimension1Id == null ? DBNull.Value : (object)Dimension1Id);
            SqlParameter SQLDime2 = new SqlParameter("@Dimension2Id", Dimension2Id == null ? DBNull.Value : (object)Dimension2Id);
            SqlParameter SQLJobOrderLineId = new SqlParameter("@JobOrderLineId", JobOrderLineId);
            SqlParameter SQLWeight = new SqlParameter("@Weight", Weight);

            List<ProcGetBomForWeavingViewModel> BomForWeaving = new List<ProcGetBomForWeavingViewModel>();

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            //string ProcName = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId).SqlProcConsumption;

            //PendingOrderQtyForPacking = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".ProcGetBomForWeaving @DocTypeId, @ProductId, @ProcessId,@Qty"+(Dimension1Id==null?"":",@Dimension1Id")+(Dimension2Id==null?"":",@Dimension2Id"), SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, (Dimension1Id==null? "" : Dimension1Id), (Dimension2Id)).ToList();


            if (JobOrderLineId != null && Weight != null && Weight != 0)
            {
                BomForWeaving = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty,@Dimension1Id, @Dimension2Id, @JobOrderLineId, @Weight", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, SQLDime1, SQLDime2, SQLJobOrderLineId, SQLWeight).ToList();
            }
            else if (Dimension1Id == null && Dimension2Id == null)
            {
                BomForWeaving = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty).ToList();
            }
            else if (Dimension1Id == null && Dimension2Id != null)
            {
                BomForWeaving = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty,@Dimension2Id", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, SQLDime2).ToList();
            }
            else if (Dimension1Id != null && Dimension2Id == null)
            {
                BomForWeaving = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty,@Dimension1Id", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, SQLDime1).ToList();
            }
            else
            {
                BomForWeaving = db.Database.SqlQuery<ProcGetBomForWeavingViewModel>("" + ProcName + " @DocTypeId, @ProductId, @ProcessId,@Qty,@Dimension1Id, @Dimension2Id", SQLDocTypeID, SQLProductID, SQLProcessId, SQLQty, SQLDime1, SQLDime2).ToList();
            }


            return BomForWeaving;

        }


        public IQueryable<JobReceiveBomViewModel> GetConsumptionLineListForIndex(int JobReceiveHeaderId)
        {
            var temp = from p in db.JobReceiveBom
                       join t in db.Dimension1 on p.Dimension1Id equals t.Dimension1Id into table
                       from Dim1 in table.DefaultIfEmpty()
                       join t1 in db.Dimension2 on p.Dimension2Id equals t1.Dimension2Id into table1
                       from Dim2 in table1.DefaultIfEmpty()
                       join t2 in db.Product on p.ProductId equals t2.ProductId into table2
                       from ProdTab in table2.DefaultIfEmpty()
                       orderby p.JobReceiveLineId
                       where p.JobReceiveHeaderId == JobReceiveHeaderId
                       select new JobReceiveBomViewModel
                       {
                           UnitName = ProdTab.Unit.UnitName,
                           UnitDecimalPlaces = ProdTab.Unit.DecimalPlaces,
                           Dimension1Name = Dim1.Dimension1Name,
                           Dimension2Name = Dim2.Dimension2Name,
                           JobReceiveBomId = p.JobReceiveBomId,
                           JobReceiveHeaderId = p.JobReceiveHeaderId,
                           JobReceiveLineId = p.JobReceiveLineId,
                           ProductName = ProdTab.ProductName,
                           Qty = p.Qty,
                           CostCenterName=p.CostCenter.CostCenterName,
                       };
            return temp;
        }

        public IQueryable<JobReceiveByProductViewModel> GetByProductListForIndex(int JobReceiveHeaderId)
        {
            var temp = from p in db.JobReceiveByProduct
                       join t in db.Dimension1 on p.Dimension1Id equals t.Dimension1Id into table
                       from Dim1 in table.DefaultIfEmpty()
                       join t1 in db.Dimension2 on p.Dimension2Id equals t1.Dimension2Id into table1
                       from Dim2 in table1.DefaultIfEmpty()
                       join t2 in db.Product on p.ProductId equals t2.ProductId into table2
                       from ProdTab in table2.DefaultIfEmpty()
                       orderby p.JobReceiveByProductId
                       where p.JobReceiveHeaderId == JobReceiveHeaderId
                       select new JobReceiveByProductViewModel
                       {
                           UnitName = ProdTab.Unit.UnitName,
                           UnitDecimalPlaces = ProdTab.Unit.DecimalPlaces,
                           Dimension1Name = Dim1.Dimension1Name,
                           Dimension2Name = Dim2.Dimension2Name,
                           JobReceiveByProductId = p.JobReceiveByProductId,
                           JobReceiveHeaderId = p.JobReceiveHeaderId,
                           ProductName = ProdTab.ProductName,
                           Qty = p.Qty,
                       };
            return temp;
        }

        public IEnumerable<ComboBoxList> GetProductHelpList(int Id, string term)
        {
            var JobReceive = new JobReceiveHeaderService(_unitOfWork).Find(Id);

            var settings = new JobReceiveSettingsService(_unitOfWork).GetJobReceiveSettingsForDocument(JobReceive.DocTypeId, JobReceive.DivisionId, JobReceive.SiteId);

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            var list = (from p in db.Product
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductName.ToLower().Contains(term.ToLower()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.ProductGroup.ProductTypeId.ToString()))
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
            var Max = (from p in db.JobReceiveLine
                       where p.JobReceiveHeaderId == id
                       select p.Sr
                        );

            if (Max.Count() > 0)
                return Max.Max(m => m ?? 0) + 1;
            else
                return (1);
        }


        public string GetFirstBarCodeForCancel(int JobOrderLineId)
        {
            return (from p in db.JobOrderLine
                    where p.JobOrderLineId == JobOrderLineId
                    join t in db.ProductUid on p.ProductUidHeaderId equals t.ProductUidHeaderId
                    join t2 in db.JobOrderCancelLine on t.ProductUIDId equals t2.ProductUidId into table
                    from tab in table.DefaultIfEmpty()
                    where tab == null
                    select p.ProductUidId).FirstOrDefault().ToString();
        }

        public void Dispose()
        {
        }

        public List<ComboBoxList> GetPendingBarCodesList(int id)
        {
            List<ComboBoxList> Barcodes = new List<ComboBoxList>();

            var JobOrderline = new JobOrderLineService(_unitOfWork).Find(id);
            var JobOrderHeader = new JobOrderHeaderService(_unitOfWork).Find(JobOrderline.JobOrderHeaderId);

            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                //context.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;

                //context.Database.CommandTimeout = 30000;

                var Temp = (from p in context.ProductUid
                            join t in context.JobReceiveLine on p.ProductUIDId equals t.ProductUidId into table
                            from tab in table.DefaultIfEmpty()
                            join t3 in context.JobReceiveHeader.Where(m => m.SiteId == JobOrderHeader.SiteId && m.DivisionId == JobOrderHeader.DivisionId) on tab.JobReceiveHeaderId equals t3.JobReceiveHeaderId into table5
                            from JRH in table5.DefaultIfEmpty()
                            //where p.ProductUidHeaderId == JobOrderline.ProductUidHeaderId && JRH == null
                            //&& p.Status != ProductUidStatusConstants.Return && p.Status != ProductUidStatusConstants.Cancel && ((p.GenDocId == p.LastTransactionDocId && p.GenDocNo == p.LastTransactionDocNo && p.GenPersonId == p.LastTransactionPersonId) || p.CurrenctGodownId != null)
                            //orderby p.ProductUIDId
                            join t2 in context.JobOrderLine on p.ProductUidHeaderId equals t2.ProductUidHeaderId
                            join JOH in context.JobOrderHeader.Where(m => m.SiteId == JobOrderHeader.SiteId && m.DivisionId == JobOrderHeader.DivisionId) on t2.JobOrderHeaderId equals JOH.JobOrderHeaderId
                            join RecLineStatus in context.JobReceiveLineStatus on tab.JobReceiveLineId equals RecLineStatus.JobReceiveLineId into RecLineStatTab
                            from RecLineStat in RecLineStatTab.DefaultIfEmpty()
                            where p.ProductUidHeaderId == JobOrderline.ProductUidHeaderId && (JRH == null || ((tab.Qty - (RecLineStat.ReturnQty ?? 0)) == 0)) && p.Status != ProductUidStatusConstants.Return &&
                            p.Status != ProductUidStatusConstants.Cancel && ((p.GenPersonId == p.LastTransactionPersonId) || p.CurrenctGodownId != null)
                            && JOH.ProcessId == JobOrderHeader.ProcessId
                            orderby p.ProductUIDId
                            select new { Id = p.ProductUIDId, Name = p.ProductUidName }).ToList();
                foreach (var item in Temp)
                {
                    Barcodes.Add(new ComboBoxList
                    {
                        Id = item.Id,
                        PropFirst = item.Name,
                    });
                }
            }



            return Barcodes;
        }

        public List<ComboBoxList> GetPendingBarCodesList(int[] id)
        {
            List<ComboBoxList> Barcodes = new List<ComboBoxList>();

            //var LineIds = id.Split(',').Select(Int32.Parse).ToArray();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                //context.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;

                //context.Database.CommandTimeout = 30000;

                var Temp = (from p in context.ViewJobOrderBalance
                            join t in context.ProductUid on p.ProductUidId equals t.ProductUIDId
                            join t2 in context.JobOrderLine on p.JobOrderLineId equals t2.JobOrderLineId
                            where id.Contains(p.JobOrderLineId) && p.ProductUidId != null
                            orderby t2.Sr
                            select new { Id = t.ProductUIDId, Name = t.ProductUidName }).ToList();
                foreach (var item in Temp)
                {
                    Barcodes.Add(new ComboBoxList
                    {
                        Id = item.Id,
                        PropFirst = item.Name,
                    });
                }
            }



            return Barcodes;
        }

        public Task<IEquatable<JobReceiveLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<JobReceiveLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
