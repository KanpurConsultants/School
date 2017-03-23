﻿using Surya.India.Data.Infrastructure;
using Surya.India.Data.Models;
using Surya.India.Model;
using Surya.India.Model.Models;
using Surya.India.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surya.India.Model.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;
using Surya.India.Model.ViewModel;

namespace Surya.India.Service
{
    public interface ISaleOrderHeaderService : IDisposable
    {
        SaleOrderHeader Create(SaleOrderHeader s);
        void Delete(int id);
        void Delete(SaleOrderHeader s);
        SaleOrderHeader GetSaleOrderHeader(int id);

        SaleOrderHeaderIndexViewModel GetSaleOrderHeaderVM(int id);
        SaleOrderHeader Find(int id);
        IQueryable<SaleOrderHeaderIndexViewModel> GetSaleOrderHeaderList(int id);     
        void Update(SaleOrderHeader s);
        string GetMaxDocNo();
        SaleOrderHeader FindByDocNo(string Docno);
        IEnumerable<SaleOrderPrintViewModel> FGetPrintData(int Id);
        IEnumerable<SaleOrderHeader> GetSaleOrderListForReport(int BuyerId);
        int NextId(int id);
        int PrevId(int id);
        IEnumerable<SaleOrderLineListViewModel> GetSaleOrdersForDocumentType(int HeaderId, string term);//DoctypeIds
    }
    public class SaleOrderHeaderService : ISaleOrderHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public SaleOrderHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

     public SaleOrderHeader Create(SaleOrderHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleOrderHeader>().Insert(s);
            return s;
        }

       public void Delete(int id)
     {
         _unitOfWork.Repository<SaleOrderHeader>().Delete(id);
     }
       public void Delete(SaleOrderHeader s)
        {
            _unitOfWork.Repository<SaleOrderHeader>().Delete(s);
        }
       public void Update(SaleOrderHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleOrderHeader>().Update(s);            
        }

       public SaleOrderHeader GetSaleOrderHeader(int id)
        {
            return _unitOfWork.Repository<SaleOrderHeader>().Query().Get().Where(m => m.SaleOrderHeaderId == id).FirstOrDefault();
        }

       public int NextId(int id)
       {
           int temp = 0;
           if (id != 0)
           {

               temp = (from p in db.SaleOrderHeader
                       join t in db.Persons on p.SaleToBuyerId equals t.PersonID
                       orderby p.DocDate descending, p.DocNo descending                       
                       select p.SaleOrderHeaderId).AsEnumerable().SkipWhile(p=>p!=id).Skip(1).FirstOrDefault();


           }
           else
           {
               temp = (from p in db.SaleOrderHeader
                       join t in db.Persons on p.SaleToBuyerId equals t.PersonID
                       orderby p.DocDate descending, p.DocNo descending                       
                       select p.SaleOrderHeaderId).FirstOrDefault();
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

               temp = (from p in db.SaleOrderHeader
                       join t in db.Persons on p.SaleToBuyerId equals t.PersonID
                       orderby p.DocDate descending, p.DocNo descending                       
                       select p.SaleOrderHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
           }
           else
           {
               temp = (from p in db.SaleOrderHeader
                       join t in db.Persons on p.SaleToBuyerId equals t.PersonID
                       orderby p.DocDate descending, p.DocNo descending                       
                       select p.SaleOrderHeaderId).AsEnumerable().LastOrDefault();
           }
           if (temp != 0)
               return temp;
           else
               return id;
       }

        public SaleOrderHeaderIndexViewModel GetSaleOrderHeaderVM(int id)
       {

           SaleOrderHeaderIndexViewModel temp= (from p in db.SaleOrderHeader
                   join t in db.Persons on p.SaleToBuyerId equals t.PersonID into table from tab in table.DefaultIfEmpty()
                   join t2 in db.Persons on p.BillToBuyerId equals t2.PersonID into table2 from tab2 in table2.DefaultIfEmpty()
                   
                   where p.SaleOrderHeaderId == id
                   select new SaleOrderHeaderIndexViewModel
                   {
                       DocTypeId=p.DocTypeId,
                       BillToBuyerName=tab2.Name,
                       BuyerOrderNo=p.BuyerOrderNo,
                       CreatedBy=p.CreatedBy,
                       CreatedDate=p.CreatedDate,
                       DivisionName=p.Division.DivisionName,
                       DocDate=p.DocDate,
                       DocNo=p.DocNo,
                       DueDate=p.DueDate,
                       ModifiedBy=p.ModifiedBy,
                       ModifiedDate=p.ModifiedDate,
                       Remark=p.Remark,
                       SaleOrderHeaderId=p.SaleOrderHeaderId,
                       SaleToBuyerName=tab.Name,
                       ShipAddress=p.ShipAddress,
                       SiteName=p.Site.SiteName,
                       Status=p.Status,
                       DocumentTypeName=p.DocType.DocumentTypeName,
                       CurrencyName=p.Currency.Name,
                       ShipMethodName=p.ShipMethod.ShipMethodName,
                       DeliveryTermsName=p.DeliveryTerms.DeliveryTermsName,
                       CreditDays=p.CreditDays,
                       TermsAndConditions=p.TermsAndConditions,
                       Priority=p.Priority
                   }

               ).FirstOrDefault();

           temp.PriorityName = Enum.GetName(typeof(SaleOrderPriority), temp.Priority);
           return temp;
       }

       public SaleOrderHeader Find(int id)
        {
            return _unitOfWork.Repository<SaleOrderHeader>().Find(id);
        }



       public IQueryable<SaleOrderHeaderIndexViewModel> GetSaleOrderHeaderList(int id)
        {
            int divisionid = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int siteid = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var temp = from p in db.SaleOrderHeader
                       join t in db.Persons on p.SaleToBuyerId equals t.PersonID
                       orderby p.DocDate descending,p.DocNo descending
                       where p.DivisionId==divisionid && p.SiteId==siteid && p.DocTypeId==id
                       select new SaleOrderHeaderIndexViewModel
                       {
                           Remark=p.Remark,
                           DocDate=p.DocDate,
                           SaleOrderHeaderId=p.SaleOrderHeaderId,
                           DocNo=p.DocNo,
                           DueDate=p.DueDate,
                           SaleToBuyerName=t.Name,
                           Status=p.Status,
                           ModifiedBy=p.ModifiedBy,
                       };
            return temp;                             
        }

       public IEnumerable<SaleOrderLineListViewModel> GetSaleOrders(int ProductId, int BuyerId)//Product Id
       {
           var tem = from p in db.SaleOrderHeader
                     join t in db.SaleOrderLine on p.SaleOrderHeaderId equals t.SaleOrderHeaderId into table
                     from tab in table.DefaultIfEmpty()

                     where tab.ProductId == ProductId && p.SaleToBuyerId == BuyerId
                     orderby p.DocNo
                     select new SaleOrderLineListViewModel
                     {
                         DocNo = p.DocNo,
                         SaleOrderLineId = tab.SaleOrderLineId,
                     };

           return (tem);
       }
   
        public SaleOrderHeader FindByDocNo(string Docno)
       {
         return  _unitOfWork.Repository<SaleOrderHeader>().Query().Get().Where(m => m.DocNo == Docno).FirstOrDefault();

       }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<SaleOrderHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }
        public void Dispose()
        {
        }

