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
using System.Data.SqlClient;
using System.Configuration;


namespace Surya.India.Service
{
    public interface IStockLineService : IDisposable
    {
        StockLine Create(StockLine s);
        void Delete(int id);
        void Delete(StockLine s);
        StockLineViewModel GetStockLine(int id);
        StockLineViewModel GetStockLineForIssue(int id);
        StockLineViewModel GetStockLineForReceive(int id);
        StockLine Find(int id);
        void Update(StockLine s);
        IEnumerable<StockLine> GetStockLineforDelete(int id);
        IEnumerable<StockLineViewModel> GetStockLineListForIndex(int id);
        IEnumerable<StockLineViewModel> GetStockLineListForIssueReceive(int id, bool Issue);
        IEnumerable<ProductBalanceForProcessViewModel> GetProcessBalanceProducts(int ProcessId, int? CostCenterId, int HeaderId);
        IEnumerable<StockLineViewModel> GetJobConsumptionForFilters(StockLineFilterViewModel vm);
        IEnumerable<ComboBoxList> GetPendingProdFromStocProcBal(int? CostCenterId, int ProcessId, int JobWorkerId, string term);
        IEnumerable<ComboBoxList> GetPendingCCFromStockProcBal(int StockHeaderId, int? CostCenterId, int ProcessId, int JobWorkerId, string term);
        StockLineViewModel GetJobConsumptionLine(int id);
        StockLineViewModel GetRateConversionLine(int id);
        RequisitionLineViewModel GetRequisitionBalanceForIssue(int id);
        IEnumerable<ProductHelpListViewModel> GetProductHelpList(int Id, int PersonId, string term, int Limit);
        IEnumerable<ComboBoxList> GetPendingRequisitionHelpList(int Id, int PersonId, string term);
        IEnumerable<ComboBoxList> GetProductHelpListForFilters(int Id, int PersonId, string term, int Limit);
        IEnumerable<StockLineViewModel> GetRequisitionsForFilters(RequisitionFiltersForIssue vm, bool All);
        IEnumerable<StockExchangeLineViewModel> GetRequisitionsForExchange(RequisitionFiltersForExchange vm);
        IEnumerable<StockReceiveLineViewModel> GetRequisitionsForReceive(RequisitionFiltersForReceive vm);
        IEnumerable<ComboBoxList> GetPendingCostCenterHelpList(int Id, int PersonId, string term);
        decimal GetExcessStock(int ProductId, int? Dim1, int? Dim2, int? ProcId, string Lot, int MaterilIssueId, string ProcName);
        IQueryable<ComboBoxResult> GetCostCentersForIssueFilters(int id, string term);
        IQueryable<ComboBoxResult> GetCostCentersForLine(int id, string term);
        IEnumerable<StockExchangeLineViewModel> GetProcessTransfersForFilters(FiltersForProcessTransfer vm);
        IQueryable<ComboBoxResult> GetCostCentersForProcessTransfer(int SiteId, int DivisionId, string FilterDocTypes, string term, int? PersonId);
        IEnumerable<StockLineViewModel> GetStockLineListForTransfer(int id);
        int GetMaxSr(int id);
        IQueryable<ComboBoxResult> GetCustomProducts(int Id, string term);
        IQueryable<ComboBoxResult> GetRequisitionsForReceive(int Id, string term);

    }

    public class StockLineService : IStockLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public StockLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public StockLine Create(StockLine S)
        {
            S.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<StockLine>().Insert(S);
            return S;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<StockLine>().Delete(id);
        }

        public void Delete(StockLine s)
        {
            _unitOfWork.Repository<StockLine>().Delete(s);
        }

        public void Update(StockLine s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<StockLine>().Update(s);
        }


