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
using Surya.India.Model.ViewModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity.SqlServer;

namespace Surya.India.Service
{
    public interface IJobOrderHeaderService : IDisposable
    {
        JobOrderHeader Create(JobOrderHeader s);
        void Delete(int id);
        void Delete(JobOrderHeader s);
        JobOrderHeaderViewModel GetJobOrderHeader(int id);
        JobOrderHeader Find(int id);
        IQueryable<JobOrderHeaderViewModel> GetJobOrderHeaderList(int DocumentTypeId, string Uname);
        IQueryable<JobOrderHeaderViewModel> GetJobOrderHeaderListPendingToSubmit(int DocumentTypeId, string Uname);
        IQueryable<JobOrderHeaderViewModel> GetJobOrderHeaderListPendingToReview(int DocumentTypeId, string Uname);
        void Update(JobOrderHeader s);
        string GetMaxDocNo();        
        IEnumerable<ComboBoxList> GetJobWorkerHelpList(int Processid, string term);//PurchaseOrderHeaderId
        string FGetJobOrderCostCenter(int DocTypeId, DateTime DocDate, int DivisionId, int SiteId);
        IEnumerable<WeavingOrderWizardViewModel> GetProdOrdersForWeavingWizard(int DocTypeId);
        DateTime AddDueDate(DateTime Base, int DueDays);
    }
    public class JobOrderHeaderService : IJobOrderHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public JobOrderHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

        public JobOrderHeader Create(JobOrderHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<JobOrderHeader>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<JobOrderHeader>().Delete(id);
        }
        public void Delete(JobOrderHeader s)
        {
            _unitOfWork.Repository<JobOrderHeader>().Delete(s);
        }
        public void Update(JobOrderHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<JobOrderHeader>().Update(s);
        }


        public JobOrderHeader Find(int id)
        {
            return _unitOfWork.Repository<JobOrderHeader>().Find(id);
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<JobOrderHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public IQueryable<JobOrderHeaderViewModel> GetJobOrderHeaderList(int DocumentTypeId, string Uname)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            return (from p in db.JobOrderHeader
                    orderby p.DocDate descending, p.DocNo descending
                    where p.DocTypeId == DocumentTypeId && p.DivisionId == DivisionId && p.SiteId == SiteId                    
                    select new JobOrderHeaderViewModel
                    {
                        DocTypeName = p.DocType.DocumentTypeName,
                        DueDate = p.DueDate,
                        JobWorkerName = p.JobWorker.Person.Name + "," + p.JobWorker.Person.Suffix,
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        CostCenterName = p.CostCenter.CostCenterName,
                        Remark = p.Remark,
                        Status = p.Status,
                        JobOrderHeaderId = p.JobOrderHeaderId,
                        ModifiedBy = p.ModifiedBy,
                        ReviewCount=p.ReviewCount,
                        ReviewBy=p.ReviewBy,
                        Reviewed=(SqlFunctions.CharIndex(Uname,p.ReviewBy) > 0),
                        GatePassDocNo = p.GatePassHeader.DocNo,
                        GatePassHeaderId = p.GatePassHeaderId,
                        GatePassDocDate = p.GatePassHeader.DocDate,
                        GatePassStatus=(p.GatePassHeaderId!=null ? p.GatePassHeader.Status : 0),
                    });
        }

