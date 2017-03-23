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
using System.Data.SqlClient;
using System.Configuration;
using Surya.India.Model.ViewModels;

namespace Surya.India.Service
{
    public interface IProdOrderHeaderService : IDisposable
    {
        ProdOrderHeader Create(ProdOrderHeader pt);
        void Delete(int id);
        void Delete(ProdOrderHeader pt);
        ProdOrderHeader Find(string Name);
        ProdOrderHeader Find(int id);
        IEnumerable<ProdOrderHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(ProdOrderHeader pt);
        ProdOrderHeader Add(ProdOrderHeader pt);
        IQueryable<ProdOrderHeaderViewModel> GetProdOrderHeaderList(int id);        
        Task<IEquatable<ProdOrderHeader>> GetAsync();
        Task<ProdOrderHeader> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
        ProdOrderHeaderViewModel GetProdOrderHeader(int id);//HeaderId
        string GetMaxDocNo();

        /// <summary>
        ///Get the ProductionOrderHeader based on the materialplan headerid
        /// </summary>
        /// <param name="id">MaterialPlanHeaderId</param>        
        ProdOrderHeader GetProdOrderForMaterialPlan(int id);
        IEnumerable<ProdOrderHeaderViewModel> GetProdOrdersForDocumentType(string term,int DocTypeId,string ProcName);//DocTypeIds
    }

    public class ProdOrderHeaderService : IProdOrderHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<ProdOrderHeader> _ProdOrderHeaderRepository;
        RepositoryQuery<ProdOrderHeader> ProdOrderHeaderRepository;
        public ProdOrderHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ProdOrderHeaderRepository = new Repository<ProdOrderHeader>(db);
            ProdOrderHeaderRepository = new RepositoryQuery<ProdOrderHeader>(_ProdOrderHeaderRepository);
        }
        public ProdOrderHeader Find(string Name)
        {
            return ProdOrderHeaderRepository.Get().Where(i => i.DocNo == Name).FirstOrDefault();
        }


        public ProdOrderHeader Find(int id)
        {
            return _unitOfWork.Repository<ProdOrderHeader>().Find(id);
        }

        public ProdOrderHeader Create(ProdOrderHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ProdOrderHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<ProdOrderHeader>().Delete(id);
        }
        public IEnumerable<ProdOrderHeader> GetProdOrderListForMaterialPlan(int id)
        {
            return (from p in db.ProdOrderHeader
                    where p.MaterialPlanHeaderId == id
                    select p);
        }

        public void Delete(ProdOrderHeader pt)
        {
            _unitOfWork.Repository<ProdOrderHeader>().Delete(pt);
        }

        public void Update(ProdOrderHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ProdOrderHeader>().Update(pt);
        }

        public IEnumerable<ProdOrderHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<ProdOrderHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DocNo))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<ProdOrderHeaderViewModel> GetProdOrderHeaderList(int id)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            return (from p in db.ProdOrderHeader
                    join t in db.DocumentType on p.DocTypeId equals t.DocumentTypeId into table
                    from tab in table.DefaultIfEmpty()
                    where p.DivisionId==DivisionId && p.SiteId==SiteId && p.DocTypeId==id
                    orderby p.DocDate descending, p.DocNo descending
                    select new ProdOrderHeaderViewModel
                    {
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        DocTypeName = tab.DocumentTypeName,
                        DueDate = p.DueDate,
                        ProdOrderHeaderId = p.ProdOrderHeaderId,
                        Remark = p.Remark,
                        Status = p.Status,
                    }
                        );
            
        }

        public ProductionOrderSettings GetProductionOrderSettingsForDocument(int DocTypeId, int DivisionId, int SiteId)
        {
            return (from p in db.ProductionOrderSettings
                    where p.DocTypeId == DocTypeId && p.DivisionId == DivisionId && p.SiteId == SiteId
                    select p
                        ).FirstOrDefault();
        }

        public ProdOrderHeaderViewModel GetProdOrderHeader(int id)
        {
            return (from p in db.ProdOrderHeader
                    join t in db.MaterialPlanHeader on p.MaterialPlanHeaderId equals t.MaterialPlanHeaderId into table from tab in table.DefaultIfEmpty()
                    where p.ProdOrderHeaderId == id
                    select new ProdOrderHeaderViewModel
                    {
                        MaterialPlanDocNo=tab.DocNo,
                        DivisionId = p.DivisionId,
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        DocTypeId = p.DocTypeId,
                        DueDate = p.DueDate,
                        ProdOrderHeaderId = p.ProdOrderHeaderId,
                        Remark = p.Remark,
                        SiteId = p.SiteId,
                        Status = p.Status,
                    }
                        ).FirstOrDefault();
        }

        public ProdOrderHeader Add(ProdOrderHeader pt)
        {
            _unitOfWork.Repository<ProdOrderHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.ProdOrderHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.ProdOrderHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.ProdOrderHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.ProdOrderHeaderId).FirstOrDefault();
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

                temp = (from p in db.ProdOrderHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.ProdOrderHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.ProdOrderHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.ProdOrderHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }
        public IEnumerable<ProdOrderHeaderListViewModel> GetPendingProdOrders(int id)
        {
            return (from p in db.ViewProdOrderBalance
                    join t in db.ProdOrderHeader on p.ProdOrderHeaderId equals t.ProdOrderHeaderId into table
                    from tab in table.DefaultIfEmpty()
                    join t1 in db.ProdOrderLine on p.ProdOrderLineId equals t1.ProdOrderLineId into table1
                    from tab1 in table1.DefaultIfEmpty()
                    where p.ProductId == id && p.BalanceQty > 0
                    select new ProdOrderHeaderListViewModel
                    {
                        ProdOrderLineId= p.ProdOrderLineId,
                        ProdOrderHeaderId= p.ProdOrderHeaderId,
                        DocNo = tab.DocNo,
                        Dimension1Name = tab1.Dimension1.Dimension1Name,
                        Dimension2Name = tab1.Dimension2.Dimension2Name,
                    }
                        );
        }
        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<ProdOrderHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public ProdOrderHeader GetProdOrderForMaterialPlan(int id)
        {
            return (from p in db.ProdOrderHeader
                    where p.MaterialPlanHeaderId == id
                    select p
                        ).FirstOrDefault();
        }

        public IEnumerable<ProdOrderHeaderViewModel> GetProdOrdersForDocumentType( string term,int DocHeaderId,string ProcName)//DocTypeId
        {
            SqlParameter SqlParameterProductId = new SqlParameter("@MaterialPlanHeaderId", DocHeaderId);

            IEnumerable<ProdOrderBalanceViewModel> StockAvailableForPacking = db.Database.SqlQuery<ProdOrderBalanceViewModel>(" " + ProcName + " @MaterialPlanHeaderId", SqlParameterProductId).ToList();

            var temp = from p in StockAvailableForPacking                       
                       where  p.BalanceQty > 0 && p.ProdOrderNo.ToLower().Contains(term.ToLower())
                       group new { p } by p.ProdOrderHeaderId into g
                       orderby g.Key descending
                       select new ProdOrderHeaderViewModel
                       {
                           ProdOrderHeaderId = g.Key,
                           DocNo = g.Max(m => m.p.ProdOrderNo) +" | "+ g.Max(m => m.p.DocTypeName),                           
                       };

            return temp;

        }

        public void Dispose()
        {
        }


        public Task<IEquatable<ProdOrderHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProdOrderHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
