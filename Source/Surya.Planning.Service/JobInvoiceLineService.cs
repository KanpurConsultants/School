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
using Surya.India.Model.ViewModel;
using Surya.India.Model.ViewModels;

namespace Surya.India.Service
{
    public interface IJobInvoiceLineService : IDisposable
    {
        JobInvoiceLine Create(JobInvoiceLine pt);
        void Delete(int id);
        void Delete(JobInvoiceLine pt);
        JobInvoiceLine Find(int id);
        IEnumerable<JobInvoiceLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(JobInvoiceLine pt);
        JobInvoiceLine Add(JobInvoiceLine pt);
        IEnumerable<JobInvoiceLine> GetJobInvoiceLineList();
        IEnumerable<JobInvoiceLineIndexViewModel> GetLineListForIndex(int HeaderId);
        Task<IEquatable<JobInvoiceLine>> GetAsync();
        Task<JobInvoiceLine> FindAsync(int id);
        JobInvoiceLineViewModel GetJobInvoiceLine(int id);
        JobInvoiceLineViewModel GetJobInvoiceReceiveLine(int id);
        int NextId(int id);
        int PrevId(int id);
        IEnumerable<JobInvoiceLineViewModel> GetJobReceiptForFilters(JobInvoiceLineFilterViewModel vm);
        IEnumerable<JobInvoiceLineViewModel> GetJobOrderForFilters(JobInvoiceLineFilterViewModel vm);
        IEnumerable<JobInvoiceLineViewModel> GetJobOrderForFiltersForInvoiceReceive(JobInvoiceLineFilterViewModel vm);
        JobInvoiceLine FindByJobInvoiceHeader(int id);
        IEnumerable<ComboBoxList> GetPendingProductsForJobInvoice(int Jid, string term);
        IEnumerable<ComboBoxList> GetPendingJobWorkersForJobInvoice(string DocTypes, string term);
        IEnumerable<ComboBoxList> GetPendingJobReceive(int Jid, string term);
        IEnumerable<ComboBoxList> GetPendingJobOrders(int Jid, string term);
        IEnumerable<JobReceiveProductHelpList> GetProductHelpList(int Id, int? JobWorkerId, string term, int Limit);
        IEnumerable<JobReceiveProductHelpList> GetProductHelpListForPendingJobOrders(int Id, int JobWorkerId, string term, int Limit);
        IEnumerable<ComboBoxList> GetPendingProductsForJobInvoice(int id, string term, int Limit);
        IEnumerable<ComboBoxList> GetPendingJobOrdersForInvoice(int id, string term, int Limit);
        int GetMaxSr(int id);
    }

