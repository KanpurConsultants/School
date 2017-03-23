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
using Surya.India.Model.ViewModels;
using Surya.India.Model.ViewModel;
using System.Data.Entity.SqlServer;

namespace Surya.India.Service
{
    public interface IJobReceiveHeaderService : IDisposable
    {
        JobReceiveHeader Create(JobReceiveHeader pt);
        void Delete(int id);
        void Delete(JobReceiveHeader pt);
        JobReceiveHeader Find(int id);
        IEnumerable<JobReceiveHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(JobReceiveHeader pt);
        JobReceiveHeader Add(JobReceiveHeader pt);
        IQueryable<JobReceiveIndexViewModel> GetJobReceiveHeaderList(int DocTypeId, string Uname);//DocumentTypeId
        IQueryable<JobReceiveIndexViewModel> GetJobReceiveHeaderListPendingToSubmit(int DocTypeId, string Uname);//DocumentTypeId
        IQueryable<JobReceiveIndexViewModel> GetJobReceiveHeaderListPendingToReview(int DocTypeId, string Uname);//DocumentTypeId
        JobReceiveHeaderViewModel GetJobReceiveHeader(int id);

        // IEnumerable<JobReceiveHeader> GetJobReceiveHeaderList(int buyerId);
        Task<IEquatable<JobReceiveHeader>> GetAsync();
        Task<JobReceiveHeader> FindAsync(int id);
        JobReceiveHeader GetJobReceiveHeaderByName(string terms);
        int NextId(int id);
        int PrevId(int id);
        string GetMaxDocNo();


    }

