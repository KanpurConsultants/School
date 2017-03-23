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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;

namespace Surya.India.Service
{
    public interface ISaleOrderCancelHeaderService : IDisposable
    {
        SaleOrderCancelHeader Create(SaleOrderCancelHeader p);
        void Delete(int id);
        SaleOrderCancelHeader Find(int id);
        void Delete(SaleOrderCancelHeader p);
        SaleOrderCancelHeader GetSaleOrderCancelHeader(int p);
        SaleOrderCancelHeaderDetailsViewModel GetSaleOrderCancelHeaderVM(int id);
        IEnumerable<SaleOrderCancelHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(SaleOrderCancelHeader p);
        SaleOrderCancelHeader Add(SaleOrderCancelHeader p);
        IEnumerable<SaleOrderCancelHeader> GetSaleOrderCancelHeaderList();
        IQueryable<SaleOrderCancelHeaderIndexViewModel> GetSOCList(int id);
        SaleOrderCancelHeader FindByDocNo(string DocNo);
        string GetMaxDocNo();
        IEnumerable<SaleOrderCancelPrintViewModel> GetSaleOrderCancelListToPrint();
        IEnumerable<SaleOrderCancelPrintViewModel> FGetPrintData(int Id);
        bool CheckForDocNoExists(string DocNo);
        int NextId(int id);
        int PrevId(int id);
    }

    public class SaleOrderCancelHeaderService : ISaleOrderCancelHeaderService
    {
        ApplicationDbContext db =new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<SaleOrderCancelHeader> _SaleOrderCancelHeaderRepository;
        RepositoryQuery<SaleOrderCancelHeader> SaleOrderCancelHeaderRepository;
        public SaleOrderCancelHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _SaleOrderCancelHeaderRepository = new Repository<SaleOrderCancelHeader>(db);
            SaleOrderCancelHeaderRepository = new RepositoryQuery<SaleOrderCancelHeader>(_SaleOrderCancelHeaderRepository);
        }

        public SaleOrderCancelHeader FindByDocNo(string DocNo)
        {
            return (from p in db.SaleOrderCancelHeader
                        where p.DocNo==DocNo
                        select p
                        ).FirstOrDefault();
        }

        public SaleOrderCancelHeader GetSaleOrderCancelHeader(int pId)
        {
            //Testing comments
            return _unitOfWork.Repository<SaleOrderCancelHeader>().Query().Get().Where(m => m.SaleOrderCancelHeaderId == pId).FirstOrDefault();
        }

        public SaleOrderCancelHeaderDetailsViewModel GetSaleOrderCancelHeaderVM(int id)
        {
            return (from p in db.SaleOrderCancelHeader
                    where p.SaleOrderCancelHeaderId == id
                    select new SaleOrderCancelHeaderDetailsViewModel
                    {                        

                        SiteId=p.SiteId,
                        SiteName = p.Site.SiteName,
                        DivisionName = p.Division.DivisionName,
                        DivisionId = p.DivisionId,
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        DocumentTypeName = p.DocType.DocumentTypeName,
                        DocumentTypeId = p.DocTypeId,
                        ReasonId=p.ReasonId,
                        Reason=p.Reason.ReasonName,
                        Remark = p.Remark,
                        SaleOrderCancelHeaderId = p.SaleOrderCancelHeaderId,
                        Status = p.Status,
                        CreatedBy=p.CreatedBy,
                        CreatedDate=p.CreatedDate,
                        ModifiedBy=p.ModifiedBy,
                        ModifiedDate=p.ModifiedDate
                    }
                     ).FirstOrDefault();
        }

        public SaleOrderCancelHeader Find(int id)
        {
            return _unitOfWork.Repository<SaleOrderCancelHeader>().Find(id);
        }

        public SaleOrderCancelHeader Create(SaleOrderCancelHeader p)
        {
            p.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleOrderCancelHeader>().Insert(p);
            return p;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleOrderCancelHeader>().Delete(id);
        }

        public void Delete(SaleOrderCancelHeader p)
        {
            _unitOfWork.Repository<SaleOrderCancelHeader>().Delete(p);
        }

        public void Update(SaleOrderCancelHeader p)
        {
            p.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleOrderCancelHeader>().Update(p);
        }