        public JobOrderHeaderViewModel GetJobOrderHeader(int id)
        {
            return (from p in db.JobOrderHeader
                    where p.JobOrderHeaderId == id
                    select new JobOrderHeaderViewModel
                    {
                        DocTypeName = p.DocType.DocumentTypeName,
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        Remark = p.Remark,
                        JobOrderHeaderId = p.JobOrderHeaderId,
                        Status = p.Status,
                        DocTypeId = p.DocTypeId,
                        DueDate = p.DueDate,
                        ProcessId = p.ProcessId,
                        JobWorkerId = p.JobWorkerId,
                        MachineId = p.MachineId,
                        BillToPartyId = p.BillToPartyId,
                        OrderById = p.OrderById,
                        GodownId = p.GodownId,
                        UnitConversionForId = p.UnitConversionForId,
                        CostCenterId = p.CostCenterId,
                        TermsAndConditions = p.TermsAndConditions,
                        CreditDays = p.CreditDays,
                        DivisionId = p.DivisionId,
                        SiteId = p.SiteId,
                        LockReason=p.LockReason,
                        CostCenterName = p.CostCenter.CostCenterName,
                        ModifiedBy=p.ModifiedBy,
                        GatePassHeaderId = p.GatePassHeaderId,
                        GatePassDocNo = p.GatePassHeader.DocNo,
                        GatePassStatus = (p.GatePassHeader == null ? 0 : p.GatePassHeader.Status),
                        GatePassDocDate = p.GatePassHeader.DocDate,
                    }
                        ).FirstOrDefault();
        }

        
        public IEnumerable<JobOrderHeaderListViewModel> GetPendingJobOrdersWithPatternMatch(int JobWorkerId, string term, int Limiter)//Product Id
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            var tem = (from p in db.ViewJobOrderBalance
                       where p.BalanceQty > 0 && p.JobWorkerId == JobWorkerId
                       && p.DivisionId == DivisionId && p.SiteId == SiteId
                       && ((string.IsNullOrEmpty(term) ? 1 == 1 : p.JobOrderNo.ToLower().Contains(term.ToLower()))
                       || (string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower()))
                       || (string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower()))
                       || (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower())))
                       orderby p.JobOrderNo
                       select new JobOrderHeaderListViewModel
                       {
                           DocNo = p.JobOrderNo,
                           JobOrderLineId = p.JobOrderLineId,
                           Dimension1Name = p.Dimension1.Dimension1Name,
                           Dimension2Name = p.Dimension2.Dimension2Name,
                           ProductName = p.Product.ProductName,
                           BalanceQty = p.BalanceQty,

                       }).Take(Limiter);

            return (tem);
        }

        public IQueryable<JobOrderHeaderViewModel> GetJobOrderHeaderListPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobOrderHeaderList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in JobOrderHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }


        public IEnumerable<ComboBoxList> GetJobWorkerHelpList(int Processid, string term)
        {


            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            string DivId = "|" + CurrentDivisionId.ToString() + "|";
            string SiteId = "|" + CurrentSiteId.ToString() + "|";


            var list = (from b in db.JobWorker
                        join bus in db.BusinessEntity on b.PersonID equals bus.PersonID into BusinessEntityTable
                        from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                        join p in db.Persons on b.PersonID equals p.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        join pp in db.PersonProcess on b.PersonID equals pp.PersonId into PersonProcessTable
                        from PersonProcessTab in PersonProcessTable.DefaultIfEmpty()
                        where PersonProcessTab.ProcessId == Processid
                        && (string.IsNullOrEmpty(term) ? 1 == 1 : PersonTab.Name.ToLower().Contains(term.ToLower()))
                        && BusinessEntityTab.DivisionIds.IndexOf(DivId) != -1
                        && BusinessEntityTab.SiteIds.IndexOf(SiteId) != -1
                        orderby PersonTab.Name
                        select new ComboBoxList
                        {
                            Id = b.PersonID,
                            PropFirst = PersonTab.Name
                            //PropSecond  = BusinessEntityTab.SiteIds.IndexOf(SiteId).ToString() 
                        }
              ).Take(20);

            return list.ToList();
        }


        public IQueryable<JobOrderHeaderViewModel> GetJobOrderHeaderListPendingToReview(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var JobOrderHeader = GetJobOrderHeaderList(id, Uname).AsQueryable();

            var PendingToReview = from p in JobOrderHeader
                                   where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname,(p.ReviewBy??""))==0)
                                   select p;
            return PendingToReview;

        }

        public string FGetJobOrderCostCenter(int DocTypeId, DateTime DocDate, int DivisionId, int SiteId)
        {
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", DocTypeId);
            SqlParameter SqlParameterDocDate = new SqlParameter("@DocDate", DocDate);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", DivisionId);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", SiteId);

            NewDocNoViewModel NewDocNoViewModel = db.Database.SqlQuery<NewDocNoViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".GetJobOrderCostCenter @DocTypeId , @DocDate , @DivisionId , @SiteId ", SqlParameterDocTypeId, SqlParameterDocDate, SqlParameterDivisionId, SqlParameterSiteId).FirstOrDefault();

            if (NewDocNoViewModel != null)
            {
                return NewDocNoViewModel.NewDocNo;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<WeavingOrderWizardViewModel> GetProdOrdersForWeavingWizard(int DocTypeId)//DocTypeId
        {

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", SiteId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", DivisionId);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocumentTypeId", DocTypeId);

            IEnumerable<WeavingOrderWizardViewModel> temp = db.Database.SqlQuery<WeavingOrderWizardViewModel>("Web.ProcWeavingOrderWizard @SiteId, @DivisionId, @DocumentTypeId", SqlParameterSiteId, SqlParameterDivisionId, SqlParameterDocTypeId).ToList();


            //var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId);

            //string[] ContraSites = null;
            //if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            //else { ContraSites = new string[] { "NA" }; }

            //string[] ContraDivisions = null;
            //if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            //else { ContraDivisions = new string[] { "NA" }; }

            //string[] ContraDocTypes = null;
            //if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { ContraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            //else { ContraDocTypes = new string[] { "NA" }; }

            //var temp = from p in db.ViewProdOrderBalance
            //           where (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == SiteId : ContraSites.Contains(p.SiteId.ToString()))
            //           && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId== DivisionId : ContraDivisions.Contains(p.DivisionId.ToString()))
            //           && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : ContraDocTypes.Contains(p.DocTypeId.ToString()))
            //           join t in db.FinishedProduct on p.ProductId equals t.ProductId into table
            //           from tab in table.DefaultIfEmpty()
            //           join t2 in db.ViewRugSize on p.ProductId equals t2.ProductId into table2
            //           from tab2 in table2.DefaultIfEmpty()
            //           join t3 in db.ProdOrderHeader on p.ProdOrderHeaderId equals t3.ProdOrderHeaderId into table3
            //           from tab3 in table3.DefaultIfEmpty()
            //           select new WeavingOrderWizardViewModel
            //           {
            //               Area = tab2.ManufaturingSizeArea * p.BalanceQty,
            //               BalanceQty = p.BalanceQty,
            //               BuyerId = tab3.BuyerId,
            //               BuyerName = tab3.Buyer.Person.Name,
            //               DesignName = tab.ProductGroup.ProductGroupName,
            //               Date = p.IndentDate.ToString(),
            //               DocNo = p.ProdOrderNo,
            //               DueDate = tab3.DueDate.ToString(),
            //               Qty = p.BalanceQty,
            //               Quality = tab.ProductQuality.ProductQualityName,
            //               Size = tab2.ManufaturingSizeName,
            //               ProdOrderLineId=p.ProdOrderLineId,
            //               RefDocLineId=p.ReferenceDocLineId,
            //               RefDocTypeId=p.ReferenceDocTypeId,
            //               DesignPatternName=tab.ProductDesign.ProductDesignName,
            //           };
            return temp;

        }

        public DateTime AddDueDate(DateTime Base, int DueDays)
        {
            DateTime DueDate = Base.AddDays((double)DueDays);
            if (DueDate.DayOfWeek == DayOfWeek.Sunday)
                DueDate = DueDate.AddDays(1);

            return DueDate;
        }

        public void Dispose()
        {
        }
    }
}
