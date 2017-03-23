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

namespace Surya.India.Service
{
    public interface ISaleOrderCancelLineService : IDisposable
    {
        SaleOrderCancelLine Create(SaleOrderCancelLine p);
        void Delete(int id);
        void Delete(SaleOrderCancelLine p);
        SaleOrderCancelLineViewModel GetSaleOrderCancelLine(int p);
        IEnumerable<SaleOrderCancelLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(SaleOrderCancelLine p);
        SaleOrderCancelLine Add(SaleOrderCancelLine p);
        IQueryable<SaleOrderCancelLineViewModel> GetSaleOrderCancelLineListForHeader(int SaleOrderCancelHeaderId);
        Task<IEquatable<SaleOrderCancelLine>> GetAsync();
        Task<SaleOrderCancelLine> FindAsync(int id);
        SaleOrderCancelLine Find(int id);
        decimal GetSaleOrderBalanceQty(string SaleOrderNo, string Product);
        int GetSaleOrderLineIdForProductandSaleOrderDocNo(string SaleOrderNo, string Product);
        IEnumerable<SaleOrderCancelLineViewModel> GetSaleOrderLineForMultiSelect(SaleOrderCancelFilterViewModel svm);
        decimal GetBalanceQuantity(int SaleOrderLineId);
        IQueryable<ComboBoxResult> GetPendingSaleOrderHelpList(int Id, string term);
    }

    public class SaleOrderCancelLineService : ISaleOrderCancelLineService
    {
        ApplicationDbContext db =new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<SaleOrderCancelLine> _SaleOrderCancelLineRepository;
        RepositoryQuery<SaleOrderCancelLine> SaleOrderCancelLineRepository;
        public SaleOrderCancelLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _SaleOrderCancelLineRepository = new Repository<SaleOrderCancelLine>(db);
            SaleOrderCancelLineRepository = new RepositoryQuery<SaleOrderCancelLine>(_SaleOrderCancelLineRepository);
        }

