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

namespace Surya.India.Service
{
    public interface ICalculationHeaderLedgerAccountService : IDisposable
    {
        CalculationHeaderLedgerAccount Create(CalculationHeaderLedgerAccount pt);
        void Delete(int id);
        void Delete(CalculationHeaderLedgerAccount pt);
        CalculationHeaderLedgerAccount Find(int id);
        IEnumerable<CalculationHeaderLedgerAccount> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(CalculationHeaderLedgerAccount pt);
        CalculationHeaderLedgerAccount Add(CalculationHeaderLedgerAccount pt);
        IEnumerable<CalculationHeaderLedgerAccountViewModel> GetCalculationHeaderLedgerAccountList(int CalculationID);
        Task<IEquatable<CalculationHeaderLedgerAccount>> GetAsync();
        Task<CalculationHeaderLedgerAccount> FindAsync(int id);
        IEnumerable<CalculationHeaderLedgerAccountViewModel> GetCalculationListForIndex(int id, int DocTypeId);//CalculationId
        CalculationHeaderLedgerAccountViewModel GetCalculationHeaderLedgerAccount(int id, int DocTypeId);//CalculationHeaderLedgerAccountId  
        CalculationHeaderLedgerAccountViewModel GetCalculationHeaderLedgerAccount(int id);//CalculationHeaderLedgerAccountId  
        IQueryable<CalculationHeaderLedgerAccountViewModel> GetHeaderIndex();
        IEnumerable<ComboBoxList> GetProductFooters(int id, string term);
        int NextId(int id);
        int PrevId(int id);
    }

    public class CalculationHeaderLedgerAccountService : ICalculationHeaderLedgerAccountService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<CalculationHeaderLedgerAccount> _CalculationHeaderLedgerAccountRepository;
        RepositoryQuery<CalculationHeaderLedgerAccount> CalculationHeaderLedgerAccountRepository;

