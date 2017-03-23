using Surya.India.Data.Infrastructure;
using Surya.India.Data.Models;
using Surya.India.Model;
using Surya.India.Model.Models;
using Surya.India.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surya.India.Model.ViewModels;

namespace Surya.India.Service
{
    public interface IDispatchWaybillHeaderService : IDisposable
    {
        DispatchWaybillHeader Create(DispatchWaybillHeader s);
        void Delete(int id);
        void Delete(DispatchWaybillHeader s);
        DispatchWaybillHeader GetDispatchWaybillHeader(int id);

        DispatchWaybillHeader Find(int id);
        IQueryable<DispatchWaybillHeaderViewModel> GetDispatchWaybillHeaderViewModelForIndex(int DocTypeId);
        IQueryable<DispatchWaybillHeaderViewModel> GetDispatchWaybillHeaderListPendingToSubmit();
        IQueryable<DispatchWaybillHeaderViewModel> GetDispatchWaybillHeaderListPendingToApprove();
        DispatchWaybillHeaderViewModel GetDispatchWaybillHeaderViewModel(int id);
        
        void Update(DispatchWaybillHeader s);
        string GetMaxDocNo();
        DispatchWaybillHeader FindByDocNo(string Docno, int DivisionId, int SiteId);
        int NextId(int id);
        int PrevId(int id);

       
    }
    public class DispatchWaybillHeaderService : IDispatchWaybillHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public DispatchWaybillHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

     public DispatchWaybillHeader Create(DispatchWaybillHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<DispatchWaybillHeader>().Insert(s);
            return s;
        }

       public void Delete(int id)
     {
         _unitOfWork.Repository<DispatchWaybillHeader>().Delete(id);
     }
       public void Delete(DispatchWaybillHeader s)
        {
            _unitOfWork.Repository<DispatchWaybillHeader>().Delete(s);
        }
       public void Update(DispatchWaybillHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<DispatchWaybillHeader>().Update(s);            
        }

       public DispatchWaybillHeader GetDispatchWaybillHeader(int id)
        {
            return _unitOfWork.Repository<DispatchWaybillHeader>().Query().Get().Where(m => m.DispatchWaybillHeaderId == id).FirstOrDefault();
        }

       public DispatchWaybillHeader Find(int id)
       {
           return _unitOfWork.Repository<DispatchWaybillHeader>().Find(id);
       }

       public DispatchWaybillHeader FindByDocNo(string Docno, int DivisionId, int SiteId)
       {
           return _unitOfWork.Repository<DispatchWaybillHeader>().Query().Get().Where(m => m.DocNo == Docno && m.DivisionId == DivisionId && m.SiteId == SiteId ).FirstOrDefault();

       }