        public StockLineViewModel GetStockLine(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            PersonId = p.StockHeader.PersonId,
                            CostCenterId = p.CostCenterId,
                        }

                        ).FirstOrDefault();

            return temp;
        }


        public StockLineViewModel GetStockLineForReceive(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            PersonId = p.StockHeader.PersonId,
                            CostCenterId = p.CostCenterId,
                        }

                        ).FirstOrDefault();

            if (temp.RequisitionLineId != null)
            {
                var Rec = (from p in db.RequisitionLineStatus
                           where p.RequisitionLineId == p.RequisitionLineId
                           select p).FirstOrDefault();
                if (Rec != null)
                    temp.RequisitionBalanceQty = Rec.IssueQty;
            }

            return temp;
        }

        public StockLineViewModel GetStockLineForIssue(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        join t in db.ViewRequisitionBalance on p.RequisitionLineId equals t.RequisitionLineId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            RequiredProductId = p.RequisitionLine.ProductId,
                            RequiredProductName = p.RequisitionLine.Product.ProductName,
                            RequisitionBalanceQty = p.Qty + tab2.BalanceQty,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            PersonId = p.StockHeader.PersonId,
                            CostCenterId = p.CostCenterId,

                        }

                        ).FirstOrDefault();


            if (temp.RequisitionLineId != null)
            {
                temp.RequisitionBalanceQty = (from p in db.ViewMaterialRequestBalance
                                              where p.RequisitionLineId == temp.RequisitionLineId
                                              select p.BalanceQty
                                                ).FirstOrDefault() + temp.Qty;
            }

            return temp;
        }

        public StockLine Find(int id)
        {
            return _unitOfWork.Repository<StockLine>().Find(id);
        }

        public IEnumerable<StockLine> GetStockLineforDelete(int id)
        {
            return (from p in db.StockLine
                    where p.StockHeaderId == id
                    select p
                        );
        }

        public IEnumerable<StockLineViewModel> GetStockLineListForIndex(int id)
        {

            return (from p in db.StockLine
                    where p.StockHeaderId == id
                    orderby p.Sr
                    select new StockLineViewModel
                    {
                        ProductName = p.Product.ProductName,
                        Specification = p.Specification,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        LotNo = p.LotNo,
                        RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                        Qty = p.Qty,
                        UnitId = p.Product.UnitId,
                        UnitName = p.Product.Unit.UnitName,
                        Rate = p.Rate,
                        Amount = p.Amount,
                        Remark = p.Remark,
                        StockLineId = p.StockLineId,
                        StockHeaderId = p.StockHeaderId,
                        UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                        ProductUidIdName = p.ProductUid.ProductUidName,
                        FromProcessName = p.FromProcess.ProcessName,
                        CostCenterName = p.CostCenter.CostCenterName,
                    }
                        );

        }

        public IEnumerable<StockLineViewModel> GetStockLineListForIssueReceive(int id, bool Issue)
        {

            return (from p in db.StockLine
                    //join t in db.Stock on p.StockId equals t.StockId
                    //where p.StockHeaderId == id && (Issue ? t.Qty_Iss > 0 : t.Qty_Rec > 0)
                    where p.StockHeaderId == id && (Issue ? p.DocNature == StockNatureConstants.Issue : p.DocNature == StockNatureConstants.Receive)
                    orderby p.Sr
                    select new StockLineViewModel
                    {
                        ProductName = p.Product.ProductName,
                        Specification = p.Specification,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        LotNo = p.LotNo,
                        RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                        Qty = p.Qty,
                        UnitId = p.Product.UnitId,
                        UnitName = p.Product.Unit.UnitName,
                        Rate = p.Rate,
                        Amount = p.Amount,
                        Remark = p.Remark,
                        StockLineId = p.StockLineId,
                        StockHeaderId = p.StockHeaderId,
                        UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                        ProductUidIdName = p.ProductUid.ProductUidName,
                        FromProcessName = p.FromProcess.ProcessName,
                        CostCenterName = p.CostCenter.CostCenterName,

                    }
                        );

        }


        public IEnumerable<StockLineViewModel> GetStockLineListForTransfer(int id)
        {

            return (from p in db.StockLine
                    //join t in db.Stock on p.StockId equals t.StockId
                    //where p.StockHeaderId == id && (Issue ? t.Qty_Iss > 0 : t.Qty_Rec > 0)
                    where p.StockHeaderId == id && p.DocNature == StockNatureConstants.Transfer
                    select new StockLineViewModel
                    {
                        ProductName = p.Product.ProductName,
                        Specification = p.Specification,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        LotNo = p.LotNo,
                        RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                        Qty = p.Qty,
                        UnitId = p.Product.UnitId,
                        UnitName = p.Product.Unit.UnitName,
                        Rate = p.Rate,
                        Amount = p.Amount,
                        Remark = p.Remark,
                        StockLineId = p.StockLineId,
                        StockHeaderId = p.StockHeaderId,
                        UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                        ProductUidIdName = p.ProductUid.ProductUidName,
                        FromProcessName = p.FromProcess.ProcessName,
                        CostCenterName = p.CostCenter.CostCenterName,

                    }
                        );

        }



        public IEnumerable<ProductBalanceForProcessViewModel> GetProcessBalanceProducts(int ProcessId, int? CostCenterId, int HeaderId)
        {
            //SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);
            //SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", CostCenterId);

            //IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".ProcStockProcessBalance @ProcessId, @CostCenterId", SqlParameterProcessId, SqlParameterCostCenterId).ToList();

            StockHeader Header = new StockHeaderService(_unitOfWork).Find(HeaderId);

            SqlParameter SqlParameterPersonIdId = new SqlParameter("@PersonId", Header.PersonId);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", CostCenterId);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate, @SiteId, @DivisionId, @PersonId, @ProcessId, @CostCenterId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonIdId, SqlParameterProcessId, SqlParameterCostCenterId).ToList();


            return FifoSaleOrderLineList;
        }

        public IEnumerable<StockLineViewModel> GetJobConsumptionForFilters(StockLineFilterViewModel vm)
        {

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }

            string[] Dimension1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dimension1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }

            string[] Dimension2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dimension2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }

            string[] CostCenterId = null;
            if (!string.IsNullOrEmpty(vm.CostCenterIds)) { CostCenterId = vm.CostCenterIds.Split(",".ToCharArray()); }

            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", vm.ProcessId);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", vm.JobWorkerId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", vm.CostCenterId);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<StockLineViewModel> StockProcessBalance = db.Database.SqlQuery<StockLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate,@SiteId,@DivisionId,@PersonId,@ProcessId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId).ToList();

            var temp = from H in StockProcessBalance
                       join Proc in db.Process on H.ProcessId equals Proc.ProcessId into table
                       from tab in table.DefaultIfEmpty()
                       where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(H.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(H.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dimension1IdArr.Contains(H.Dimension1Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dimension2IdArr.Contains(H.Dimension2Id.ToString()))
                        && (string.IsNullOrEmpty(vm.CostCenterIds) ? 1 == 1 : CostCenterId.Contains(H.CostCenterId.ToString()))
                        && H.BalanceQty != 0
                       select new StockLineViewModel
                       {
                           StockHeaderId = vm.StockHeaderId,
                           Dimension1Name = H.Dimension1Name,
                           Dimension1Id = H.Dimension1Id,
                           Dimension2Id = H.Dimension2Id,
                           Dimension2Name = H.Dimension2Name,
                           Specification = H.Specification,
                           Qty = - H.BalanceQty,
                           BalanceQty = H.BalanceQty,
                           UnitName = H.UnitName,
                           UnitDecimalPlaces = H.UnitDecimalPlaces,
                           //UnitId=P.UnitId,
                           Rate = vm.Rate,
                           ProductName = H.ProductName,
                           ProductId = H.ProductId,
                           FromProcessId = tab == null ? null : (int?)tab.ProcessId,
                           ProcessName = tab == null ? null : tab.ProcessName,
                           CostCenterId = H.CostCenterId,
                           CostCenterName = H.CostCenterName,
                           LotNo = H.LotNo
                       };

            return temp;


        }

        public IEnumerable<ComboBoxList> GetPendingProdFromStocProcBal(int? CostCenterId, int ProcessId, int JobWorkerId, string term)//DocTypeId
        {

            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", CostCenterId);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", JobWorkerId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate,@SiteId,@DivisionId,@PersonId, @ProcessId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId).ToList();



            return (from p in FifoSaleOrderLineList
                    where p.BalanceQty != 0 && (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductName.ToLower().Contains(term.ToLower()))
                    group new { p } by p.ProductId into g
                    orderby g.Key descending
                    select new ComboBoxList
                    {
                        Id = g.Key,
                        PropFirst = g.Max(m => m.p.ProductName)
                    }
                        );
        }

        public IEnumerable<ComboBoxList> GetPendingCCFromStockProcBal(int StockHeaderId, int? CostCenterId, int ProcessId, int JobWorkerId, string term)//DocTypeId
        {

            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", CostCenterId);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", JobWorkerId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            StockHeader header = new StockHeaderService(_unitOfWork).Find(StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);

            string ProcName = "";

            if (string.IsNullOrEmpty(settings.SqlProcStockProcessBalance))
            {
                ProcName = "Web.spStockProcessBalance";
            }
            else
            {
                ProcName = settings.SqlProcStockProcessBalance;
            }

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>(ProcName  + "  @UptoDate,@SiteId,@DivisionId,@PersonId, @ProcessId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId).ToList();



            return (from p in FifoSaleOrderLineList
                    where p.BalanceQty != 0 && p.CostCenterName != null && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
                    group new { p } by p.CostCenterId into g
                    orderby g.Key descending
                    where g.Key != null
                    select new ComboBoxList
                    {
                        Id = g.Key.Value,
                        PropFirst = g.Max(m => m.p.CostCenterName)
                    }
                        ).Take(40).ToList();
        }

        public StockLineViewModel GetJobConsumptionLine(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            UnitId = p.Product.UnitId,
                            CostCenterId = p.CostCenterId,
                            CostCenterName = p.CostCenter.CostCenterName,

                        }

                        ).FirstOrDefault();



            StockHeader header = new StockHeaderService(_unitOfWork).Find(temp.StockHeaderId);

            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", header.ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", (object)temp.CostCenterId ?? DBNull.Value);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", header.PersonId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate, @SiteId, @DivisionId, @PersonId, @ProcessId, @CostCenterId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId, SqlParameterCostCenterId).ToList();

            temp.BalanceQty = (from p in FifoSaleOrderLineList
                               where p.BalanceQty > 0 && p.Dimension1Id == temp.Dimension1Id && p.Dimension2Id == temp.Dimension2Id && p.Specification == temp.Specification && p.LotNo == temp.LotNo
                               select p.BalanceQty).FirstOrDefault() + temp.Qty;

            return temp;

        }


        public StockLineViewModel GetRateConversionLine(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            UnitId = p.Product.UnitId,
                            CostCenterId = p.CostCenterId,
                            CostCenterName = p.CostCenter.CostCenterName,

                        }

                        ).FirstOrDefault();



            StockHeader header = new StockHeaderService(_unitOfWork).Find(temp.StockHeaderId);

            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", header.ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", (object)temp.CostCenterId ?? DBNull.Value);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", header.PersonId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate, @SiteId, @DivisionId, @PersonId, @ProcessId, @CostCenterId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId, SqlParameterCostCenterId).ToList();


            temp.BalanceQty = (from p in FifoSaleOrderLineList
                               where p.BalanceQty > 0 && p.Dimension1Id == temp.Dimension1Id && p.Dimension2Id == temp.Dimension2Id && p.Specification == temp.Specification && p.LotNo == temp.LotNo
                               select p.BalanceQty).FirstOrDefault() + temp.Qty;

            return temp;

        }


        public RequisitionLineViewModel GetRequisitionBalanceForIssue(int id)
        {
            return (from b in db.ViewRequisitionBalance
                    join p in db.RequisitionLine on b.RequisitionLineId equals p.RequisitionLineId
                    join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                    where b.RequisitionLineId == id
                    select new RequisitionLineViewModel
                    {
                        RequisitionHeaderDocNo = b.RequisitionNo,
                        RequisitionLineId = b.RequisitionLineId,
                        Qty = b.BalanceQty,
                        Remark = p.Remark,
                        Dimension1Id = p.Dimension1Id,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Id = p.Dimension2Id,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        ProductId = p.ProductId,
                        ProductName = p.Product.ProductName,
                        Specification = p.Specification,
                        UnitName = p.Product.Unit.UnitName,
                        ProcessId = p.ProcessId,
                        ProcessName = p.Process.ProcessName,
                        CostCenterId=t.CostCenterId,
                        CostCenterName=t.CostCenter.CostCenterName,
                    }).FirstOrDefault();

        }


        public IEnumerable<ProductHelpListViewModel> GetProductHelpList(int Id, int PersonId, string term, int Limit)
        {
            var Stock = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        join t2 in db.RequisitionLine on p.RequisitionLineId equals t2.RequisitionLineId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : t2.Specification.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.RequisitionNo.ToLower().Contains(term.ToLower())
                        )
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == SiteId : ContraSites.Contains(t.SiteId.ToString()))
                          && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == DivisionId : ContraDivisions.Contains(t.DivisionId.ToString()))
                        && p.PersonId == PersonId
                        orderby t.DocDate, t.DocNo
                        select new ProductHelpListViewModel
                        {
                            ProductName = p.Product.ProductName,
                            ProductId = p.ProductId,
                            Specification = t2.Specification,
                            Dimension1Name = p.Dimension1.Dimension1Name,
                            Dimension2Name = p.Dimension2.Dimension2Name,
                            HeaderDocNo = p.RequisitionNo,
                            LineId = p.RequisitionLineId,
                            BalanceQty = p.BalanceQty,
                        }
                          ).Take(Limit);

            return list.ToList();
        }


        public IEnumerable<ComboBoxList> GetPendingRequisitionHelpList(int Id, int PersonId, string term)
        {

            var GoodsReceipt = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(GoodsReceipt.DocTypeId, GoodsReceipt.DivisionId, GoodsReceipt.SiteId);

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

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.RequisitionNo.ToLower().Contains(term.ToLower())) && p.PersonId == GoodsReceipt.PersonId && p.BalanceQty > 0
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(t.DocTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                    && p.PersonId == PersonId
                        group new { p } by p.RequisitionHeaderId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.p.RequisitionNo),
                            Id = g.Key,
                        }
                          ).Take(20);

            return list.ToList();
        }


        public IEnumerable<ComboBoxList> GetPendingCostCenterHelpList(int Id, int PersonId, string term)
        {

            var GoodsReceipt = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(GoodsReceipt.DocTypeId, GoodsReceipt.DivisionId, GoodsReceipt.SiteId);

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

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.RequisitionNo.ToLower().Contains(term.ToLower())) && p.PersonId == GoodsReceipt.PersonId && p.BalanceQty > 0
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(t.DocTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                    && p.PersonId == PersonId
                        group new { p } by p.CostCenterId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.p.CostCenter.CostCenterName),
                            Id = (int)g.Key,
                        }
                          ).Take(20);

            return list.ToList();
        }


        public IEnumerable<ComboBoxList> GetProductHelpListForFilters(int Id, int PersonId, string term, int Limit)
        {
            var Stock = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        join t2 in db.RequisitionLine on p.RequisitionLineId equals t2.RequisitionLineId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(t.SiteId.ToString()))
                          && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(t.DivisionId.ToString()))
                        && p.PersonId == PersonId
                        orderby t.DocDate, t.DocNo
                        group p by p.ProductId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.Product.ProductName),
                            Id = g.Key,
                        }
                          ).Take(Limit);

            return list.ToList();
        }



        public IEnumerable<StockLineViewModel> GetRequisitionsForFilters(RequisitionFiltersForIssue vm, bool All)
        {

            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.RequisitionHeaderId)) { SaleOrderIdArr = vm.RequisitionHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dim1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dim1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dim1IdArr = new string[] { "NA" }; }

            string[] Dim2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dim2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dim2IdArr = new string[] { "NA" }; }

            string[] CostCenterArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterArr = new string[] { "NA" }; }

            var temp = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join t2 in db.RequisitionLine on p.RequisitionLineId equals t2.RequisitionLineId into table3
                        from tab3 in table3.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.RequisitionHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.RequisitionHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dim1IdArr.Contains(tab3.Dimension1Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dim2IdArr.Contains(tab3.Dimension2Id.ToString()))
                        && (string.IsNullOrEmpty(vm.CostCenterId) ? 1 == 1 : CostCenterArr.Contains(tab.CostCenterId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(tab.SiteId.ToString()))
                          && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(tab.DivisionId.ToString()))
                        && (All ? 1 == 1 : p.BalanceQty > 0) && p.PersonId == vm.PersonId
                        orderby p.ProductId, p.Dimension1Id, p.Dimension2Id
                        select new StockLineViewModel
                        {
                            Dimension1Name = tab3.Dimension1.Dimension1Name,
                            Dimension2Name = tab3.Dimension2.Dimension2Name,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            ProcessId = tab3.ProcessId,
                            Specification = tab3.Specification,
                            RequisitionBalanceQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            RequisitionHeaderDocNo = tab.DocNo,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            CostCenterId = p.CostCenterId,
                            StockHeaderId = vm.StockHeaderId,
                            RequisitionLineId = p.RequisitionLineId,
                            UnitId = tab2.UnitId,
                            UnitName = tab2.Unit.UnitName,
                            UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                        }



                        );
            return temp;
        }


        public IEnumerable<StockExchangeLineViewModel> GetRequisitionsForExchange(RequisitionFiltersForExchange vm)
        {

            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.RequisitionHeaderId)) { SaleOrderIdArr = vm.RequisitionHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dim1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dim1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dim1IdArr = new string[] { "NA" }; }

            string[] Dim2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dim2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dim2IdArr = new string[] { "NA" }; }

            string[] CostCenterArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterArr = new string[] { "NA" }; }

            var temp = (from p in db.RequisitionLine
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join t2 in db.StockLine on p.RequisitionLineId equals t2.RequisitionLineId into sltable
                        from stockline in sltable.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : (stockline == null ? ProductIdArr.Contains(p.ProductId.ToString()) : ProductIdArr.Contains(stockline.ProductId.ToString())))
                        && (string.IsNullOrEmpty(vm.RequisitionHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.RequisitionHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(p.Product.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : (stockline == null ? Dim1IdArr.Contains(p.Dimension1Id.ToString()) : Dim1IdArr.Contains(stockline.Dimension1Id.ToString())))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : (stockline == null ? Dim2IdArr.Contains(p.Dimension2Id.ToString()) : Dim2IdArr.Contains(stockline.Dimension2Id.ToString())))
                        && (string.IsNullOrEmpty(vm.CostCenterId) ? 1 == 1 : CostCenterArr.Contains(tab.CostCenterId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(tab.SiteId.ToString()))
                          && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(tab.DivisionId.ToString()))
                         && tab.PersonId == vm.PersonId
                        join t in db.ViewRequisitionBalance on p.RequisitionLineId equals t.RequisitionLineId into reqtable
                        from reqtab in reqtable.DefaultIfEmpty()
                        select new StockExchangeLineViewModel
                        {
                            Dimension1Name = (stockline == null ? p.Dimension1.Dimension1Name : stockline.Dimension1.Dimension1Name),
                            Dimension2Name = (stockline == null ? p.Dimension2.Dimension2Name : stockline.Dimension2.Dimension2Name),
                            Specification = (stockline == null ? p.Specification : stockline.Specification),
                            Dimension1Id = (stockline == null ? p.Dimension1Id : stockline.Dimension1Id),
                            Dimension2Id = (stockline == null ? p.Dimension2Id : stockline.Dimension2Id),
                            ProcessId = p.ProcessId,
                            RequisitionHeaderDocNo = tab.DocNo,
                            ProductName = (stockline == null ? p.Product.ProductName : stockline.Product.ProductName),
                            ProductId = p.ProductId,
                            CostCenterId = tab.CostCenterId,
                            CostCenterName = tab.CostCenter.CostCenterName,
                            StockHeaderId = vm.StockHeaderId,
                            RequisitionLineId = p.RequisitionLineId,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                            RequisitionBalanceQty = reqtab.BalanceQty,
                        }



                        );
            return temp;
        }



        public IEnumerable<StockReceiveLineViewModel> GetRequisitionsForReceive(RequisitionFiltersForReceive vm)
        {

            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.RequisitionHeaderId)) { SaleOrderIdArr = vm.RequisitionHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dim1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dim1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dim1IdArr = new string[] { "NA" }; }

            string[] Dim2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dim2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dim2IdArr = new string[] { "NA" }; }

            string[] CostCenterArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterArr = new string[] { "NA" }; }

             var temp = (from p in db.RequisitionLine
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join t2 in db.StockLine on p.RequisitionLineId equals t2.RequisitionLineId into sltable
                        from stockline in sltable.DefaultIfEmpty()
                        join t in db.RequisitionLineStatus on p.RequisitionLineId equals t.RequisitionLineId into reqtable
                        from reqtab in reqtable.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : (stockline == null ? ProductIdArr.Contains(p.ProductId.ToString()) : ProductIdArr.Contains(stockline.ProductId.ToString())))
                        && (string.IsNullOrEmpty(vm.RequisitionHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.RequisitionHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(p.Product.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : (stockline == null ? Dim1IdArr.Contains(p.Dimension1Id.ToString()) : Dim1IdArr.Contains(stockline.Dimension1Id.ToString())))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : (stockline == null ? Dim2IdArr.Contains(p.Dimension2Id.ToString()) : Dim2IdArr.Contains(stockline.Dimension2Id.ToString())))
                        && (string.IsNullOrEmpty(vm.CostCenterId) ? 1 == 1 : CostCenterArr.Contains(tab.CostCenterId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(tab.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(tab.DivisionId.ToString()))
                        where tab.PersonId == vm.PersonId && (reqtab.IssueQty ?? 0 - reqtab.ReceiveQty ?? 0) > 0 && stockline.DocNature == StockNatureConstants.Issue
                        group new { stockline, p, tab, reqtab } by new { stockline.RequisitionLineId, stockline.ProductId, stockline.Dimension1Id, stockline.Dimension2Id } into g
                        select new StockReceiveLineViewModel
                        {
                            Dimension1Name = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Dimension1.Dimension1Name) : g.Max(m => m.p.Dimension1.Dimension1Name)),
                            Dimension2Name = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Dimension2.Dimension2Name) : g.Max(m => m.p.Dimension2.Dimension2Name)),
                            Specification = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Specification) : g.Max(m => m.p.Specification)),
                            Dimension1Id = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Dimension1Id) : g.Max(m => m.p.Dimension1Id)),
                            Dimension2Id = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Dimension2Id) : g.Max(m => m.p.Dimension2Id)),
                            ProcessId = g.Max(m => m.p.ProcessId),
                            RequisitionHeaderDocNo = g.Max(m => m.tab.DocNo),
                            ProductName = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Product.ProductName) : g.Max(m => m.p.Product.ProductName)),
                            ProductId = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.ProductId) : g.Max(m => m.p.ProductId)),
                            CostCenterId = g.Max(m => m.tab.CostCenterId),
                            CostCenterName = g.Max(m => m.tab.CostCenter.CostCenterName),
                            StockHeaderId = vm.StockHeaderId,
                            RequisitionLineId = g.Max(m => m.p.RequisitionLineId),
                            UnitId = g.Max(m => m.p.Product.UnitId),
                            UnitName = g.Max(m => m.p.Product.Unit.UnitName),
                            UnitDecimalPlaces = g.Max(m => m.p.Product.Unit.DecimalPlaces),
                            RequisitionBalanceQty = g.Where(m => m.stockline != null).Any() ? g.Sum(m => m.stockline.Qty) : 0,
                        }



                        ).ToList();
            return temp;
        }


        public decimal GetExcessStock(int ProductId, int? Dim1, int? Dim2, int? ProcId, string Lot, int MaterilIssueId, string ProcName)
        {
            StockHeader Header = new StockHeaderService(_unitOfWork).Find(MaterilIssueId);
            decimal EXS = 0;
            if (string.IsNullOrEmpty(ProcName))
                EXS = 0;
            else
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString()))
                {
                    sqlConnection.Open();

                    SqlCommand Totalf = new SqlCommand("SELECT " + ProcName + "( " + ProductId + ", " + (!Dim1.HasValue ? "NULL" : "" + Dim1 + "") + ", " + (!Dim2.HasValue ? "NULL" : "" + Dim2 + "") + ", " + (!ProcId.HasValue ? "NULL" : "" + ProcId + "") + ", " + (string.IsNullOrEmpty(Lot) ? "NULL" : Lot) + ", " + Header.SiteId + ", NULL" + ", " + (!Header.GodownId.HasValue ? "NULL" : "" + Header.GodownId + "") + ")", sqlConnection);

                    EXS = Convert.ToDecimal(Totalf.ExecuteScalar() == DBNull.Value ? 0 : Totalf.ExecuteScalar());
                }
            }
            return EXS;
        }

        public IQueryable<ComboBoxResult> GetCostCentersForIssueFilters(int id, string term)
        {

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            return (from p in db.ViewRequisitionBalance
                    where p.PersonId == id && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenter.CostCenterName.ToLower().Contains(term.ToLower()))
                    && p.SiteId == SiteId && p.DivisionId == DivisionId && p.CostCenter.IsActive==true
                    group p by p.CostCenterId into g
                    orderby g.Max(m => m.CostCenter.CostCenterName)
                    select new ComboBoxResult
                    {
                        text = g.Max(m => m.CostCenter.CostCenterName),
                        id = g.Key.Value.ToString(),
                    });

        }

        public IQueryable<ComboBoxResult> GetCostCentersForLine(int id, string term)
        {
            var StockHeader = new StockHeaderService(_unitOfWork).Find(id);

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            return (from p in db.CostCenter
                    where p.LedgerAccount.PersonId == StockHeader.PersonId && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
                    && p.ProcessId == StockHeader.ProcessId
                    && p.SiteId == SiteId && p.DivisionId == DivisionId && p.IsActive == true
                    group p by p.CostCenterId into g
                    orderby g.Max(m => m.CostCenterName)
                    select new ComboBoxResult
                    {
                        text = g.Max(m => m.CostCenterName),
                        id = g.Key.ToString(),
                    });
        }

        public IEnumerable<StockExchangeLineViewModel> GetProcessTransfersForFilters(FiltersForProcessTransfer vm)
        {

            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            //string[] ContraSites = null;
            //if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            //else { ContraSites = new string[] { "NA" }; }

            //string[] ContraDivisions = null;
            //if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            //else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dimension1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dimension1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dimension1IdArr = new string[] { "NA" }; }

            string[] Dimension2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dimension2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dimension2IdArr = new string[] { "NA" }; }

            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", vm.ProcessId);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", vm.PersonId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", vm.FromCostCenterId);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<StockLineViewModel> StockProcessBalance = db.Database.SqlQuery<StockLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate,@SiteId,@DivisionId,@PersonId,@ProcessId,@CostCenterId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId, SqlParameterCostCenterId).ToList();

            var temp = from H in StockProcessBalance
                       join Proc in db.Process on H.ProcessId equals Proc.ProcessId into table
                       from tab in table.DefaultIfEmpty()
                       where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(H.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(H.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dimension1IdArr.Contains(H.Dimension1Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dimension2IdArr.Contains(H.Dimension2Id.ToString()))
                        && H.BalanceQty != 0
                       select new StockExchangeLineViewModel
                       {
                           StockHeaderId = vm.StockHeaderId,
                           Dimension1Name = H.Dimension1Name,
                           Dimension1Id = H.Dimension1Id,
                           Dimension2Id = H.Dimension2Id,
                           Dimension2Name = H.Dimension2Name,
                           Specification = H.Specification,
                           BalanceQty = H.BalanceQty,
                           Qty = H.BalanceQty,
                           UnitName = H.UnitName,
                           UnitDecimalPlaces = H.UnitDecimalPlaces,
                           //UnitId=P.UnitId,
                           ProductName = H.ProductName,
                           ProductId = H.ProductId,
                           ProcessId = (tab == null ? null : (int?)tab.ProcessId),
                           ProcessName = (tab == null ? null : tab.ProcessName),
                           CostCenterId = H.CostCenterId,
                           ToCostCenterId = vm.ToCostCenterId,
                           CostCenterName = H.CostCenterName,
                           LotNo = H.LotNo
                       };

            return temp;


        }

        public IQueryable<ComboBoxResult> GetCostCentersForProcessTransfer(int SiteId, int DivisionId, string FilterDocTypes, string term, int? PersonId)
        {

            //var Header = new StockHeaderService(_unitOfWork).Find(id);

            //var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Header.DocTypeId, Header.DivisionId, Header.SiteId);


            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(FilterDocTypes)) { ContraDocTypes = FilterDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            var list = (from p in db.CostCenter
                        join t in db.LedgerAccount on p.LedgerAccountId equals t.LedgerAccountId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
                          && (string.IsNullOrEmpty(FilterDocTypes) ? 1 == 1 : ContraDocTypes.Contains(p.DocTypeId.ToString()))
                          && p.SiteId == SiteId && p.DivisionId == DivisionId && t.PersonId == PersonId
                        orderby p.CostCenterName
                        select new ComboBoxResult
                        {
                            id = p.CostCenterId.ToString(),
                            text = p.CostCenterName,
                        }
                          );

            return list;
        }

        public IQueryable<ComboBoxResult> GetCustomProducts(int Id, string term)//DocTypeId
        {

            var StockHeader = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] Products = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
            else { Products = new string[] { "NA" }; }

            string[] ProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroups = new string[] { "NA" }; }

            return (from p in db.Product
                    where (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.ProductGroup.ProductTypeId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : Products.Contains(p.ProductId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : ProductGroups.Contains(p.ProductGroupId.ToString()))
                    && p.ProductName.ToLower().Contains(term.ToLower())
                    group new { p } by p.ProductId into g
                    orderby g.Key
                    select new ComboBoxResult
                    {
                        id = g.Key.ToString(),
                        text = g.Max(m => m.p.ProductName),
                    }
                        );
        }

        public IQueryable<ComboBoxResult> GetRequisitionsForReceive(int Id, string term)//DocTypeId
        {

            var StockHeader = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] Products = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
            else { Products = new string[] { "NA" }; }

            string[] ProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroups = new string[] { "NA" }; }

            return (from p in db.RequisitionLineStatus
                    join t in db.RequisitionLine on p.RequisitionLineId equals t.RequisitionLineId
                    join t2 in db.RequisitionHeader on t.RequisitionHeaderId equals t2.RequisitionHeaderId
                    where (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(t.Product.ProductGroup.ProductTypeId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : Products.Contains(t.ProductId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? t2.SiteId == SiteId : ContraSites.Contains(t2.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? t2.DivisionId == DivisionId : ContraDivisions.Contains(t2.DivisionId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : ProductGroups.Contains(t.Product.ProductGroupId.ToString()))
                    && t2.DocNo.ToLower().Contains(term.ToLower()) && t2.PersonId == StockHeader.PersonId
                    && (p.IssueQty ?? 0 - p.ReceiveQty ?? 0) > 0
                    group new { t2 } by t2.RequisitionHeaderId into g
                    orderby g.Key
                    select new ComboBoxResult
                    {
                        id = g.Key.ToString(),
                        text = g.Max(m => m.t2.DocNo),
                    }
                        );
        }

        public int GetMaxSr(int id)
        {
            var Max = (from p in db.StockLine
                       where p.StockHeaderId == id
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
    }
}
