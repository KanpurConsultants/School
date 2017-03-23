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

namespace Surya.India.Service
{
    public interface IPackingHeaderService : IDisposable
    {
        PackingHeader Create(PackingHeader s);
        void Delete(int id);
        void Delete(PackingHeader s);
        PackingHeader GetPackingHeader(int id);

        PackingHeaderViewModel GetPackingHeaderViewModel(int id);
        PackingHeader Find(int id);
        IQueryable<PackingHeaderViewModel> GetPackingHeaderList();
        IQueryable<PackingHeaderViewModel> GetPackingHeaderListPendingToSubmit();
        IQueryable<PackingHeaderViewModel> GetPackingHeaderListPendingToApprove();
        
        void Update(PackingHeader s);
        string GetMaxDocNo();
        PackingHeader FindByDocNo(string Docno, int DivisionId, int SiteId);
        int NextId(int id);
        int PrevId(int id);

       
    }
    public class PackingHeaderService : IPackingHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public PackingHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

     public PackingHeader Create(PackingHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<PackingHeader>().Insert(s);
            return s;
        }

       public void Delete(int id)
     {
         _unitOfWork.Repository<PackingHeader>().Delete(id);
     }
       public void Delete(PackingHeader s)
        {
            _unitOfWork.Repository<PackingHeader>().Delete(s);
        }
       public void Update(PackingHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<PackingHeader>().Update(s);            
        }

       public PackingHeader GetPackingHeader(int id)
        {
            return _unitOfWork.Repository<PackingHeader>().Query().Get().Where(m => m.PackingHeaderId == id).FirstOrDefault();
        }

       public PackingHeader Find(int id)
       {
           return _unitOfWork.Repository<PackingHeader>().Find(id);
       }

       public PackingHeader FindByDocNo(string Docno, int DivisionId, int SiteId)
       {
           return _unitOfWork.Repository<PackingHeader>().Query().Get().Where(m => m.DocNo == Docno && m.DivisionId == DivisionId && m.SiteId == SiteId ).FirstOrDefault();

       }

