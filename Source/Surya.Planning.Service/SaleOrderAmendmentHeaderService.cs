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

namespace Surya.India.Service
{
    public interface ISaleOrderAmendmentHeaderService : IDisposable
    {
        SaleOrderAmendmentHeader Create(SaleOrderAmendmentHeader pt);
        void Delete(int id);
        void Delete(SaleOrderAmendmentHeader pt);
        SaleOrderAmendmentHeader Find(int id);
        IEnumerable<SaleOrderAmendmentHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(SaleOrderAmendmentHeader pt);
        SaleOrderAmendmentHeader Add(SaleOrderAmendmentHeader pt);
        IQueryable<SaleOrderAmendmentHeaderIndexViewModel> GetSaleOrderAmendmentHeaderList(int id);
        Task<IEquatable<SaleOrderAmendmentHeader>> GetAsync();
        Task<SaleOrderAmendmentHeader> FindAsync(int id);
        SaleOrderAmendmentHeader GetSaleOrderAmendmentHeaderByName(string terms);
        int NextId(int id);
        int PrevId(int id);
        string GetMaxDocNo();
    }

    public class SaleOrderAmendmentHeaderService : ISaleOrderAmendmentHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public SaleOrderAmendmentHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public SaleOrderAmendmentHeader GetSaleOrderAmendmentHeaderByName(string terms)
        {
            return (from p in db.SaleOrderAmendmentHeader
                    where p.DocNo == terms
                    select p).FirstOrDefault();
        }


        public SaleOrderAmendmentHeader Find(int id)
        {
            return _unitOfWork.Repository<SaleOrderAmendmentHeader>().Find(id);
        }

        public SaleOrderAmendmentHeader Create(SaleOrderAmendmentHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleOrderAmendmentHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleOrderAmendmentHeader>().Delete(id);
        }

        public void Delete(SaleOrderAmendmentHeader pt)
        {
            _unitOfWork.Repository<SaleOrderAmendmentHeader>().Delete(pt);
        }

        public void Update(SaleOrderAmendmentHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleOrderAmendmentHeader>().Update(pt);
        }

        public IEnumerable<SaleOrderAmendmentHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<SaleOrderAmendmentHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DocNo))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<SaleOrderAmendmentHeaderIndexViewModel> GetSaleOrderAmendmentHeaderList(int id)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            return (from p in db.SaleOrderAmendmentHeader
                    orderby  p.DocDate descending,p.DocNo descending
                    where p.SiteId==SiteId && p.DivisionId==p.DivisionId && p.DocTypeId==id
                    select new SaleOrderAmendmentHeaderIndexViewModel
                    {
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        BuyerName=p.Buyer.Person.Name,
                        SaleOrderAmendmentHeaderId = p.SaleOrderAmendmentHeaderId,
                        ReasonName = p.Reason.ReasonName,
                        Remark = p.Remark,
                        Status = p.Status,  
                        ModifiedBy=p.ModifiedBy,
                    }
                        );
        }

        public SaleOrderAmendmentHeader Add(SaleOrderAmendmentHeader pt)
        {
            _unitOfWork.Repository<SaleOrderAmendmentHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.SaleOrderAmendmentHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.SaleOrderAmendmentHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.SaleOrderAmendmentHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.SaleOrderAmendmentHeaderId).FirstOrDefault();
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

                temp = (from p in db.SaleOrderAmendmentHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.SaleOrderAmendmentHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.SaleOrderAmendmentHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.SaleOrderAmendmentHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }
        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<SaleOrderAmendmentHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<SaleOrderAmendmentHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SaleOrderAmendmentHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
