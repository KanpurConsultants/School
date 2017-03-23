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

namespace Surya.India.Service
{
    public interface ISaleInvoiceReturnHeaderService : IDisposable
    {
        SaleInvoiceReturnHeader Create(SaleInvoiceReturnHeader pt);
        void Delete(int id);
        void Delete(SaleInvoiceReturnHeader pt);
        SaleInvoiceReturnHeader Find(string Name);
        SaleInvoiceReturnHeader Find(int id);
        
        IEnumerable<SaleInvoiceReturnHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(SaleInvoiceReturnHeader pt);
        SaleInvoiceReturnHeader Add(SaleInvoiceReturnHeader pt);
        SaleInvoiceReturnHeaderViewModel GetSaleInvoiceReturnHeader(int id);//HeadeRId
        IQueryable<SaleInvoiceReturnHeaderViewModel> GetSaleInvoiceReturnHeaderList(int id, string Uname);
        IQueryable<SaleInvoiceReturnHeaderViewModel> GetSaleInvoiceReturnPendingToSubmit(int id, string Uname);
        IQueryable<SaleInvoiceReturnHeaderViewModel> GetSaleInvoiceReturnPendingToApprove(int id, string Uname);
        Task<IEquatable<SaleInvoiceReturnHeader>> GetAsync();
        Task<SaleInvoiceReturnHeader> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
        string GetMaxDocNo();
    }

    public class SaleInvoiceReturnHeaderService : ISaleInvoiceReturnHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<SaleInvoiceReturnHeader> _SaleInvoiceReturnHeaderRepository;
        RepositoryQuery<SaleInvoiceReturnHeader> SaleInvoiceReturnHeaderRepository;
        public SaleInvoiceReturnHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _SaleInvoiceReturnHeaderRepository = new Repository<SaleInvoiceReturnHeader>(db);
            SaleInvoiceReturnHeaderRepository = new RepositoryQuery<SaleInvoiceReturnHeader>(_SaleInvoiceReturnHeaderRepository);
        }

        public SaleInvoiceReturnHeader Find(string Name)
        {
            return SaleInvoiceReturnHeaderRepository.Get().Where(i => i.DocNo == Name).FirstOrDefault();
        }


        public SaleInvoiceReturnHeader Find(int id)
        {
            return _unitOfWork.Repository<SaleInvoiceReturnHeader>().Find(id);
        }       

        public SaleInvoiceReturnHeader Create(SaleInvoiceReturnHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleInvoiceReturnHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleInvoiceReturnHeader>().Delete(id);
        }

        public void Delete(SaleInvoiceReturnHeader pt)
        {
            _unitOfWork.Repository<SaleInvoiceReturnHeader>().Delete(pt);
        }

        public void Update(SaleInvoiceReturnHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleInvoiceReturnHeader>().Update(pt);
        }

        public IEnumerable<SaleInvoiceReturnHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<SaleInvoiceReturnHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DocNo))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }
        public SaleInvoiceReturnHeaderViewModel GetSaleInvoiceReturnHeader(int id)
        {
            return (from p in db.SaleInvoiceReturnHeader
                    where p.SaleInvoiceReturnHeaderId == id
                    select new SaleInvoiceReturnHeaderViewModel
                    {
                        SaleInvoiceReturnHeaderId = p.SaleInvoiceReturnHeaderId,
                        DivisionId = p.DivisionId,
                        DocNo = p.DocNo,
                        DocDate = p.DocDate,
                        DocTypeId = p.DocTypeId,
                        Remark = p.Remark,
                        SiteId = p.SiteId,
                        SaleDispatchReturnHeaderId=p.SaleDispatchReturnHeaderId,
                        Status = p.Status,
                        BuyerId = p.BuyerId,                        
                        SalesTaxGroupId=p.SalesTaxGroupId,
                        CurrencyId=p.CurrencyId,
                        ReasonId=p.ReasonId,
                        
                    }

                        ).FirstOrDefault();
        }
        public IQueryable<SaleInvoiceReturnHeaderViewModel> GetSaleInvoiceReturnHeaderList(int id, string Uname)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"]; 

            var pt = (from p in db.SaleInvoiceReturnHeader
                      join t3 in db._Users on p.ModifiedBy equals t3.UserName into table3
                      from tab3 in table3.DefaultIfEmpty()
                      join t in db.DocumentType on p.DocTypeId equals t.DocumentTypeId into table
                      from tab in table.DefaultIfEmpty()
                      join t1 in db.Persons on p.BuyerId equals t1.PersonID into table2
                      from tab2 in table2.DefaultIfEmpty()
                      orderby p.DocDate descending, p.DocNo descending
                      where p.SiteId == SiteId && p.DivisionId == DivisionId && p.DocTypeId == id && (p.Status == 0 ? (p.ModifiedBy == Uname || UserRoles.Contains("Admin")) : 1 == 1)
                      select new SaleInvoiceReturnHeaderViewModel
                      {
                          DocDate = p.DocDate,
                          DocNo = p.DocNo,
                          DocTypeName = tab.DocumentTypeName,
                          SaleInvoiceReturnHeaderId = p.SaleInvoiceReturnHeaderId,
                          Remark = p.Remark,
                          Status = p.Status,
                          BuyerName = tab2.Name,
                          ModifiedBy = p.ModifiedBy,
                          FirstName = tab3.FirstName,
                      }
                         );
            return pt;
        }

        public SaleInvoiceReturnHeader Add(SaleInvoiceReturnHeader pt)
        {
            _unitOfWork.Repository<SaleInvoiceReturnHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.SaleInvoiceReturnHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.SaleInvoiceReturnHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.SaleInvoiceReturnHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.SaleInvoiceReturnHeaderId).FirstOrDefault();
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

                temp = (from p in db.SaleInvoiceReturnHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.SaleInvoiceReturnHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.SaleInvoiceReturnHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.SaleInvoiceReturnHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<SaleInvoiceReturnHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public IQueryable<SaleInvoiceReturnHeaderViewModel> GetSaleInvoiceReturnPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var SaleInvoiceReturnHeader = GetSaleInvoiceReturnHeaderList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in SaleInvoiceReturnHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }

        public IQueryable<SaleInvoiceReturnHeaderViewModel> GetSaleInvoiceReturnPendingToApprove(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var SaleInvoiceReturnHeader = GetSaleInvoiceReturnHeaderList(id, Uname).AsQueryable();

            var PendingToApprove = from p in SaleInvoiceReturnHeader
                                   where p.Status == (int)StatusConstants.Submitted || p.Status == (int)StatusConstants.ModificationSubmitted
                                   select p;
            return PendingToApprove;

        }

        public void Dispose()
        {
        }


        public Task<IEquatable<SaleInvoiceReturnHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SaleInvoiceReturnHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