        public decimal GetSaleOrderBalanceQty(string SaleOrderNo,string Product)
        {
            //return (
            //        from p in db.SaleOrderLine
            //        join bal in db.ViewSaleOrderBalance on p.SaleOrderLineId equals bal.SaleOrderLineId
            //        where p.SaleOrderHeader.DocNo == SaleOrderNo & p.Product.ProductName == Product
            //        select bal.BalanceQty
            //    ).FirstOrDefault();

            var SaleOrderLine1 = from L in db.SaleOrderLine
                                 join H in db.SaleOrderHeader on L.SaleOrderHeaderId equals H.SaleOrderHeaderId into SaleOrderHeaderTable
                                 from SaleOrderHeaderTab in SaleOrderHeaderTable.DefaultIfEmpty()
                                 join P in db.Product on L.ProductId equals P.ProductId into ProductTable
                                 from ProductTab in ProductTable.DefaultIfEmpty()
                                 where SaleOrderHeaderTab.DocNo == SaleOrderNo && ProductTab.ProductName == Product
                                 select new
                                 {
                                     SaleOrderLineId = L.SaleOrderLineId
                                 };

            var SaleOrderLine = (from L in db.SaleOrderLine
                                join H in db.SaleOrderHeader on L.SaleOrderHeaderId equals H.SaleOrderHeaderId into SaleOrderHeaderTable
                                from SaleOrderHeaderTab in SaleOrderHeaderTable.DefaultIfEmpty()
                                join P in db.Product on L.ProductId equals P.ProductId into ProductTable
                                from ProductTab in ProductTable.DefaultIfEmpty()
                                where SaleOrderHeaderTab.DocNo == SaleOrderNo && ProductTab.ProductName == Product
                                select new
                                {
                                    SaleOrderLineId = L.SaleOrderLineId
                                }).FirstOrDefault();


            

            if (SaleOrderLine != null)
            {
                try
                {
                int  SaleOrderLineId = SaleOrderLine.SaleOrderLineId;

                var BalanceList = from L in db.ViewSaleOrderBalanceForCancellation
                                  join P in db.Product on L.ProductId equals P.ProductId
                                  where L.SaleOrderLineId == SaleOrderLineId
                                  select new
                                  {
                                      BalanceQty = L.BalanceQty
                                  };

                if (BalanceList != null)
                {
                    return BalanceList.FirstOrDefault().BalanceQty;
                }
                else
                {
                    return 0;
                }
                }
                    catch(Exception e)
                {
                    string str = e.Message;
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
        public int GetSaleOrderLineIdForProductandSaleOrderDocNo(string SaleOrderNo,string Product)
        {
            return (from p in db.SaleOrderLine
                    where p.SaleOrderHeader.DocNo == SaleOrderNo & p.Product.ProductName == Product
                    select p.SaleOrderLineId

                ).FirstOrDefault();
        }
        public SaleOrderCancelLineViewModel GetSaleOrderCancelLine(int pId)
        {
            //return SaleOrderCancelLineRepository
            //    .Get().Where(i => i.SaleOrderCancelLineId == pId).FirstOrDefault();

            return (from p in db.SaleOrderCancelLine
                    join t in db.Persons on p.SaleOrderLine.SaleOrderHeader.SaleToBuyerId equals t.PersonID into table from tab in table.DefaultIfEmpty()
                    join temp1 in db.SaleOrderCancelHeader on p.SaleOrderCancelHeaderId equals temp1.SaleOrderCancelHeaderId into table1 from tab1 in table1.DefaultIfEmpty()
                    where p.SaleOrderCancelLineId == pId
                    select new SaleOrderCancelLineViewModel
                    {
                        LineRemark=p.Remark,
                        BuyerName=tab.Name,
                        CancelNo=tab1.DocNo,
                        DocDate=tab1.DocDate,
                        DocNo=p.SaleOrderLine.SaleOrderHeader.DocNo,
                        DocumentTypeName=tab1.DocType.DocumentTypeName,
                        OrderQty=p.SaleOrderLine.Qty,
                        Reason=tab1.Reason.ReasonName,
                        Remark=tab1.Remark,
                        ProductId=p.SaleOrderLine.ProductId,
                        ProductName=p.SaleOrderLine.Product.ProductName,
                        Qty=p.Qty,
                        SaleOrderCancelLineId=p.SaleOrderCancelLineId,
                        SaleOrderLineId=p.SaleOrderLineId,
                        SaleOrderCancelHeaderId=p.SaleOrderCancelHeaderId,
                        CreatedBy=p.CreatedBy,
                        CreatedDate=p.CreatedDate,
                        ModifiedBy=p.ModifiedBy,
                        ModifiedDate=p.ModifiedDate
                    }).FirstOrDefault();

        }

        public SaleOrderCancelLine Create(SaleOrderCancelLine p)
        {
            p.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleOrderCancelLine>().Insert(p);
            return p;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleOrderCancelLine>().Delete(id);
        }

        public void Delete(SaleOrderCancelLine p)
        {
            _unitOfWork.Repository<SaleOrderCancelLine>().Delete(p);
        }

        public void Update(SaleOrderCancelLine p)
        {
            p.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleOrderCancelLine>().Update(p);
        }

        public IEnumerable<SaleOrderCancelLine> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<SaleOrderCancelLine>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.SaleOrderCancelLineId))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public SaleOrderCancelLine Find(int id)
        {
            return _unitOfWork.Repository<SaleOrderCancelLine>().Find(id);
        }


        public IQueryable<SaleOrderCancelLineViewModel> GetSaleOrderCancelLineListForHeader(int SaleOrderCancelHeaderId)
        {
            //TEsting Comments:-
            //var p = _unitOfWork.Repository<SaleOrderCancelLine>().Query()
                            
            //                .Include(i => i.SaleOrderLine)
            //                .Include(i => i.SaleOrderLine.Product)
            //                .Include(i => i.SaleOrderLine.SaleOrderHeader)                            
            //                .Get().Where(m=>m.SaleOrderCancelHeaderId==SaleOrderCancelHeaderId);

            //return p;
            return (from p in db.SaleOrderCancelLine
                    join t in db.Persons on p.SaleOrderLine.SaleOrderHeader.SaleToBuyerId equals t.PersonID into table from tab in table.DefaultIfEmpty()
                    where p.SaleOrderCancelHeaderId == SaleOrderCancelHeaderId
                    orderby p.SaleOrderCancelLineId
                    select new SaleOrderCancelLineViewModel
                    {
                        LineRemark=p.Remark,
                        DocNo = p.SaleOrderLine.SaleOrderHeader.DocNo,
                        OrderQty = p.SaleOrderLine.Qty,
                        BuyerName=tab.Name,
                        ProductId =p.SaleOrderLine.Product.ProductId,
                        ProductName = p.SaleOrderLine.Product.ProductName,
                        Qty = p.Qty,
                        SaleOrderCancelLineId = p.SaleOrderCancelLineId,
                        SaleOrderLineId = p.SaleOrderLineId,
                        SaleOrderCancelHeaderId = p.SaleOrderCancelHeaderId,
                        CreatedBy = p.CreatedBy,
                        CreatedDate = p.CreatedDate,
                        ModifiedBy = p.ModifiedBy,
                        ModifiedDate = p.ModifiedDate
                    });

        }

        public SaleOrderCancelLine Add(SaleOrderCancelLine p)
        {
            _unitOfWork.Repository<SaleOrderCancelLine>().Insert(p);
            return p;
        }

        public void Dispose()
        {
        }

        public IEnumerable<SaleOrderCancelLineViewModel> GetSaleOrderLineForMultiSelect(SaleOrderCancelFilterViewModel svm)
        {
            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(svm.ProductId)) { ProductIdArr = svm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }            

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(svm.SaleOrderId)) { SaleOrderIdArr = svm.SaleOrderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(svm.ProductGroupId)) { ProductGroupIdArr = svm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            var temp = (from p in db.ViewSaleOrderBalanceForCancellation
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(svm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                            && (svm.BuyerId== 0? 1== 1 : p.BuyerId==svm.BuyerId )
                        && (string.IsNullOrEmpty(svm.SaleOrderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.SaleOrderHeaderId.ToString()))
                        && (string.IsNullOrEmpty(svm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                        && p.BalanceQty > 0
                        select new SaleOrderCancelLineViewModel
                        {
                            BalanceQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            DocNo = p.SaleOrderNo,
                            ProductName = tab2.ProductName,
                            ProductId=p.ProductId,
                            SaleOrderCancelHeaderId = svm.SaleOrderCancelHeaderId,
                            SaleOrderLineId = p.SaleOrderLineId
                        });
            return temp;
        }

        public decimal GetBalanceQuantity(int SaleOrderLineId)
        {
            return ( (from p in db.ViewSaleOrderBalance
                    where p.SaleOrderLineId == SaleOrderLineId
                    select p.BalanceQty).FirstOrDefault());
        }
        public Task<IEquatable<SaleOrderCancelLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SaleOrderCancelLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ComboBoxResult> GetPendingSaleOrderHelpList(int Id, string term)
        {
            var SaleOrderHeader = new SaleOrderCancelHeaderService(_unitOfWork).Find(Id);
            
            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var list = (from p in db.ViewSaleOrderBalanceForCancellation
                        join t in db.SaleOrderLine on p.SaleOrderLineId equals t.SaleOrderLineId
                        join t2 in db.SaleOrderHeader on p.SaleOrderHeaderId equals t2.SaleOrderHeaderId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.SaleOrderNo.ToLower().Contains(term.ToLower())) && p.BalanceQty > 0
                        && t2.SiteId==CurrentSiteId && t2.DivisionId==CurrentDivisionId && p.BuyerId==SaleOrderHeader.BuyerId
                        group new { p } by p.SaleOrderHeaderId into g
                        orderby g.Max(m => m.p.SaleOrderNo)
                        select new ComboBoxResult
                        {
                            text = g.Max(m => m.p.SaleOrderNo), 
                            id = g.Key.ToString(),
                        }
                          );

            return list;
        }


    }
}
