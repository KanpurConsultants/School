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
    public interface IRequisitionCancelHeaderService : IDisposable
    {
        RequisitionCancelHeader Create(RequisitionCancelHeader pt);
        void Delete(int id);
        void Delete(RequisitionCancelHeader pt);
        RequisitionCancelHeader Find(int id);
        void Update(RequisitionCancelHeader pt);
        RequisitionCancelHeader Add(RequisitionCancelHeader pt);
        IQueryable<RequisitionCancelHeaderViewModel> GetRequisitionCancelHeaderList(int id);
        Task<IEquatable<RequisitionCancelHeader>> GetAsync();
        Task<RequisitionCancelHeader> FindAsync(int id);
        RequisitionCancelHeader GetRequisitionCancelHeaderByName(string terms);
        int NextId(int id);
        int PrevId(int id);
    }

    public class RequisitionCancelHeaderService : IRequisitionCancelHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public RequisitionCancelHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public RequisitionCancelHeader GetRequisitionCancelHeaderByName(string terms)
        {
            return (from p in db.RequisitionCancelHeader
                    where p.DocNo == terms
                    select p).FirstOrDefault();
        }


        public RequisitionCancelHeader Find(int id)
        {
            return _unitOfWork.Repository<RequisitionCancelHeader>().Find(id);
        }

        public RequisitionCancelHeader Create(RequisitionCancelHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<RequisitionCancelHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<RequisitionCancelHeader>().Delete(id);
        }

        public void Delete(RequisitionCancelHeader pt)
        {
            _unitOfWork.Repository<RequisitionCancelHeader>().Delete(pt);
        }

        public void Update(RequisitionCancelHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<RequisitionCancelHeader>().Update(pt);
        }

        public IQueryable<RequisitionCancelHeaderViewModel> GetRequisitionCancelHeaderList(int id)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            return (from p in db.RequisitionCancelHeader
                    orderby p.DocDate descending, p.DocNo descending
                    where p.SiteId == SiteId && p.DivisionId == DivisionId && p.DocTypeId == id
                    select new RequisitionCancelHeaderViewModel
                    {
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        RequisitionCancelHeaderId = p.RequisitionCancelHeaderId,
                        ReasonName = p.Reason.ReasonName,
                        Remark = p.Remark,
                        Status = p.Status,
                        PersonName = p.Person.Name,
                        ModifiedBy = p.ModifiedBy,
                    }
                        );
        }

        public RequisitionCancelHeader Add(RequisitionCancelHeader pt)
        {
            _unitOfWork.Repository<RequisitionCancelHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.RequisitionCancelHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.RequisitionCancelHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.RequisitionCancelHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.RequisitionCancelHeaderId).FirstOrDefault();
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

                temp = (from p in db.RequisitionCancelHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.RequisitionCancelHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.RequisitionCancelHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.RequisitionCancelHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<RequisitionCancelHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RequisitionCancelHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
