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
using System.Data.Entity.SqlServer;

namespace Surya.India.Service
{
    public interface IJobOrderCancelHeaderService : IDisposable
    {
        JobOrderCancelHeader Create(JobOrderCancelHeader pt);
        void Delete(int id);
        void Delete(JobOrderCancelHeader pt);
        JobOrderCancelHeader Find(int id);
        IEnumerable<JobOrderCancelHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(JobOrderCancelHeader pt);
        JobOrderCancelHeader Add(JobOrderCancelHeader pt);
        IQueryable<JobOrderCancelHeaderIndexViewModel> GetJobOrderCancelHeaderList(int id, string Uname);
        IQueryable<JobOrderCancelHeaderIndexViewModel> GetJobOrderCancelHeaderListPendingToSubmit(int id, string Uname);
        IQueryable<JobOrderCancelHeaderIndexViewModel> GetJobOrderCancelHeaderListPendingToReview(int id, string Uname);

        // IEnumerable<JobOrderCancelHeader> GetJobOrderCancelHeaderList(int buyerId);
        Task<IEquatable<JobOrderCancelHeader>> GetAsync();
        Task<JobOrderCancelHeader> FindAsync(int id);
        JobOrderCancelHeader GetJobOrderCancelHeaderByName(string terms);
        int NextId(int id);
        int PrevId(int id);
        string GetMaxDocNo();
    }

    public class JobOrderCancelHeaderService : IJobOrderCancelHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public JobOrderCancelHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public JobOrderCancelHeader GetJobOrderCancelHeaderByName(string terms)
        {
            return (from p in db.JobOrderCancelHeader
                    where p.DocNo == terms
                    select p).FirstOrDefault();
        }


        public JobOrderCancelHeader Find(int id)
        {
            return _unitOfWork.Repository<JobOrderCancelHeader>().Find(id);
        }

        public JobOrderCancelHeader Create(JobOrderCancelHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<JobOrderCancelHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<JobOrderCancelHeader>().Delete(id);
        }

        public void Delete(JobOrderCancelHeader pt)
        {
            _unitOfWork.Repository<JobOrderCancelHeader>().Delete(pt);
        }

        public void Update(JobOrderCancelHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<JobOrderCancelHeader>().Update(pt);
        }

        public IEnumerable<JobOrderCancelHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<JobOrderCancelHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DocNo))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<JobOrderCancelHeaderIndexViewModel> GetJobOrderCancelHeaderList(int id, string Uname)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"]; 

            return (from p in db.JobOrderCancelHeader
                    orderby p.DocDate descending, p.DocNo descending
                    where p.SiteId == SiteId && p.DivisionId == DivisionId && p.DocTypeId == id
                    select new JobOrderCancelHeaderIndexViewModel
                    {
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        JobOrderCancelHeaderId = p.JobOrderCancelHeaderId,
                        ReasonName = p.Reason.ReasonName,
                        Remark = p.Remark,
                        Status = p.Status,
                        JobWorkerName = p.JobWorker.Person.Name,
                        ModifiedBy=p.ModifiedBy,
                        ReviewCount=p.ReviewCount,
                        ReviewBy=p.ReviewBy,
                        Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
                    }
                        );
        }

        public JobOrderCancelHeader Add(JobOrderCancelHeader pt)
        {
            _unitOfWork.Repository<JobOrderCancelHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.JobOrderCancelHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.JobOrderCancelHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.JobOrderCancelHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.JobOrderCancelHeaderId).FirstOrDefault();
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

                temp = (from p in db.JobOrderCancelHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.JobOrderCancelHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.JobOrderCancelHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.JobOrderCancelHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }
        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<JobOrderCancelHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public IQueryable<JobOrderCancelHeaderIndexViewModel> GetJobOrderCancelHeaderListPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobOrderCancelHeaderList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }

        public IQueryable<JobOrderCancelHeaderIndexViewModel> GetJobOrderCancelHeaderListPendingToReview(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobOrderCancelHeaderList(id, Uname).AsQueryable();

            var PendingToReview = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname, (p.ReviewBy ?? "")) == 0)
                                   select p;
            return PendingToReview;

        }

        public void Dispose()
        {
        }


        public Task<IEquatable<JobOrderCancelHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<JobOrderCancelHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