        public IEnumerable<SaleOrderPrintViewModel> FGetPrintData(int Id)
        {
            ApplicationDbContext Db = new ApplicationDbContext();
            IEnumerable<SaleOrderPrintViewModel> saleorderprintviewmodel = db.Database.SqlQuery<SaleOrderPrintViewModel>(Db.strSchemaName + ".ProcSaleOrderPrint @Id", new SqlParameter("@Id", Id)).ToList();
            return saleorderprintviewmodel;
        }

        public IEnumerable<SaleOrderHeader> GetSaleOrderListForReport(int BuyerId)
        {
            return _unitOfWork.Repository<SaleOrderHeader>().Query().Include(m => m.DocType).Get().Where(m => m.SaleToBuyerId  == BuyerId);
        }

        public IEnumerable<SaleOrderHeader> GetSaleOrderListFromIds(String  StrSaleOrderIdsList)
        {
            string[] strarr = StrSaleOrderIdsList.Split(',');
            int[] SaleOrderListArr = Array.ConvertAll(strarr, s => int.Parse(s));

            var p = (from H in db.SaleOrderHeader where SaleOrderListArr.Contains(H.SaleOrderHeaderId) select H).ToList();

            return p;


            //return _unitOfWork.Repository<SaleOrderHeader>().Query().Get().Where(SaleOrderListArr.Contains(m => m.SaleOrderHeaderId));
        }


        public IEnumerable<SaleOrderLineListViewModel> GetSaleOrdersForDocumentType(int HeaderId, string term)
        {
            //return (from p in db.SaleOrderHeader
            //        where DocTypeIds.Contains(p.DocTypeId.ToString())
            //        orderby p.DocDate descending, p.DocNo descending
            //        select new SaleOrderLineListViewModel
            //        {
            //            DocNo = p.DocNo,
            //            SaleOrderHeaderId = p.SaleOrderHeaderId
            //        }
            //            );

            var Header = new MaterialPlanHeaderService(_unitOfWork).Find(HeaderId);

            var Settings = new MaterialPlanSettingsService(_unitOfWork).GetMaterialPlanSettingsForDocument(Header.DocTypeId, Header.DivisionId, Header.SiteId);

            SqlParameter SqlParameterDocType = new SqlParameter("@PlanningDocumentType", Header.DocTypeId);
            SqlParameter SqlParameterSite = new SqlParameter("@Site", Header.SiteId);
            SqlParameter SqlParameterDivision = new SqlParameter("@Division", Header.DivisionId);
            SqlParameter SqlParameterBuyer = new SqlParameter("@BuyerId", Header.BuyerId.HasValue ? Header.BuyerId : (object)DBNull.Value);

            string ProcName = Settings.PendingProdOrderList;
            if (string.IsNullOrEmpty(ProcName))
                throw new Exception("Pending ProdOrders not configured");

            IEnumerable<PendingSaleOrderFromProc> CalculationLineList = db.Database.SqlQuery<PendingSaleOrderFromProc>("" + ProcName + " @PlanningDocumentType, @Site, @Division, @BuyerId", SqlParameterDocType, SqlParameterSite, SqlParameterDivision, SqlParameterBuyer).ToList();

            var list = (from p in CalculationLineList
                        where p.SaleOrderNo.ToLower().Contains(term.ToLower())
                        group new {p} by p.SaleOrderHeaderId into g                        
                        select new SaleOrderLineListViewModel
                        {
                            DocNo = g.Max(m=>m.p.SaleOrderNo),
                            SaleOrderHeaderId = g.Key
                        }
                          );

            return list.ToList();
        }
    }
}