    public class JobInvoiceLineService : IJobInvoiceLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<JobInvoiceLine> _JobInvoiceLineRepository;
        RepositoryQuery<JobInvoiceLine> JobInvoiceLineRepository;
        public JobInvoiceLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _JobInvoiceLineRepository = new Repository<JobInvoiceLine>(db);
            JobInvoiceLineRepository = new RepositoryQuery<JobInvoiceLine>(_JobInvoiceLineRepository);
        }

        public JobInvoiceLine FindByJobInvoiceHeader(int id)
        {
            return (from p in db.JobInvoiceLine
                    where p.JobInvoiceHeaderId == id
                    select p).FirstOrDefault();
        }
        public JobInvoiceLine Find(int id)
        {
            return _unitOfWork.Repository<JobInvoiceLine>().Find(id);
        }

        public JobInvoiceLine Create(JobInvoiceLine pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<JobInvoiceLine>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<JobInvoiceLine>().Delete(id);
        }

        public void Delete(JobInvoiceLine pt)
        {
            _unitOfWork.Repository<JobInvoiceLine>().Delete(pt);
        }
        public IEnumerable<JobInvoiceLineViewModel> GetJobReceiptForFilters(JobInvoiceLineFilterViewModel vm)
        {

            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(vm.JobInvoiceHeaderId);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { ContraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.JobReceiveHeaderId)) { SaleOrderIdArr = vm.JobReceiveHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            var temp = (from p in db.ViewJobReceiveBalanceForInvoice
                        join t in db.JobReceiveHeader on p.JobReceiveHeaderId equals t.JobReceiveHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join t1 in db.JobReceiveLine on p.JobReceiveLineId equals t1.JobReceiveLineId into table1
                        from tab1 in table1.DefaultIfEmpty()
                        join t4 in db.JobOrderLine on tab1.JobOrderLineId equals t4.JobOrderLineId into table4
                        from tab4 in table4.DefaultIfEmpty()
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.JobReceiveHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.JobReceiveHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : ContraDocTypes.Contains(p.DocTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && (vm.AsOnDate.HasValue ? tab.DocDate <= vm.AsOnDate.Value : 1 == 1)
                        && (JobInvoice.JobWorkerId.HasValue ? p.JobWorkerId == JobInvoice.JobWorkerId : 1 == 1)
                        && p.BalanceQty > 0 && tab.ProcessId == settings.ProcessId
                        orderby tab1.JobReceiveHeader.DocDate, tab1.JobReceiveHeader.DocNo, tab1.Sr
                        select new JobInvoiceLineViewModel
                        {
                            Dimension1Name = tab4.Dimension1.Dimension1Name,
                            Dimension2Name = tab4.Dimension2.Dimension2Name,
                            Specification = tab4.Specification,
                            ReceiptBalQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            JobReceiveDocNo = tab.DocNo,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            JobInvoiceHeaderId = vm.JobInvoiceHeaderId,
                            JobReceiveLineId = p.JobReceiveLineId,
                            UnitId = tab2.UnitId,
                            UnitName = tab2.Unit.UnitName,
                            DealUnitId = tab4.DealUnitId,
                            DealUnitName = tab4.DealUnit.UnitName,
                            JobWorkerId = tab.JobWorkerId,
                            DealQty = tab4.UnitConversionMultiplier * p.BalanceQty,
                            Rate = tab4.Rate,
                            UnitConversionMultiplier = tab4.UnitConversionMultiplier,
                            UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                            DealUnitDecimalPlaces = tab4.DealUnit.DecimalPlaces,
                            CostCenterId = tab4.JobOrderHeader.CostCenterId,
                        }

                        );
            return temp;
        }
        public IEnumerable<JobInvoiceLineViewModel> GetJobOrderForFilters(JobInvoiceLineFilterViewModel vm)
        {

            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(vm.JobInvoiceHeaderId);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { ContraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.JobOrderHeaderId)) { SaleOrderIdArr = vm.JobOrderHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            //ToChange View to get Joborders instead of goodsreceipts
            var temp = (from p in db.ViewJobReceiveBalanceForInvoice
                        join t2 in db.JobOrderHeader on p.JobOrderHeaderId equals t2.JobOrderHeaderId into table5
                        from tabl2 in table5.DefaultIfEmpty()
                        join t in db.JobReceiveHeader on p.JobReceiveHeaderId equals t.JobReceiveHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join t1 in db.JobReceiveLine on p.JobReceiveLineId equals t1.JobReceiveLineId into table1
                        from tab1 in table1.DefaultIfEmpty()
                        join t3 in db.JobOrderLine on tab1.JobOrderLineId equals t3.JobOrderLineId into table3
                        from tab3 in table3.DefaultIfEmpty()

                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.JobOrderHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.JobOrderHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : ContraDocTypes.Contains(p.DocTypeId.ToString()))
                        && (vm.AsOnDate.HasValue ? tab.DocDate <= vm.AsOnDate.Value : 1 == 1)
                        && (JobInvoice.JobWorkerId.HasValue ? p.JobWorkerId == JobInvoice.JobWorkerId : 1 == 1)
                        && p.BalanceQty > 0 && tab.ProcessId == settings.ProcessId
                        orderby tabl2.DocDate, tabl2.DocNo, tab3.Sr
                        select new JobInvoiceLineViewModel
                        {
                            Dimension1Name = tab3.Dimension1.Dimension1Name,
                            Dimension2Name = tab3.Dimension2.Dimension2Name,
                            Specification = tab3.Specification,
                            ReceiptBalQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            CostCenterId = tabl2.CostCenterId,
                            JobReceiveDocNo = tab.DocNo,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            JobInvoiceHeaderId = vm.JobInvoiceHeaderId,
                            JobReceiveLineId = p.JobReceiveLineId,
                            UnitId = tab2.UnitId,
                            UnitName = tab2.Unit.UnitName,
                            DealUnitId = tab3.DealUnitId,
                            DealUnitName = tab3.DealUnit.UnitName,
                            JobWorkerId = tab.JobWorkerId,
                            DealQty = p.BalanceQty * tab3.UnitConversionMultiplier,
                            Rate = tab3.Rate,
                            UnitConversionMultiplier = tab3.UnitConversionMultiplier,
                            JobOrderDocNo = tabl2.DocNo,
                            UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                            DealUnitDecimalPlaces = tab3.DealUnit.DecimalPlaces,
                        }
                        );

            return temp;
        }

        public IEnumerable<JobInvoiceLineViewModel> GetJobOrderForFiltersForInvoiceReceive(JobInvoiceLineFilterViewModel vm)
        {

            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(vm.JobInvoiceHeaderId);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { ContraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.JobOrderHeaderId)) { SaleOrderIdArr = vm.JobOrderHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            //ToChange View to get Joborders instead of goodsreceipts
            var temp = (from p in db.ViewJobOrderBalance
                        join t2 in db.JobOrderHeader on p.JobOrderHeaderId equals t2.JobOrderHeaderId into table5
                        from tabl in table5.DefaultIfEmpty()
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join t3 in db.JobOrderLine on p.JobOrderLineId equals t3.JobOrderLineId into table3
                        from tab3 in table3.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.JobOrderHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.JobOrderHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : ContraDocTypes.Contains(tabl.DocTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && p.JobWorkerId == vm.JobWorkerId && tab3.ProductUidHeaderId==null
                        && p.BalanceQty > 0
                        orderby tabl.DocDate, tabl.DocNo, tab3.Sr
                        select new JobInvoiceLineViewModel
                        {
                            Dimension1Name = tab3.Dimension1.Dimension1Name,
                            Dimension2Name = tab3.Dimension2.Dimension2Name,
                            Specification = tab3.Specification,
                            ReceiptBalQty = p.BalanceQty,
                            OrderBalanceQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            JobQty = p.BalanceQty,
                            ReceiveQty = p.BalanceQty,
                            CostCenterId = tabl.CostCenterId,
                            PassQty = p.BalanceQty,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            JobInvoiceHeaderId = vm.JobInvoiceHeaderId,
                            JobOrderLineId = tab3.JobOrderLineId,
                            ProductUidId=tab3.ProductUidId,
                            ProductUidName=tab3.ProductUid.ProductUidName,
                            UnitId = tab2.UnitId,
                            UnitName = tab2.Unit.UnitName,
                            DealUnitId = tab3.DealUnitId,
                            DealUnitName = tab3.DealUnit.UnitName,
                            JobWorkerId = p.JobWorkerId,
                            DealQty = p.BalanceQty * tab3.UnitConversionMultiplier,
                            Rate = tab3.Rate,
                            UnitConversionMultiplier = tab3.UnitConversionMultiplier,
                            JobOrderDocNo = tabl.DocNo,
                            UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                            DealUnitDecimalPlaces = tab3.DealUnit.DecimalPlaces,
                        }
                        );

            return temp;
        }


        public void Update(JobInvoiceLine pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<JobInvoiceLine>().Update(pt);
        }

        public IEnumerable<JobInvoiceLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<JobInvoiceLine>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.JobInvoiceLineId))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }
        public IEnumerable<JobInvoiceLineIndexViewModel> GetLineListForIndex(int HeaderId)
        {
            return (from p in db.JobInvoiceLine
                    join t in db.JobReceiveLine on p.JobReceiveLineId equals t.JobReceiveLineId into table
                    from tab in table.DefaultIfEmpty()
                    join t in db.JobOrderLine on tab.JobOrderLineId equals t.JobOrderLineId into table1
                    from tab1 in table1.DefaultIfEmpty()
                    join t in db.JobOrderHeader on tab1.JobOrderHeaderId equals t.JobOrderHeaderId into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t in db.Product on tab1.ProductId equals t.ProductId into table2
                    from tab2 in table2.DefaultIfEmpty()
                    where p.JobInvoiceHeaderId == HeaderId
                    orderby p.Sr
                    select new JobInvoiceLineIndexViewModel
                    {

                        ProductName = tab2.ProductName,
                        Amount = p.Amount,
                        Rate = p.Rate,
                        Qty = tab.Qty,
                        JobOrderDocNo = tab3.DocNo,
                        JobInvoiceLineId = p.JobInvoiceLineId,
                        UnitId = tab2.UnitId,
                        UnitName = tab2.Unit.UnitName,
                        UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                        ProductUidName = tab.ProductUid.ProductUidName,
                        Specification = tab1.Specification,
                        Dimension1Name = tab1.Dimension1.Dimension1Name,
                        Dimension2Name = tab1.Dimension2.Dimension2Name,
                        LotNo = tab.LotNo,
                        JobReceiveHeaderDocNo = tab.JobReceiveHeader.DocNo,
                        JobOrderHeaderDocNo = tab1.JobOrderHeader.DocNo,
                        DealQty = p.DealQty,
                        DealUnitId = p.DealUnitId,
                        DealUnitName = p.DealUnit.UnitName,
                        DealUnitDecimalPlaces = p.DealUnit.DecimalPlaces,
                        Remark=p.Remark,
                        OrderDocTypeId=tab3.DocTypeId,
                        ReceiptDocTypeId=tab.JobReceiveHeader.DocTypeId,
                        OrderHeaderId=tab3.JobOrderHeaderId,
                        ReceiptHeaderId=tab.JobReceiveHeaderId,
                        OrderLineId=tab1.JobOrderLineId,
                        ReceiptLineId=tab.JobReceiveLineId,
                    }
                        );
        }
        public JobInvoiceLineViewModel GetJobInvoiceLine(int id)
        {
            return (from p in db.JobInvoiceLine
                    join t in db.JobReceiveLine on p.JobReceiveLineId equals t.JobReceiveLineId into table
                    from tab in table.DefaultIfEmpty()
                    join t3 in db.JobOrderLine on tab.JobOrderLineId equals t3.JobOrderLineId into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t2 in db.JobInvoiceHeader on p.JobInvoiceHeaderId equals t2.JobInvoiceHeaderId into table2
                    from tab2 in table2.DefaultIfEmpty()
                    join t in db.JobReceiveHeader on tab.JobReceiveHeaderId equals t.JobReceiveHeaderId into table1
                    from tab1 in table1.DefaultIfEmpty()
                    where p.JobInvoiceLineId == id
                    select new JobInvoiceLineViewModel
                    {
                        Amount = p.Amount,
                        ProductId = tab3.ProductId,
                        ProductName = tab3.Product.ProductName,
                        JobReceiveLineId = p.JobReceiveLineId,
                        JobReceiveDocNo = tab1.DocNo,
                        JobInvoiceHeaderId = p.JobInvoiceHeaderId,
                        JobInvoiceLineId = p.JobInvoiceLineId,
                        Qty = tab.Qty,
                        UnitId = tab3.Product.UnitId,
                        UnitConversionMultiplier = p.UnitConversionMultiplier,
                        Rate = p.Rate,
                        JobWorkerId = p.JobWorkerId,
                        DealUnitId = p.DealUnitId,
                        DealQty = p.DealQty,
                        Dimension1Id = tab3.Dimension1Id,
                        Dimension1Name = tab3.Dimension1.Dimension1Name,
                        Dimension2Id = tab3.Dimension2Id,
                        Dimension2Name = tab3.Dimension2.Dimension2Name,
                        Specification = tab3.Specification,
                        Remark=p.Remark,
                    }
                        ).FirstOrDefault();
        }

        public JobInvoiceLineViewModel GetJobInvoiceReceiveLine(int id)
        {
            return (from p in db.JobInvoiceLine
                    join t in db.JobReceiveLine on p.JobReceiveLineId equals t.JobReceiveLineId into table
                    from tab in table.DefaultIfEmpty()
                    join t in db.ViewJobOrderBalance on tab.JobOrderLineId equals t.JobOrderLineId into Vtable
                    from Vtab in Vtable.DefaultIfEmpty()
                    join t3 in db.JobOrderLine on tab.JobOrderLineId equals t3.JobOrderLineId into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t2 in db.JobInvoiceHeader on p.JobInvoiceHeaderId equals t2.JobInvoiceHeaderId into table2
                    from tab2 in table2.DefaultIfEmpty()
                    join t in db.JobReceiveHeader on tab.JobReceiveHeaderId equals t.JobReceiveHeaderId into table1
                    from tab1 in table1.DefaultIfEmpty()
                    where p.JobInvoiceLineId == id
                    select new JobInvoiceLineViewModel
                    {
                        Amount = p.Amount,
                        ProductId = tab3.ProductId,
                        ProductName = tab3.Product.ProductName,
                        ProductUidId=tab.ProductUidId,
                        ProductUidName=tab.ProductUid.ProductUidName,
                        JobReceiveLineId = p.JobReceiveLineId,
                        JobOrderDocNo = tab3.JobOrderHeader.DocNo,
                        JobOrderLineId = tab3.JobOrderLineId,
                        OrderBalanceQty = Vtab.BalanceQty + tab.Qty + tab.LossQty,
                        UnitDecimalPlaces = tab3.Product.Unit.DecimalPlaces,
                        DealUnitDecimalPlaces = tab3.DealUnit.DecimalPlaces,
                        LossQty = tab.LossQty,
                        LotNo = tab.LotNo,
                        PassQty = tab.PassQty,
                        JobQty = tab.Qty + tab.LossQty,
                        ReceiveQty = tab.Qty,
                        Remark = tab.Remark,
                        JobInvoiceHeaderId = p.JobInvoiceHeaderId,
                        JobInvoiceLineId = p.JobInvoiceLineId,
                        Qty = tab.Qty,
                        UnitId = tab3.Product.UnitId,
                        UnitConversionMultiplier = p.UnitConversionMultiplier,
                        Rate = p.Rate,
                        JobWorkerId = p.JobWorkerId,
                        DealUnitId = p.DealUnitId,
                        DealQty = p.DealQty,
                        Dimension1Id = tab3.Dimension1Id,
                        Dimension1Name = tab3.Dimension1.Dimension1Name,
                        Dimension2Id = tab3.Dimension2Id,
                        Dimension2Name = tab3.Dimension2.Dimension2Name,
                        Specification = tab3.Specification,
                        Weight=tab.Weight,
                        PenaltyAmt=tab.PenaltyAmt,
                        IncentiveAmt=tab.IncentiveAmt ?? 0,

                    }
                        ).FirstOrDefault();
        }

        //public JobInvoiceLineViewModel GetJobInvoiceLineBalance(int id)
        //{
        //    return (from b in db.ViewJobInvoiceBalance
        //            join p in db.JobInvoiceLine on b.JobInvoiceLineId equals p.JobInvoiceLineId 
        //            join t in db.JobGoodsReceiptLine on p.JobGoodsReceiptLineId equals t.JobGoodsReceiptLineId into table
        //            from tab in table.DefaultIfEmpty()
        //            join t2 in db.JobInvoiceHeader on p.JobInvoiceHeaderId equals t2.JobInvoiceHeaderId into table2
        //            from tab2 in table2.DefaultIfEmpty()
        //            join t in db.JobGoodsReceiptHeader on tab.JobGoodsReceiptHeaderId equals t.JobGoodsReceiptHeaderId into table1
        //            from tab1 in table1.DefaultIfEmpty()

        //            where p.JobInvoiceLineId == id
        //            select new JobInvoiceLineViewModel
        //            {

        //                SupplierId = tab2.SupplierId,
        //                Amount = p.Amount,
        //                ProductId = tab.ProductId,
        //                JobGoodsReceiptLineId = p.JobGoodsReceiptLineId,
        //                JobGoodsRecieptHeaderDocNo = tab1.DocNo,
        //                JobInvoiceHeaderId = p.JobInvoiceHeaderId,
        //                JobInvoiceLineId = p.JobInvoiceLineId,
        //                Qty = b.BalanceQty,
        //                Rate = p.Rate,
        //                Remark = p.Remark,
        //                UnitConversionMultiplier = p.UnitConversionMultiplier,
        //                DealUnitId = p.DealUnitId,
        //                DealQty = p.DealQty,
        //                UnitId = tab.Product.UnitId,
        //                Dimension1Id = tab.Dimension1Id,
        //                Dimension1Name = tab.Dimension1.Dimension1Name,
        //                Dimension2Id = tab.Dimension2Id,
        //                Dimension2Name = tab.Dimension2.Dimension2Name,
        //                Specification = tab.Specification,
        //                LotNo = tab.LotNo,

        //            }
        //                ).FirstOrDefault();
        //}

        public IEnumerable<JobInvoiceLine> GetJobInvoiceLineList()
        {
            var pt = _unitOfWork.Repository<JobInvoiceLine>().Query().Get().OrderBy(m => m.JobInvoiceLineId);

            return pt;
        }

        public JobInvoiceLine Add(JobInvoiceLine pt)
        {
            _unitOfWork.Repository<JobInvoiceLine>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.JobInvoiceLine
                        orderby p.JobInvoiceLineId
                        select p.JobInvoiceLineId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.JobInvoiceLine
                        orderby p.JobInvoiceLineId
                        select p.JobInvoiceLineId).FirstOrDefault();
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

                temp = (from p in db.JobInvoiceLine
                        orderby p.JobInvoiceLineId
                        select p.JobInvoiceLineId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.JobInvoiceLine
                        orderby p.JobInvoiceLineId
                        select p.JobInvoiceLineId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public IEnumerable<ComboBoxList> GetPendingProductsForJobInvoice(int Jid, string term)//DocTypeId
        {

            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(Jid);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            return (from p in db.ViewJobReceiveBalanceForInvoice
                    join t in db.Product on p.ProductId equals t.ProductId into ProdTable
                    from ProTab in ProdTable.DefaultIfEmpty()
                    where p.BalanceQty > 0 && ProTab.ProductName.ToLower().Contains(term.ToLower()) && (JobInvoice.JobWorkerId.HasValue ? p.JobWorkerId == JobInvoice.JobWorkerId : 1 == 1)
                     && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                     && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                    group new { p, ProTab } by p.ProductId into g
                    orderby g.Key descending
                    select new ComboBoxList
                    {
                        Id = g.Key,
                        PropFirst = g.Max(m => m.ProTab.ProductName)
                    }
                        ).Take(20);
        }

        public IEnumerable<ComboBoxList> GetPendingJobReceive(int Jid, string term)//DocTypeId
        {

            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(Jid);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            return (from p in db.ViewJobReceiveBalanceForInvoice
                    join t in db.JobReceiveHeader on p.JobReceiveHeaderId equals t.JobReceiveHeaderId into ProdTable
                    from ProTab in ProdTable.DefaultIfEmpty()
                    where p.BalanceQty > 0 && (ProTab.DocNo.ToLower().Contains(term.ToLower()) || ProTab.JobWorkerDocNo.ToLower().Contains(term.ToLower())) && (JobInvoice.JobWorkerId.HasValue ? p.JobWorkerId == JobInvoice.JobWorkerId : 1 == 1)
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                    group new { p, ProTab } by p.JobReceiveHeaderId into g
                    select new ComboBoxList
                    {
                        Id = g.Key,
                        PropFirst = g.Max(m => m.ProTab.DocNo + " | " + m.ProTab.JobWorkerDocNo),
                    }
                        ).Take(20);
        }

        public IEnumerable<ComboBoxList> GetPendingJobOrders(int Jid, string term)//DocTypeId
        {

            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(Jid);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            return (from p in db.ViewJobReceiveBalanceForInvoice
                    join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId into ProdTable
                    from ProTab in ProdTable.DefaultIfEmpty()
                    where p.BalanceQty > 0 && ProTab.DocNo.ToLower().Contains(term.ToLower()) && p.JobWorkerId == JobInvoice.JobWorkerId
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                    group new { p, ProTab } by p.JobOrderHeaderId into g
                    select new ComboBoxList
                    {
                        Id = g.Key,
                        PropFirst = g.Max(m => m.ProTab.DocNo),
                    }
                        ).Take(20);
        }

        public IEnumerable<ComboBoxList> GetPendingJobWorkersForJobInvoice(string DocTypes, string term)//DocTypeId
        {

            return (from p in db.ViewJobReceiveBalanceForInvoice
                    join t in db.JobWorker on p.JobWorkerId equals t.PersonID into table
                    from tab in table.DefaultIfEmpty()
                    where p.BalanceQty > 0 && tab.Person.Name.ToLower().Contains(term.ToLower())
                    group new { p, tab } by p.JobWorkerId into g
                    orderby g.Key descending
                    select new ComboBoxList
                    {
                        Id = g.Key,
                        PropFirst = g.Max(m => m.tab.Person.Name)
                    }
                        );
        }

        public IEnumerable<JobReceiveProductHelpList> GetProductHelpList(int Id, int? JobWorkerId, string term, int Limit)
        {
            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(Id);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var list = (from p in db.ViewJobReceiveBalanceForInvoice
                        join t in db.JobReceiveHeader on p.JobReceiveHeaderId equals t.JobReceiveHeaderId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderLine.Specification.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderNo.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : t.JobWorkerDocNo.ToLower().Contains(term.ToLower())
                        )
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && (JobWorkerId.HasValue && JobWorkerId > 0 ? p.JobWorkerId == JobWorkerId : 1 == 1)
                        orderby t.DocDate, t.DocNo
                        select new JobReceiveProductHelpList
                        {
                            ProductName = p.Product.ProductName,
                            ProductId = p.ProductId,
                            Specification = p.JobOrderLine.Specification,
                            Dimension1Name = p.Dimension1.Dimension1Name,
                            Dimension2Name = p.Dimension2.Dimension2Name,
                            JobOrderNo = p.JobOrderNo,
                            JobReceiveDocNo = t.JobWorkerDocNo,
                            JobReceiveLineId = p.JobReceiveLineId,
                            Qty = p.BalanceQty,
                        }
                          ).Take(Limit);

            return list.ToList();
        }

        public IEnumerable<JobReceiveProductHelpList> GetProductHelpListForPendingJobOrders(int Id, int JobWorkerId, string term, int Limit)
        {
            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(Id);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var list = (from p in db.ViewJobOrderBalance
                        join t2 in db.JobOrderLine on p.JobOrderLineId equals t2.JobOrderLineId
                        join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : t2.Specification.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderNo.ToLower().Contains(term.ToLower())
                        )
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && p.JobWorkerId == JobWorkerId && p.BalanceQty > 0
                        orderby t.DocDate, t.DocNo
                        select new JobReceiveProductHelpList
                        {
                            ProductName = p.Product.ProductName,
                            ProductId = p.ProductId,
                            Specification = t2.Specification,
                            Dimension1Name = p.Dimension1.Dimension1Name,
                            Dimension2Name = p.Dimension2.Dimension2Name,
                            JobOrderNo = p.JobOrderNo,
                            JobOrderLineId = p.JobOrderLineId,
                            Qty = p.BalanceQty,
                        }
                          ).Take(Limit);

            return list.ToList();
        }


        public IEnumerable<ComboBoxList> GetPendingProductsForJobInvoice(int id, string term, int Limit)//DocTypeId
        {

            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(id);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { ContraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            var list = (from p in db.ViewJobOrderBalance
                        join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId
                        join t2 in db.JobOrderLine on p.JobOrderLineId equals t2.JobOrderLineId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower())
                        )
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : ContraDocTypes.Contains(t.DocTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && p.BalanceQty > 0 && t2.ProductUidHeaderId==null && p.JobWorkerId==JobInvoice.JobWorkerId
                        group p by p.ProductId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.Product.ProductName),
                            Id = g.Key,
                        }
                          ).Take(Limit);

            return list.ToList();
        }

        public IEnumerable<ComboBoxList> GetPendingJobOrdersForInvoice(int id, string term, int Limit)//DocTypeId
        {

            var JobInvoice = new JobInvoiceHeaderService(_unitOfWork).Find(id);

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(JobInvoice.DocTypeId, JobInvoice.DivisionId, JobInvoice.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { ContraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            var list = (from p in db.ViewJobOrderBalance
                        join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId
                        join t2 in db.JobOrderLine on p.JobOrderLineId equals t2.JobOrderLineId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderNo.ToLower().Contains(term.ToLower())
                        )
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == JobInvoice.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == JobInvoice.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : ContraDocTypes.Contains(t.DocTypeId.ToString()))
                        && p.BalanceQty > 0 && t2.ProductUidHeaderId==null && p.JobWorkerId==JobInvoice.JobWorkerId
                        orderby t.DocDate, t.DocNo
                        group p by p.JobOrderHeaderId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.JobOrderNo),
                            Id = g.Key,
                        }
                          ).Take(Limit);

            return list.ToList();
        }

        public int GetMaxSr(int id)
        {
            var Max = (from p in db.JobInvoiceLine
                       where p.JobInvoiceHeaderId == id
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

        public JobInvoiceRateAmendmentLineViewModel GetLineDetail(int id)
        {
            return (from p in db.JobInvoiceLine
                    join JR in db.JobReceiveLine on p.JobReceiveLineId equals JR.JobReceiveLineId
                    join JO in db.JobOrderLine on JR.JobOrderLineId equals JO.JobOrderLineId
                    join t2 in db.Product on JO.ProductId equals t2.ProductId
                    join t3 in db.Dimension1 on JO.Dimension1Id equals t3.Dimension1Id into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t4 in db.Dimension2 on JO.Dimension2Id equals t4.Dimension2Id into table4
                    from tab4 in table4.DefaultIfEmpty()
                    join t5 in db.JobWorker on p.JobWorkerId equals t5.PersonID
                    where p.JobInvoiceLineId == id
                    select new JobInvoiceRateAmendmentLineViewModel
                    {
                        Dimension1Name = tab3.Dimension1Name,
                        Dimension2Name = tab4.Dimension2Name,
                        LotNo = JR.LotNo,
                        Qty = p.Qty ?? 0,
                        Specification = JO.Specification,
                        UnitId = t2.UnitId,
                        DealUnitId = p.DealUnitId,
                        DealQty = p.DealQty,
                        UnitConversionMultiplier = p.UnitConversionMultiplier,
                        UnitName = t2.Unit.UnitName,
                        DealUnitName = p.DealUnit.UnitName,
                        ProductId = JO.ProductId,
                        ProductName = t2.ProductName,
                        unitDecimalPlaces = t2.Unit.DecimalPlaces,
                        DealunitDecimalPlaces = p.DealUnit.DecimalPlaces,
                        JobWorkerId = p.JobWorkerId,
                        JobWorkerName = t5.Person.Name,
                        Rate = p.Rate,

                    }
                        ).FirstOrDefault();

        }

        public bool ValidateJobInvoice(int lineid, int headerid)
        {
            var temp = (from p in db.JobInvoiceRateAmendmentLine
                        where p.JobInvoiceLineId == lineid && p.JobInvoiceAmendmentHeaderId == headerid
                        select p).FirstOrDefault();
            if (temp != null)
                return false;
            else
                return true;

        }

        public JobInvoiceLineViewModel GetLineDetailFromUId(string UID)
        {
            return (from p in db.JobInvoiceLine
                    join JR in db.JobReceiveLine on p.JobReceiveLineId equals JR.JobReceiveLineId
                    join JO in db.JobOrderLine on JR.JobOrderLineId equals JO.JobOrderLineId
                    join t in db.ProductUid on JR.ProductUidId equals t.ProductUIDId into uidtable
                    from uidtab in uidtable.DefaultIfEmpty()
                    join t2 in db.Product on JO.ProductId equals t2.ProductId
                    join t3 in db.Dimension1 on JO.Dimension1Id equals t3.Dimension1Id into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t4 in db.Dimension2 on JO.Dimension2Id equals t4.Dimension2Id into table4
                    from tab4 in table4.DefaultIfEmpty()
                    where uidtab.ProductUidName == UID
                    select new JobInvoiceLineViewModel
                    {
                        Dimension1Name = tab3.Dimension1Name,
                        Dimension2Name = tab4.Dimension2Name,
                        LotNo = JR.LotNo,
                        Qty = (p.Qty ?? 0),
                        Specification = JO.Specification,
                        UnitId = t2.UnitId,
                        DealUnitId = p.DealUnitId,
                        DealQty = p.DealQty,
                        UnitConversionMultiplier = p.UnitConversionMultiplier,
                        UnitName = t2.Unit.UnitName,
                        DealUnitName = p.DealUnit.UnitName,
                        Rate = p.Rate,
                        ProductId=JO.ProductId,
                        ProductName=JO.Product.ProductName,
                        InvoiceDocNo=p.JobInvoiceHeader.DocNo,
                    }
                        ).FirstOrDefault();

        }


        public Task<IEquatable<JobInvoiceLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<JobInvoiceLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