       public int NextId(int id)
       {
           int temp = 0;
           if (id != 0)
           {

               temp = (from p in db.DispatchWaybillHeader
                       orderby p.DocDate descending, p.DocNo descending
                       select p.DispatchWaybillHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();


           }
           else
           {
               temp = (from p in db.DispatchWaybillHeader
                       orderby p.DocDate descending, p.DocNo descending
                       select p.DispatchWaybillHeaderId).FirstOrDefault();
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

               temp = (from p in db.DispatchWaybillHeader
                       orderby p.DocDate descending, p.DocNo descending
                       select p.DispatchWaybillHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
           }
           else
           {
               temp = (from p in db.DispatchWaybillHeader
                       orderby p.DocDate descending, p.DocNo descending
                       select p.DispatchWaybillHeaderId).AsEnumerable().LastOrDefault();
           }
           if (temp != 0)
               return temp;
           else
               return id;
       }

       public string GetMaxDocNo()
       {
           int x;
           var maxVal = _unitOfWork.Repository<DispatchWaybillHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
           return (maxVal + 1).ToString();
       }

       public void Dispose()
       {
       }

       public IQueryable<DispatchWaybillHeaderViewModel> GetDispatchWaybillHeaderViewModelForIndex(int DocTypeId)
       {
           IQueryable<DispatchWaybillHeaderViewModel> DispatchWaybillHeader = from H in db.DispatchWaybillHeader
                                                                  join p in db.Persons on H.ConsigneeId equals p.PersonID into PersonTable
                                                                  from PersonTab in PersonTable.DefaultIfEmpty()
                                                                  join s in db.SaleInvoiceHeader on H.SaleInvoiceHeaderId equals s.SaleInvoiceHeaderId into SaleInvoiceHeaderTable
                                                                  from SaleInvoiceHeaderTab in SaleInvoiceHeaderTable.DefaultIfEmpty()
                                                                  where H.DocTypeId == DocTypeId
                                                                  orderby H.DispatchWaybillHeaderId
                                                                  select new DispatchWaybillHeaderViewModel
                                                                  {
                                                                      DispatchWaybillHeaderId = H.DispatchWaybillHeaderId,
                                                                      DocDate = H.DocDate,
                                                                      DocNo = H.DocNo,
                                                                      ConsigneeName = PersonTab.Name,
                                                                      SaleInvoiceHeaderDocNo = SaleInvoiceHeaderTab.DocNo,
                                                                      WaybillNo = H.WaybillNo,
                                                                      WaybillDate = H.WaybillDate,
                                                                      Remark = H.Remark,
                                                                      Status = H.Status
                                                                  };

           return DispatchWaybillHeader;
       }


       public IQueryable<DispatchWaybillHeaderViewModel> GetDispatchWaybillHeaderList()
        {
            IQueryable<DispatchWaybillHeaderViewModel> DispatchWaybillHeaderlist = from H in db.DispatchWaybillHeader
                                                                     join B in db.Persons on H.ConsigneeId equals B.PersonID into ConsigneeTable
                                                                     from ConsigneeTab in ConsigneeTable.DefaultIfEmpty()
                                                                     orderby H.DocDate descending, H.DocNo descending
                                                                     select new DispatchWaybillHeaderViewModel
                                                                     {
                                                                         DispatchWaybillHeaderId = H.DispatchWaybillHeaderId,
                                                                         DocDate = H.DocDate,
                                                                         DocNo = H.DocNo,
                                                                         ConsigneeName = ConsigneeTab.Name,
                                                                         Remark = H.Remark,
                                                                         Status = H.Status,
                                                                     };

            return DispatchWaybillHeaderlist;                             
        }
        
        public IQueryable<DispatchWaybillHeaderViewModel> GetDispatchWaybillHeaderListPendingToSubmit()
       {
           IQueryable<DispatchWaybillHeaderViewModel> DispatchWaybillHeaderlistpendingtosubmit = from H in db.DispatchWaybillHeader
                                                                                   join B in db.Persons on H.ConsigneeId equals B.PersonID into ConsigneeTable
                                                                                   from ConsigneeTab in ConsigneeTable.DefaultIfEmpty()
                                                                                   orderby H.DocDate descending, H.DocNo descending
                                                                                   where H.Status == (int)StatusConstants.Drafted || H.Status == (int)StatusConstants.Modified
                                                                                   select new DispatchWaybillHeaderViewModel
                                                                                   {
                                                                                       DispatchWaybillHeaderId = H.DispatchWaybillHeaderId,
                                                                                       DocDate = H.DocDate,
                                                                                       DocNo = H.DocNo,
                                                                                       ConsigneeName = ConsigneeTab.Name,
                                                                                       Remark = H.Remark,
                                                                                       Status = H.Status,
                                                                                   };
           return DispatchWaybillHeaderlistpendingtosubmit;   
       }

        public IQueryable<DispatchWaybillHeaderViewModel> GetDispatchWaybillHeaderListPendingToApprove()
        {
            IQueryable<DispatchWaybillHeaderViewModel> DispatchWaybillHeaderlistpendingtoapprove = from H in db.DispatchWaybillHeader
                                                                                     join B in db.Persons on H.ConsigneeId equals B.PersonID into ConsigneeTable
                                                                                     from ConsigneeTab in ConsigneeTable.DefaultIfEmpty()
                                                                                     orderby H.DocDate descending, H.DocNo descending
                                                                                     where H.Status == (int)StatusConstants.Submitted || H.Status == (int)StatusConstants.ModificationSubmitted
                                                                                     select new DispatchWaybillHeaderViewModel
                                                                                     {
                                                                                         DispatchWaybillHeaderId = H.DispatchWaybillHeaderId,
                                                                                         DocDate = H.DocDate,
                                                                                         DocNo = H.DocNo,
                                                                                         ConsigneeName = ConsigneeTab.Name,
                                                                                         Remark = H.Remark,
                                                                                         Status = H.Status,
                                                                                     };

            return DispatchWaybillHeaderlistpendingtoapprove;   
        }


        public DispatchWaybillHeaderViewModel GetDispatchWaybillHeaderViewModel(int id)
        {
            DispatchWaybillHeaderViewModel packingheaderlistpendingtoapprove = (from H in db.DispatchWaybillHeader
                                                                          join B in db.Persons on H.ConsigneeId equals B.PersonID into ConsigneeTable
                                                                          from ConsigneeTab in ConsigneeTable.DefaultIfEmpty()
                                                                          orderby H.DocDate descending, H.DocNo descending
                                                                          where H.DispatchWaybillHeaderId == id
                                                                          select new DispatchWaybillHeaderViewModel
                                                                          {
                                                                              DispatchWaybillHeaderId = H.DispatchWaybillHeaderId,
                                                                              DocDate = H.DocDate,
                                                                              DocNo = H.DocNo,
                                                                              ConsigneeName = ConsigneeTab.Name,
                                                                              RouteId = H.RouteId,
                                                                              Remark = H.Remark,
                                                                              Status = H.Status,
                                                                          }).FirstOrDefault();

            return packingheaderlistpendingtoapprove;
        }
    }
}
