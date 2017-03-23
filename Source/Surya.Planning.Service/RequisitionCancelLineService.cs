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
using System.Data.SqlClient;

namespace Surya.India.Service
{
    public interface IRequisitionCancelLineService : IDisposable
    {
        RequisitionCancelLine Create(RequisitionCancelLine pt);
        void Delete(int id);
        void Delete(RequisitionCancelLine pt);
        RequisitionCancelLine Find(int id);
        void Update(RequisitionCancelLine pt);
        RequisitionCancelLine Add(RequisitionCancelLine pt);
        IEnumerable<RequisitionCancelLineViewModel> GetRequisitionCancelLineForHeader(int id);//Header Id
        Task<IEquatable<RequisitionCancelLine>> GetAsync();
        Task<RequisitionCancelLine> FindAsync(int id);
        RequisitionCancelLineViewModel GetRequisitionCancelLine(int id);//Line Id
        int NextId(int id);
        int PrevId(int id);
        IEnumerable<RequisitionCancelLineViewModel> GetRequisitionLineForOrders(RequisitionCancelFilterViewModel svm);
        RequisitionCancelLineViewModel GetLineDetail(int id);
        IEnumerable<RequisitionCancelProductHelpList> GetPendingProductsForOrder(int id, string term, int Limit);
        IEnumerable<ComboBoxList> GetPendingProductsForFilters(int id, string term, int Limit);
        IEnumerable<ComboBoxList> GetPendingRequisitionsForFilters(int id, string term, int Limit);
    }