       public int NextId(int id)
       {
           int temp = 0;
           if (id != 0)
           {

               temp = (from p in db.PackingHeader
                       orderby p.DocDate descending, p.DocNo descending
                       select p.PackingHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();


           }
           else
           {
               temp = (from p in db.PackingHeader
                       orderby p.DocDate descending, p.DocNo descending
                       select p.PackingHeaderId).FirstOrDefault();
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

               temp = (from p in db.PackingHeader
                       orderby p.DocDate descending, p.DocNo descending
                       select p.PackingHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
           }
           else
           {
               temp = (from p in db.PackingHeader
                       orderby p.DocDate descending, p.DocNo descending
                       select p.PackingHeaderId).AsEnumerable().LastOrDefault();
           }
           if (temp != 0)
               return temp;
           else
               return id;
       }

       public string GetMaxDocNo()
       {
           int x;
           var maxVal = _unitOfWork.Repository<PackingHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
           return (maxVal + 1).ToString();
       }

       public void Dispose()
       {
       }

        public PackingHeaderViewModel GetPackingHeaderViewModel(int id)
       {

           PackingHeaderViewModel packingheader = (from H in db.PackingHeader
                        join B in db.Persons on H.BuyerId equals B.PersonID into BuyerTable  from BuyerTab in BuyerTable.DefaultIfEmpty()
                        join B in db.Persons on H.JobWorkerId equals B.PersonID into JobWorkerTable from JobWorkerTab in JobWorkerTable.DefaultIfEmpty()
                        join D in db.DocumentType on H.DocTypeId equals D.DocumentTypeId into DocumentTypeTable from DocumentTypeTab in DocumentTypeTable.DefaultIfEmpty()
                        join Div in db.Divisions on H.DivisionId equals Div.DivisionId into DivisionTable from DivisionTab in DivisionTable.DefaultIfEmpty()
                        join S in db.Site on H.SiteId equals S.SiteId into SiteTable from SiteTab in SiteTable.DefaultIfEmpty()
                        join G in db.Godown on H.GodownId equals G.GodownId into GodownTable from GodownTab in GodownTable.DefaultIfEmpty()
                                                   join Du in db.Units on H.DealUnitId equals Du.UnitId into DeliveryUnitTable
                                                   from DeliveryUnitTab in DeliveryUnitTable.DefaultIfEmpty()
                        where H.PackingHeaderId == id
                        select new PackingHeaderViewModel
                        {
                            PackingHeaderId = H.PackingHeaderId,
                            DocTypeName = DocumentTypeTab.DocumentTypeName,
                            DocDate = H.DocDate,
                            DocNo = H.DocNo,
                            BuyerId = H.BuyerId,
                            BuyerName = BuyerTab.Name,
                            JobWorkerId = H.JobWorkerId.Value,
                            JobWorkerName = JobWorkerTab.Name,
                            DivisionId = H.DivisionId,
                            DivisionName = DivisionTab.DivisionName,
                            SiteId = H.SiteId,
                            SiteName = SiteTab.SiteName,
                            GodownId = H.GodownId,
                            GodownName = GodownTab.GodownName,
                            DealUnitId = H.DealUnitId,
                            DealUnitName = DeliveryUnitTab.UnitName,
                            BaleNoPattern = H.BaleNoPattern,
                            ShipMethodId = H.ShipMethodId,
                            Remark = H.Remark,
                            Status = H.Status,
                            CreatedBy = H.CreatedBy,
                            CreatedDate = H.CreatedDate,
                            ModifiedBy = H.ModifiedBy,
                            ModifiedDate = H.ModifiedDate
                        }).FirstOrDefault();

           return packingheader;
       }


       public IQueryable<PackingHeaderViewModel> GetPackingHeaderList()
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            IQueryable<PackingHeaderViewModel> packingheaderlist = from H in db.PackingHeader
                                                                   join B in db.Persons on H.BuyerId equals B.PersonID into BuyerTable
                            from BuyerTab in BuyerTable.DefaultIfEmpty()
                            orderby H.DocDate descending, H.DocNo descending
                            where H.SiteId==SiteId && H.DivisionId==DivisionId
                            select new PackingHeaderViewModel
                            {
                                PackingHeaderId = H.PackingHeaderId,
                                DocDate = H.DocDate,
                                DocNo = H.DocNo,
                                BuyerName = BuyerTab.Name,
                                Remark = H.Remark,
                                Status = H.Status,
                                ModifiedBy=H.ModifiedBy,
                            };


            return packingheaderlist;                             
        }
        
        public IQueryable<PackingHeaderViewModel> GetPackingHeaderListPendingToSubmit()
       {
           IQueryable<PackingHeaderViewModel> packingheaderlistpendingtosubmit = from H in db.PackingHeader
                                                                                 join B in db.Persons on H.BuyerId equals B.PersonID into BuyerTable
                            from BuyerTab in BuyerTable.DefaultIfEmpty()
                            orderby H.DocDate descending, H.DocNo descending
                            where H.Status == (int)StatusConstants.Drafted || H.Status == (int)StatusConstants.Modified
                            select new PackingHeaderViewModel
                            {
                                PackingHeaderId = H.PackingHeaderId,
                                DocDate = H.DocDate,
                                DocNo = H.DocNo,
                                BuyerName = BuyerTab.Name,
                                Remark = H.Remark,
                                Status = H.Status,
                            };
           return packingheaderlistpendingtosubmit;   
       }

        public IQueryable<PackingHeaderViewModel> GetPackingHeaderListPendingToApprove()
        {
            IQueryable<PackingHeaderViewModel> packingheaderlistpendingtoapprove = from H in db.PackingHeader
                            join B in db.Persons on H.BuyerId equals B.PersonID into BuyerTable
                            from BuyerTab in BuyerTable.DefaultIfEmpty()
                            orderby H.DocDate descending, H.DocNo descending
                            where H.Status == (int)StatusConstants.Submitted || H.Status == (int)StatusConstants.ModificationSubmitted
                            select new PackingHeaderViewModel
                            {
                                PackingHeaderId = H.PackingHeaderId,
                                DocDate = H.DocDate,
                                DocNo = H.DocNo,
                                BuyerName = BuyerTab.Name,
                                Remark = H.Remark,
                                Status = H.Status,
                            };

            return packingheaderlistpendingtoapprove;   
        }
    }
}