        public IEnumerable<SaleOrderCancelHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<SaleOrderCancelHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.SaleOrderCancelHeaderId))
                .Filter(q => !string.IsNullOrEmpty(q.DocNo ))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }


        public IEnumerable<SaleOrderCancelHeader> GetSaleOrderCancelHeaderList()
        {
            var p = _unitOfWork.Repository<SaleOrderCancelHeader>().Query().Get().OrderByDescending(m=>m.DocDate).ThenByDescending(m=>m.DocNo);           
            
            return p;
        }

        public IQueryable<SaleOrderCancelHeaderIndexViewModel> GetSOCList(int id)
        {

         

            //Working copy::
            //var temp = (from p in db.SaleOrderCancelHeader
            //            orderby p.DocDate descending, p.DocNo descending
            //            select new SaleOrderCancelHeaderDetailsViewModel
            //            {
            //                SaleOrderCancelHeaderId = p.SaleOrderCancelHeaderId,
            //                DocDate = p.DocDate,
            //                DocNo = p.DocNo,
            //                Reason = p.Reason.ReasonName,
            //                Remark = p.Remark,
            //                Status = p.Status,
            //            });                                   

            //var SaleOrderCancelBuyer = from L in db.SaleOrderCancelLine
            //                           join Ol in db.SaleOrderLine on L.SaleOrderLineId equals Ol.SaleOrderLineId into SaleOrderLineTable
            //                           from SaleOrderLineTab in SaleOrderLineTable.DefaultIfEmpty()
            //                           join Oh in db.SaleOrderHeader on SaleOrderLineTab.SaleOrderHeaderId equals Oh.SaleOrderHeaderId into SaleOrderHeaderTable
            //                           from SaleOrderHeaderTab in SaleOrderHeaderTable.DefaultIfEmpty()
            //                           group new { L, SaleOrderHeaderTab } by new { L.SaleOrderCancelHeaderId, SaleOrderHeaderTab.SaleToBuyerId } into Result
            //                           select new
            //                           {
            //                               SaleOrderCancelHeaderId = Result.Key.SaleOrderCancelHeaderId,
            //                               BuyerId = Result.Key.SaleToBuyerId
            //                           };

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var temp = from H in db.SaleOrderCancelHeader
                       orderby H.DocDate descending, H.DocNo descending
                       where H.SiteId==SiteId && H.DivisionId==DivisionId && H.DocTypeId==id
                       select new SaleOrderCancelHeaderIndexViewModel
                                        {
                                            SaleOrderCancelHeaderId = H.SaleOrderCancelHeaderId,
                                            DocNo = H.DocNo,
                                            DocDate = H.DocDate,
                                            BuyerName = H.Buyer.Person.Name,
                                            ReasonName = H.Reason.ReasonName,
                                            Remark = H.Remark,
                                            Status = H.Status,
                                            ModifiedBy=H.ModifiedBy,
                                        };


            return temp;
        }

        public SaleOrderCancelHeader Add(SaleOrderCancelHeader p)
        {
            _unitOfWork.Repository<SaleOrderCancelHeader>().Insert(p);
            return p;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<SaleOrderCancelHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SaleOrderCancelHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<SaleOrderCancelHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }
        public bool CheckForDocNoExists(string DocNo)
        {
            var temp = (from p in db.SaleOrderCancelHeader
                        where p.DocNo == DocNo
                        select p).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.SaleOrderCancelHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.SaleOrderCancelHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();


            }
            else
            {
                temp = (from p in db.SaleOrderCancelHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.SaleOrderCancelHeaderId).FirstOrDefault();
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

                temp = (from p in db.SaleOrderCancelHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.SaleOrderCancelHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.SaleOrderCancelHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.SaleOrderCancelHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public IEnumerable<SaleOrderCancelPrintViewModel> GetSaleOrderCancelListToPrint()
        {


            // var p = _unitOfWork.Repository<SaleOrderCancelPrintViewModel>().Query().Get().OrderByDescending(m => m.DocDate).ThenByDescending(m => m.DocNo);

            List<SaleOrderCancelPrintViewModel> TestList = new List<SaleOrderCancelPrintViewModel>();

            //SaleOrderCancelPrintViewModel test;



            string connectionString = "";
            //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
            DataSet ds = new DataSet();
            SqlCommand command = new SqlCommand();
            SqlDataAdapter sqlDataAapter;
            SqlParameter param;

            // var result = db.Company ().spSearchResults(Your list of parameters...);

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                command.Connection = sqlConnection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ProcSaleOrderCancelPrint";

                param = new SqlParameter("@V_DocId", "1027");
                param.Direction = ParameterDirection.Input;
                param.DbType = DbType.String;
                command.Parameters.Add(param);

                sqlDataAapter = new SqlDataAdapter(command);

                sqlDataAapter.Fill(ds);
            }

          
            return TestList;
        }

        public IEnumerable<SaleOrderCancelPrintViewModel> FGetPrintData(int Id)
        {
            IEnumerable<SaleOrderCancelPrintViewModel> saleordercancelprintviewmodel = db.Database.SqlQuery<SaleOrderCancelPrintViewModel>("ProcSaleOrderCancelPrint @Id", new SqlParameter("@Id", Id)).ToList();
            return saleordercancelprintviewmodel;
        }
    }
}