    public class RequisitionCancelLineService : IRequisitionCancelLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<RequisitionCancelLine> _RequisitionCancelLineRepository;
        RepositoryQuery<RequisitionCancelLine> RequisitionCancelLineRepository;
        public RequisitionCancelLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _RequisitionCancelLineRepository = new Repository<RequisitionCancelLine>(db);
            RequisitionCancelLineRepository = new RepositoryQuery<RequisitionCancelLine>(_RequisitionCancelLineRepository);
        }


        public RequisitionCancelLine Find(int id)
        {
            return _unitOfWork.Repository<RequisitionCancelLine>().Find(id);
        }

        public RequisitionCancelLine Create(RequisitionCancelLine pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<RequisitionCancelLine>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<RequisitionCancelLine>().Delete(id);
        }

        public void Delete(RequisitionCancelLine pt)
        {
            _unitOfWork.Repository<RequisitionCancelLine>().Delete(pt);
        }

        public void Update(RequisitionCancelLine pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<RequisitionCancelLine>().Update(pt);
        }

        public IEnumerable<RequisitionCancelLineViewModel> GetRequisitionCancelLineForHeader(int id)
        {
            return (from p in db.RequisitionCancelLine
                    join t in db.RequisitionLine on p.RequisitionLineId equals t.RequisitionLineId into table
                    from tab in table.DefaultIfEmpty()
                    join t5 in db.RequisitionHeader on tab.RequisitionHeaderId equals t5.RequisitionHeaderId
                    join t1 in db.Dimension1 on tab.Dimension1Id equals t1.Dimension1Id into table1
                    from tab1 in table1.DefaultIfEmpty()
                    join t2 in db.Dimension2 on tab.Dimension2Id equals t2.Dimension2Id into table2
                    from tab2 in table2.DefaultIfEmpty()
                    join t3 in db.Product on tab.ProductId equals t3.ProductId into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t4 in db.RequisitionCancelHeader on p.RequisitionCancelHeaderId equals t4.RequisitionCancelHeaderId 
                    join t6 in db.Persons on t4.PersonId equals t6.PersonID into table6
                    from tab6 in table6.DefaultIfEmpty()
                    orderby p.RequisitionCancelLineId
                    where p.RequisitionCancelHeaderId == id
                    select new RequisitionCancelLineViewModel
                    {
                        Dimension1Name = tab1.Dimension1Name,
                        Dimension2Name = tab2.Dimension2Name,
                        ProductId = tab.ProductId,
                        ProductName = tab3.ProductName,
                        RequisitionCancelHeaderDocNo = t4.DocNo,
                        RequisitionCancelHeaderId = p.RequisitionCancelHeaderId,
                        RequisitionCancelLineId = p.RequisitionCancelLineId,
                        RequisitionDocNo = t5.DocNo,
                        RequisitionLineId = tab.RequisitionLineId,
                        Qty = p.Qty,
                        Remark = p.Remark,
                        Specification = tab.Specification,                         
                        UnitId = tab3.UnitId,
                        unitDecimalPlaces=tab3.Unit.DecimalPlaces,
                    }
                        );

        }

      
        public RequisitionCancelLineViewModel GetRequisitionCancelLine(int id)
        {
            var temp= (from p in db.RequisitionCancelLine
                 join t in db.RequisitionLine on p.RequisitionLineId equals t.RequisitionLineId into table
                 from tab in table.DefaultIfEmpty()
                 join t5 in db.RequisitionHeader on tab.RequisitionHeaderId equals t5.RequisitionHeaderId into table5 from tab5 in table5.DefaultIfEmpty()
                 join t1 in db.Dimension1 on tab.Dimension1Id equals t1.Dimension1Id into table1
                 from tab1 in table1.DefaultIfEmpty()
                 join t2 in db.Dimension2 on tab.Dimension2Id equals t2.Dimension2Id into table2
                 from tab2 in table2.DefaultIfEmpty()
                 join t3 in db.Product on tab.ProductId equals t3.ProductId into table3
                 from tab3 in table3.DefaultIfEmpty()
                 join t4 in db.RequisitionCancelHeader on p.RequisitionCancelHeaderId equals t4.RequisitionCancelHeaderId into table4 from tab4 in table4.DefaultIfEmpty()
                 join t6 in db.Persons on tab4.PersonId equals t6.PersonID into table6
                 from tab6 in table6.DefaultIfEmpty()        
                 join t7 in db.ViewRequisitionBalance on p.RequisitionLineId equals t7.RequisitionLineId into table7 from tab7 in table7.DefaultIfEmpty()
                 orderby p.RequisitionCancelLineId 
                 where p.RequisitionCancelLineId == id
                 select new RequisitionCancelLineViewModel
                 {
                     Dimension1Name = tab1.Dimension1Name,
                     Dimension2Name = tab2.Dimension2Name,
                     ProductId = tab.ProductId,
                     ProductName = tab3.ProductName,
                     RequisitionCancelHeaderDocNo = tab4.DocNo,
                     RequisitionCancelHeaderId = p.RequisitionCancelHeaderId,
                     RequisitionCancelLineId = p.RequisitionCancelLineId,
                     RequisitionDocNo = tab5.DocNo,
                     RequisitionLineId = tab.RequisitionLineId,
                     BalanceQty=p.Qty+tab7.BalanceQty,
                     Qty = p.Qty,
                     Remark = p.Remark,
                     Specification = tab.Specification,
                     UnitId = tab3.UnitId,
                     UnitName=tab3.Unit.UnitName,
                 }

                      ).FirstOrDefault();

            return temp;
               
        }

        public RequisitionCancelLine Add(RequisitionCancelLine pt)
        {
            _unitOfWork.Repository<RequisitionCancelLine>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.RequisitionCancelLine
                        orderby p.RequisitionCancelLineId
                        select p.RequisitionCancelLineId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.RequisitionCancelLine
                        orderby p.RequisitionCancelLineId
                        select p.RequisitionCancelLineId).FirstOrDefault();
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

                temp = (from p in db.RequisitionCancelLine
                        orderby p.RequisitionCancelLineId
                        select p.RequisitionCancelLineId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.RequisitionCancelLine
                        orderby p.RequisitionCancelLineId
                        select p.RequisitionCancelLineId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public IEnumerable<RequisitionCancelLineViewModel> GetRequisitionLineForOrders(RequisitionCancelFilterViewModel svm)
        {

           
            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(svm.ProductId)) { ProductIdArr = svm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(svm.RequisitionId)) { SaleOrderIdArr = svm.RequisitionId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(svm.ProductGroupId)) { ProductGroupIdArr = svm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dimension1IdArr = null;
            if (!string.IsNullOrEmpty(svm.Dimension1Id)) { Dimension1IdArr = svm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dimension1IdArr = new string[] { "NA" }; }

            string[] Dimension2IdArr = null;
            if (!string.IsNullOrEmpty(svm.Dimension2Id)) { Dimension2IdArr = svm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dimension2IdArr = new string[] { "NA" }; }
           

            var temp = (from p in db.ViewRequisitionBalance
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join t in db.RequisitionLine on p.RequisitionLineId equals t.RequisitionLineId into table1
                        from tab1 in table1.DefaultIfEmpty()
                        join t2 in db.RequisitionHeader on tab1.RequisitionHeaderId equals t2.RequisitionHeaderId
                        where (string.IsNullOrEmpty(svm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                            && (svm.PersonId == 0 ? 1 == 1 : p.PersonId == svm.PersonId)
                        && (string.IsNullOrEmpty(svm.RequisitionId) ? 1 == 1 : SaleOrderIdArr.Contains(p.RequisitionHeaderId.ToString()))
                        && (string.IsNullOrEmpty(svm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(svm.Dimension1Id) ? 1 == 1 : Dimension1IdArr.Contains(tab1.Dimension1Id.ToString()))
                        && (string.IsNullOrEmpty(svm.Dimension2Id) ? 1 == 1 : Dimension2IdArr.Contains(tab1.Dimension2Id.ToString()))
                        && p.BalanceQty > 0 && p.PersonId == svm.PersonId
                        orderby t2.DocDate, t2.DocNo
                        select new RequisitionCancelLineViewModel
                        {
                            BalanceQty = p.BalanceQty,
                            Qty = 0,
                            RequisitionDocNo = p.RequisitionNo,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            RequisitionCancelHeaderId = svm.RequisitionCancelHeaderId,
                            RequisitionLineId = p.RequisitionLineId,
                            unitDecimalPlaces = tab2.Unit.DecimalPlaces,    
                            UnitId=tab2.UnitId,
                            UnitName=tab2.Unit.UnitName,
                        }
                        );
            return temp;
        }


        public RequisitionCancelLineViewModel GetLineDetail(int id)
        {
            return (from p in db.ViewRequisitionBalance
                    join t1 in db.RequisitionLine on p.RequisitionLineId equals t1.RequisitionLineId
                    join t2 in db.Product on p.ProductId equals t2.ProductId
                    join t3 in db.Dimension1 on t1.Dimension1Id equals t3.Dimension1Id into table3
                    from tab3 in table3.DefaultIfEmpty()
                    join t4 in db.Dimension2 on t1.Dimension2Id equals t4.Dimension2Id into table4
                    from tab4 in table4.DefaultIfEmpty()
                    where p.RequisitionLineId == id
                    select new RequisitionCancelLineViewModel
                    {
                        Dimension1Name = tab3.Dimension1Name,
                        Dimension2Name = tab4.Dimension2Name,
                        Qty = p.BalanceQty,
                        BalanceQty=p.BalanceQty,
                        Specification = t1.Specification,
                        UnitId = t2.UnitId,
                        UnitName = t2.Unit.UnitName,
                        ProductId = p.ProductId,
                        ProductName = t1.Product.ProductName,
                         unitDecimalPlaces= t2.Unit.DecimalPlaces,
                         RequisitionDocNo=p.RequisitionNo,
                    }
                        ).FirstOrDefault();

        }


 

        public IEnumerable<RequisitionCancelProductHelpList> GetPendingProductsForOrder(int id,string term,int Limit)
        {

            var RequisitionCancel = new RequisitionCancelHeaderService(_unitOfWork).Find(id);

            var settings = new RequisitionSettingService(_unitOfWork).GetRequisitionSettingForDocument(RequisitionCancel.DocTypeId, RequisitionCancel.DivisionId, RequisitionCancel.SiteId);


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
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == RequisitionCancel.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == RequisitionCancel.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        orderby t.DocDate, t.DocNo
                        select new RequisitionCancelProductHelpList
                        {
                            ProductName = p.Product.ProductName,
                            ProductId = p.ProductId,
                            Specification = t2.Specification,
                            Dimension1Name = p.Dimension1.Dimension1Name,
                            Dimension2Name = p.Dimension2.Dimension2Name,
                            RequisitionDocNo = p.RequisitionNo,
                            RequisitionLineId = p.RequisitionLineId,
                            BalanceQty = p.BalanceQty,
                        }
                          ).Take(Limit);

            return list.ToList();

        }

        public IEnumerable<ComboBoxList> GetPendingProductsForFilters(int id, string term, int Limit)
        {

            var RequisitionCancel = new RequisitionCancelHeaderService(_unitOfWork).Find(id);

            var settings = new RequisitionSettingService(_unitOfWork).GetRequisitionSettingForDocument(RequisitionCancel.DocTypeId, RequisitionCancel.DivisionId, RequisitionCancel.SiteId);


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
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == RequisitionCancel.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == RequisitionCancel.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        group p by p.ProductId into g
                        orderby g.Max(m=>m.RequisitionNo)
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m=>m.Product.ProductName),
                            Id = g.Key,
                        }
                          ).Take(Limit);

            return list.ToList();

        }

        public IEnumerable<ComboBoxList> GetPendingRequisitionsForFilters(int id, string term, int Limit)
        {

            var RequisitionCancel = new RequisitionCancelHeaderService(_unitOfWork).Find(id);

            var settings = new RequisitionSettingService(_unitOfWork).GetRequisitionSettingForDocument(RequisitionCancel.DocTypeId, RequisitionCancel.DivisionId, RequisitionCancel.SiteId);         

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.RequisitionNo.ToLower().Contains(term.ToLower()))                        
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == RequisitionCancel.SiteId : ContraSites.Contains(p.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == RequisitionCancel.DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
                        group p by p.RequisitionHeaderId into g
                        orderby g.Max(m => m.RequisitionNo)
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.RequisitionNo),
                            Id = g.Key,
                        }
                          ).Take(Limit);

            return list.ToList();

        }




        public void Dispose()
        {
        }


        public Task<IEquatable<RequisitionCancelLine>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RequisitionCancelLine> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