        public CalculationHeaderLedgerAccountService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _CalculationHeaderLedgerAccountRepository = new Repository<CalculationHeaderLedgerAccount>(db);
            CalculationHeaderLedgerAccountRepository = new RepositoryQuery<CalculationHeaderLedgerAccount>(_CalculationHeaderLedgerAccountRepository);
        }


        public CalculationHeaderLedgerAccount Find(int id)
        {
            return _unitOfWork.Repository<CalculationHeaderLedgerAccount>().Find(id);
        }
        public CalculationHeaderLedgerAccountViewModel GetCalculationHeaderLedgerAccount(int id, int DocTypeId)
        {
            return (from p in db.CalculationHeaderLedgerAccount
                    where p.CalculationId == id && p.DocTypeId == DocTypeId
                    select new CalculationHeaderLedgerAccountViewModel
                    {
                        CalculationId = p.CalculationId,
                        DocTypeId = p.DocTypeId,
                        CalculationName = p.Calculation.CalculationName,
                        DocTypeName = p.DocType.DocumentTypeName,
                    }
                        ).FirstOrDefault();
        }

        public CalculationHeaderLedgerAccountViewModel GetCalculationHeaderLedgerAccount(int id)
        {
            return (from p in db.CalculationHeaderLedgerAccount
                    where p.CalculationHeaderLedgerAccountId == id
                    select new CalculationHeaderLedgerAccountViewModel
                    {
                        CalculationFooterId = p.CalculationFooterId,
                        CalculationFooterName = p.CalculationFooter.Charge.ChargeName,
                        CalculationHeaderLedgerAccountId = p.CalculationHeaderLedgerAccountId,
                        CalculationId = p.CalculationId,
                        CalculationName = p.Calculation.CalculationName,
                        CostCenterId = p.CostCenterId,
                        CostCenterName = p.CostCenter.CostCenterName,
                        DocTypeId = p.DocTypeId,
                        DocTypeName = p.DocType.DocumentTypeName,
                        LedgerAccountCrId = p.LedgerAccountCrId,
                        LedgerAccountCrName = p.LedgerAccountCr.LedgerAccountName,
                        LedgerAccountDrId = p.LedgerAccountDrId,
                        LedgerAccountDrName = p.LedgerAccountDr.LedgerAccountName,
                        ContraLedgerAccountId=p.ContraLedgerAccountId,
                        ContraLedgerAccountName=p.ContraLedgerAccount.LedgerAccountName,
                    }
                        ).FirstOrDefault();
        }

        public IEnumerable<CalculationHeaderLedgerAccountViewModel> GetCalculationListForIndex(int id, int DocTypeId)
        {

            List<CalculationHeaderLedgerAccountViewModel> Records = new List<CalculationHeaderLedgerAccountViewModel>();

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            Records = (from p in db.CalculationHeaderLedgerAccount
                       where p.CalculationId == id && p.DocTypeId == DocTypeId && p.SiteId==SiteId && p.DivisionId==DivisionId
                       orderby p.CalculationHeaderLedgerAccountId
                       select new CalculationHeaderLedgerAccountViewModel
                       {
                           CalculationFooterId = p.CalculationFooterId,
                           CalculationFooterName = p.CalculationFooter.Charge.ChargeName,
                           CalculationHeaderLedgerAccountId = p.CalculationHeaderLedgerAccountId,
                           CalculationId = p.CalculationId,
                           CalculationName = p.Calculation.CalculationName,
                           CostCenterId = p.CostCenterId,
                           CostCenterName = p.CostCenter.CostCenterName,
                           DocTypeId = p.DocTypeId,
                           DocTypeName = p.DocType.DocumentTypeName,
                           LedgerAccountCrId = p.LedgerAccountCrId,
                           LedgerAccountCrName = p.LedgerAccountCr.LedgerAccountName,
                           LedgerAccountDrId = p.LedgerAccountDrId,
                           LedgerAccountDrName = p.LedgerAccountDr.LedgerAccountName,
                           ContraLedgerAccountName=p.ContraLedgerAccount.LedgerAccountName,
                           ContraLedgerAccountId=p.ContraLedgerAccountId,
                       }).ToList();

            var PendingRecords = (from p in db.CalculationFooter
                                  join t in db.CalculationHeaderLedgerAccount.Where(m=>m.DocTypeId==DocTypeId) on p.CalculationFooterLineId equals t.CalculationFooterId into table
                                  from tab in table.DefaultIfEmpty()
                                  where tab == null && p.CalculationId==id
                                  select new CalculationHeaderLedgerAccountViewModel
                                  {
                                      CalculationFooterId = p.CalculationFooterLineId,
                                      CalculationFooterName = p.Charge.ChargeName,
                                      CalculationId = p.CalculationId,
                                      CalculationName = p.Calculation.CalculationName,
                                      CostCenterId = p.CostCenterId,
                                      CostCenterName = p.CostCenter.CostCenterName,
                                      DocTypeId = DocTypeId,
                                  });

            foreach (var item in PendingRecords)
                Records.Add(item);

            return (Records);

        }

        public CalculationHeaderLedgerAccount Create(CalculationHeaderLedgerAccount pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<CalculationHeaderLedgerAccount>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<CalculationHeaderLedgerAccount>().Delete(id);
        }

        public void Delete(CalculationHeaderLedgerAccount pt)
        {
            _unitOfWork.Repository<CalculationHeaderLedgerAccount>().Delete(pt);
        }

        public void Update(CalculationHeaderLedgerAccount pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<CalculationHeaderLedgerAccount>().Update(pt);
        }

        public IEnumerable<CalculationHeaderLedgerAccount> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<CalculationHeaderLedgerAccount>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.CalculationHeaderLedgerAccountId))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<CalculationHeaderLedgerAccountViewModel> GetCalculationHeaderLedgerAccountList(int CalculationID)
        {
            return (from p in db.CalculationHeaderLedgerAccount
                    where p.CalculationId == CalculationID
                    orderby p.CalculationHeaderLedgerAccountId
                    select new CalculationHeaderLedgerAccountViewModel
                    {
                        CalculationFooterId = p.CalculationFooterId,
                        CalculationFooterName = p.CalculationFooter.Charge.ChargeName,
                        CalculationHeaderLedgerAccountId = p.CalculationHeaderLedgerAccountId,
                        CalculationId = p.CalculationId,
                        CalculationName = p.Calculation.CalculationName,
                        CostCenterId = p.CostCenterId,
                        CostCenterName = p.CostCenter.CostCenterName,
                        DocTypeId = p.DocTypeId,
                        DocTypeName = p.DocType.DocumentTypeName,
                        LedgerAccountCrId = p.LedgerAccountCrId,
                        LedgerAccountCrName = p.LedgerAccountCr.LedgerAccountName,
                        LedgerAccountDrId = p.LedgerAccountDrId,
                        LedgerAccountDrName = p.LedgerAccountDr.LedgerAccountName,
                        ContraLedgerAccountName = p.ContraLedgerAccount.LedgerAccountName,
                        ContraLedgerAccountId = p.ContraLedgerAccountId,
                    }
                        );
        }

        public CalculationHeaderLedgerAccount Add(CalculationHeaderLedgerAccount pt)
        {
            _unitOfWork.Repository<CalculationHeaderLedgerAccount>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.CalculationHeaderLedgerAccount
                        orderby p.CalculationHeaderLedgerAccountId
                        select p.CalculationHeaderLedgerAccountId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.CalculationHeaderLedgerAccount
                        orderby p.CalculationHeaderLedgerAccountId
                        select p.CalculationHeaderLedgerAccountId).FirstOrDefault();
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
                temp = (from p in db.CalculationHeaderLedgerAccount
                        orderby p.CalculationHeaderLedgerAccountId
                        select p.CalculationHeaderLedgerAccountId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.CalculationHeaderLedgerAccount
                        orderby p.CalculationHeaderLedgerAccountId
                        select p.CalculationHeaderLedgerAccountId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public IQueryable<CalculationHeaderLedgerAccountViewModel> GetHeaderIndex()
        {

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            return (from p in db.CalculationHeaderLedgerAccount
                    where p.SiteId==SiteId && p.DivisionId==DivisionId
                    group p by new { p.DocTypeId, p.CalculationId } into g
                    orderby g.Key.CalculationId
                    select new CalculationHeaderLedgerAccountViewModel
                    {
                        DocTypeId = g.Key.DocTypeId,
                        CalculationId = g.Key.CalculationId,
                        DocTypeName = g.Max(i => i.DocType.DocumentTypeName),
                        CalculationName = g.Max(i => i.Calculation.CalculationName),
                    }
                         );
        }

        public IEnumerable<ComboBoxList> GetProductFooters(int id, string term)
        {

            return (from p in db.CalculationFooter
                    join t in db.Charge on p.ChargeId equals t.ChargeId
                    where p.CalculationId == id && string.IsNullOrEmpty(term) ? 1 == 1 : t.ChargeName.ToLower().Contains(term.ToLower())
                    select new ComboBoxList
                    {
                        Id = p.CalculationFooterLineId,
                        PropFirst = t.ChargeName,
                    }
                        ).Take(25);

        }

        public void Dispose()
        {
        }


        public Task<IEquatable<CalculationHeaderLedgerAccount>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CalculationHeaderLedgerAccount> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