    public class JobReceiveHeaderService : IJobReceiveHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<JobReceiveHeader> _JobReceiveHeaderRepository;
        RepositoryQuery<JobReceiveHeader> JobReceiveHeaderRepository;
        public JobReceiveHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _JobReceiveHeaderRepository = new Repository<JobReceiveHeader>(db);
            JobReceiveHeaderRepository = new RepositoryQuery<JobReceiveHeader>(_JobReceiveHeaderRepository);
        }
        public JobReceiveHeader GetJobReceiveHeaderByName(string terms)
        {
            return (from p in db.JobReceiveHeader
                    where p.DocNo == terms
                    select p).FirstOrDefault();
        }


        public JobReceiveHeader Find(int id)
        {
            return _unitOfWork.Repository<JobReceiveHeader>().Find(id);
        }

        public JobReceiveHeader Create(JobReceiveHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<JobReceiveHeader>().Insert(pt);
            return pt;
        }
        public JobReceiveHeaderViewModel GetJobReceiveHeader(int id)
        {
            return (from p in db.JobReceiveHeader
                    where p.JobReceiveHeaderId == id
                    select new JobReceiveHeaderViewModel
                    {
                        DivisionId=p.DivisionId,
                        DivisionName=p.Division.DivisionName,
                        DocTypeName=p.DocType.DocumentTypeName,
                        GodownName=p.Godown.GodownName,
                        JobReceiveByName=p.JobReceiveBy.Person.Name,
                        JobReceiveById=p.JobReceiveById,
                        JobWorkerId=p.JobWorkerId,
                        JobWorkerName=p.JobWorker.Person.Name,
                        ProcessId=p.ProcessId,
                        ProcessName=p.Process.ProcessName,
                        Status=p.Status,
                        JobWorkerDocNo=p.JobWorkerDocNo,
                        SiteId=p.SiteId,
                        SiteName=p.Site.SiteName,
                        JobReceiveHeaderId=p.JobReceiveHeaderId,
                        DocTypeId = p.DocTypeId,
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        GodownId=p.GodownId,
                        Remark=p.Remark,
                        ModifiedBy=p.ModifiedBy,

                    }
                        ).FirstOrDefault();
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<JobReceiveHeader>().Delete(id);
        }

        public void Delete(JobReceiveHeader pt)
        {
            _unitOfWork.Repository<JobReceiveHeader>().Delete(pt);
        }

        public void Update(JobReceiveHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<JobReceiveHeader>().Update(pt);
        }

        public IEnumerable<JobReceiveHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<JobReceiveHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DocNo))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<JobReceiveIndexViewModel> GetJobReceiveHeaderList(int DocTypeId, string Uname)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            return (from p in db.JobReceiveHeader
                    orderby p.DocDate descending,p.DocNo descending
                    where p.SiteId == SiteId && p.DivisionId == DivisionId && p.DocTypeId == DocTypeId
                    select new JobReceiveIndexViewModel
                    {
                        JobReceiveHeaderId=p.JobReceiveHeaderId,
                        DocDate = p.DocDate,
                        JobWorkerDocNo=p.JobWorkerDocNo,
                        DocNo = p.DocNo,
                        JobWorkerName=p.JobWorker.Person.Name,
                        DocTypeName = p.DocType.DocumentTypeName,
                        Remark = p.Remark,
                        Status=p.Status,
                        ModifiedBy=p.ModifiedBy,
                        ReviewCount = p.ReviewCount,
                        ReviewBy=p.ReviewBy,
                        Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
                    }
                );
        }

        public JobReceiveHeader Add(JobReceiveHeader pt)
        {
            _unitOfWork.Repository<JobReceiveHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.JobReceiveHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.JobReceiveHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.JobReceiveHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.JobReceiveHeaderId).FirstOrDefault();
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

                temp = (from p in db.JobReceiveHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.JobReceiveHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.JobReceiveHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.JobReceiveHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }
        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<JobReceiveHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }


        public IEnumerable<JobReceiveListViewModel> GetPendingReceipts(int id, int Jid)
        {
            return (from p in db.ViewJobReceiveBalance
                    join t in db.JobReceiveHeader on p.JobReceiveHeaderId equals t.JobReceiveHeaderId into table
                    from tab in table.DefaultIfEmpty()
                    join t1 in db.JobOrderLine on p.JobOrderLineId equals t1.JobOrderLineId into table1
                    from tab1 in table1.DefaultIfEmpty()
                    where p.ProductId == id && p.BalanceQty > 0 && p.JobWorkerId==Jid
                    select new JobReceiveListViewModel
                    {
                        JobReceiveLineId = p.JobReceiveLineId,
                        JobReceiveHeaderId = p.JobReceiveHeaderId,
                        DocNo = tab.DocNo,
                        JobWorkerDocNo = tab.JobWorkerDocNo,
                        Dimension1Name = tab1.Dimension1.Dimension1Name,
                        Dimension2Name = tab1.Dimension2.Dimension2Name,
                    }
                        );
        }

        public IEnumerable<JobReceiveHeaderListViewModel> GetPendingJobReceivesWithPatternMatch(int JobWorkerId, string term, int Limiter)//Product Id
        {
            var tem = (from p in db.ViewJobReceiveBalance
                       where p.BalanceQty > 0 && p.JobWorkerId == JobWorkerId
                       && ((string.IsNullOrEmpty(term) ? 1 == 1 : p.JobReceiveNo.ToLower().Contains(term.ToLower()))
                       || (string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower()))
                       || (string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower()))
                       || (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower())))
                       orderby p.JobReceiveNo
                       select new JobReceiveHeaderListViewModel
                       {
                           DocNo = p.JobReceiveNo,
                           JobReceiveLineId = p.JobReceiveLineId,
                           Dimension1Name = p.Dimension1.Dimension1Name,
                           Dimension2Name = p.Dimension2.Dimension2Name,
                           ProductName = p.Product.ProductName,

                       }).Take(Limiter);

            return (tem);
        }

        public IQueryable<JobReceiveIndexViewModel> GetJobReceiveHeaderListPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobReceiveHeaderList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }
        public IQueryable<JobReceiveIndexViewModel> GetJobReceiveHeaderListPendingToReview(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobReceiveHeaderList(id, Uname).AsQueryable();

            var PendingToReview = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname, (p.ReviewBy ?? "")) == 0)
                                   select p;
            return PendingToReview;

        }
        public void Dispose()
        {
        }


        public Task<IEquatable<JobReceiveHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<JobReceiveHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
