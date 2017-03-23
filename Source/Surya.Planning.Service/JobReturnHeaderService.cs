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
using System.Data.Entity.SqlServer;

namespace Surya.India.Service
{
    public interface IJobReturnHeaderService : IDisposable
    {
        JobReturnHeader Create(JobReturnHeader pt);
        void Delete(int id);
        void Delete(JobReturnHeader pt);
        JobReturnHeader Find(string Name);
        JobReturnHeader Find(int id);
        IEnumerable<JobReturnHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(JobReturnHeader pt);
        JobReturnHeader Add(JobReturnHeader pt);
        JobReturnHeaderViewModel GetJobReturnHeader(int id);//HeadeRId
        IQueryable<JobReturnHeaderViewModel> GetJobReturnHeaderList(int id, string Uname);
        IQueryable<JobReturnHeaderViewModel> GetJobReturnHeaderListPendingToSubmit(int id, string Uname);
        IQueryable<JobReturnHeaderViewModel> GetJobReturnHeaderListPendingToReview(int id, string Uname);
        Task<IEquatable<JobReturnHeader>> GetAsync();
        Task<JobReturnHeader> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
        string GetMaxDocNo();
    }

    public class JobReturnHeaderService : IJobReturnHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<JobReturnHeader> _JobReturnHeaderRepository;
        RepositoryQuery<JobReturnHeader> JobReturnHeaderRepository;
        public JobReturnHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _JobReturnHeaderRepository = new Repository<JobReturnHeader>(db);
            JobReturnHeaderRepository = new RepositoryQuery<JobReturnHeader>(_JobReturnHeaderRepository);
        }

        public JobReturnHeader Find(string Name)
        {
            return JobReturnHeaderRepository.Get().Where(i => i.DocNo == Name).FirstOrDefault();
        }


        public JobReturnHeader Find(int id)
        {
            return _unitOfWork.Repository<JobReturnHeader>().Find(id);
        }

        public JobReturnHeader Create(JobReturnHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<JobReturnHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<JobReturnHeader>().Delete(id);
        }

        public void Delete(JobReturnHeader pt)
        {
            _unitOfWork.Repository<JobReturnHeader>().Delete(pt);
        }

        public void Update(JobReturnHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<JobReturnHeader>().Update(pt);
        }

        public IEnumerable<JobReturnHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<JobReturnHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DocNo))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }
        public JobReturnHeaderViewModel GetJobReturnHeader(int id)
        {
            return (from p in db.JobReturnHeader
                    where p.JobReturnHeaderId == id
                    select new JobReturnHeaderViewModel
                    {
                        JobReturnHeaderId = p.JobReturnHeaderId,
                        DivisionId = p.DivisionId,
                        DocNo = p.DocNo,
                        DocDate = p.DocDate,
                        DocTypeId = p.DocTypeId,
                        Remark = p.Remark,
                        SiteId = p.SiteId,
                        GodownId=p.GodownId,
                        Status = p.Status,
                        JobWorkerId = p.JobWorkerId,                                              
                        ReasonId=p.ReasonId,
                        ProcessId = p.ProcessId,
                        OrderById = p.OrderById,
                        ModifiedBy=p.ModifiedBy,
                        GatePassHeaderId = p.GatePassHeaderId,
                        GatePassDocNo = p.GatePassHeader.DocNo,
                        GatePassStatus = (p.GatePassHeader == null ? 0 : p.GatePassHeader.Status),
                        GatePassDocDate = p.GatePassHeader.DocDate,
                    }

                        ).FirstOrDefault();
        }
        public IQueryable<JobReturnHeaderViewModel> GetJobReturnHeaderList(int id, string Uname)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"]; 

            var pt = (from p in db.JobReturnHeader
                      join t in db.DocumentType on p.DocTypeId equals t.DocumentTypeId into table
                      from tab in table.DefaultIfEmpty()
                      join t1 in db.Persons on p.JobWorkerId equals t1.PersonID into table2
                      from tab2 in table2.DefaultIfEmpty()
                      orderby p.DocDate descending, p.DocNo descending
                      where p.SiteId == SiteId && p.DivisionId == DivisionId && p.DocTypeId == id 
                      select new JobReturnHeaderViewModel
                      {
                          DocDate = p.DocDate,
                          DocNo = p.DocNo,
                          DocTypeName = tab.DocumentTypeName,
                          JobReturnHeaderId = p.JobReturnHeaderId,
                          Remark = p.Remark,
                          Status = p.Status,
                          JobWorkerName = tab2.Name,
                          ModifiedBy=p.ModifiedBy,
                          ReviewCount=p.ReviewCount,
                          ReviewBy=p.ReviewBy,
                          Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
                          GatePassDocNo = p.GatePassHeader.DocNo,
                          GatePassHeaderId = p.GatePassHeaderId,
                          GatePassDocDate = p.GatePassHeader.DocDate,
                          GatePassStatus = (p.GatePassHeaderId != null ? p.GatePassHeader.Status : 0),
                      }
                         );
            return pt;
        }

        public JobReturnHeader Add(JobReturnHeader pt)
        {
            _unitOfWork.Repository<JobReturnHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.JobReturnHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.JobReturnHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.JobReturnHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.JobReturnHeaderId).FirstOrDefault();
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

                temp = (from p in db.JobReturnHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.JobReturnHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.JobReturnHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.JobReturnHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<JobReturnHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public IQueryable<JobReturnHeaderViewModel> GetJobReturnHeaderListPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobReturnHeaderList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }

        public IQueryable<JobReturnHeaderViewModel> GetJobReturnHeaderListPendingToReview(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobReturnHeaderList(id, Uname).AsQueryable();

            var PendingToReview = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname, (p.ReviewBy ?? "")) == 0)
                                   select p;
            return PendingToReview;

        }

        public void Dispose()
        {
        }


        public Task<IEquatable<JobReturnHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<JobReturnHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}