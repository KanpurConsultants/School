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
using System.Data.SqlClient;
using System.Configuration;

namespace Surya.India.Service
{
    public interface ISaleInvoiceReturnHeaderChargeService : IDisposable
    {
        SaleInvoiceReturnHeaderCharge Create(SaleInvoiceReturnHeaderCharge pt);
        void Delete(int id);
        void Delete(SaleInvoiceReturnHeaderCharge pt);
        SaleInvoiceReturnHeaderCharge Find(int id);
        IEnumerable<SaleInvoiceReturnHeaderCharge> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(SaleInvoiceReturnHeaderCharge pt);
        SaleInvoiceReturnHeaderCharge Add(SaleInvoiceReturnHeaderCharge pt);
        IEnumerable<SaleInvoiceReturnHeaderCharge> GetSaleInvoiceReturnHeaderChargeList();
        Task<IEquatable<SaleInvoiceReturnHeaderCharge>> GetAsync();
        Task<SaleInvoiceReturnHeaderCharge> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
        IEnumerable<HeaderChargeViewModel> GetCalculationFooterList(int HeaderTableId);
    }

    public class SaleInvoiceReturnHeaderChargeService : ISaleInvoiceReturnHeaderChargeService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        public SaleInvoiceReturnHeaderChargeService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<HeaderChargeViewModel> GetCalculationFooterList(int HeaderTableId)
        {
            var temp=from p in db.SaleInvoiceReturnHeaderCharge
                    where p.HeaderTableId==HeaderTableId
                    orderby p.Sr
                    select new HeaderChargeViewModel
                    {
                        AddDeduct = p.AddDeduct,
                        AffectCost = p.AffectCost,
                        CalculateOnId = p.CalculateOnId,
                        CalculateOnName = p.CalculateOn.ChargeName,
                        CalculateOnCode = p.CalculateOn.ChargeCode,
                        Id = p.Id,
                        HeaderTableId=p.HeaderTableId,
                        ChargeId = p.ChargeId,
                        ChargeName = p.Charge.ChargeName,
                        ChargeCode = p.Charge.ChargeCode,
                        ChargeTypeId = p.ChargeTypeId,
                        ChargeTypeName = p.ChargeType.ChargeTypeName,
                        CostCenterId = p.CostCenterId,
                        CostCenterName = p.CostCenter.CostCenterName,
                        IncludedInBase = p.IncludedInBase,
                        LedgerAccountCrId = p.LedgerAccountCrId,
                        LedgerAccountCrName = p.LedgerAccountCr.LedgerAccountName,
                        LedgerAccountDrId = p.LedgerAccountDrId,
                        LedgerAccountDrName = p.LedgerAccountDr.LedgerAccountName,
                        ParentChargeId = p.ParentChargeId,
                        ProductChargeId = p.ProductChargeId,
                        ProductChargeName = p.ProductCharge.ChargeName,
                        ProductChargeCode = p.ProductCharge.ChargeCode,
                        Rate = p.Rate,
                        Amount = p.Amount,
                        Sr = p.Sr,
                        RateType = p.RateType,
                        IsVisible = p.IsVisible,
                    };

            return (temp);
        }

        public IEnumerable<HeaderChargeViewModel> GetCalculationFooterListSProc(int HeaderTableId, string HeaderTableName,string LineTableName)
        {
            SqlParameter SqlParameterHeaderTableId = new SqlParameter("@HeaderTableId", HeaderTableId);
            SqlParameter SqlParameterHeaderTableName = new SqlParameter("@HeaderTableName", HeaderTableName);
            SqlParameter SqlParameterLineTableName = new SqlParameter("@LineTableName", LineTableName);

            IEnumerable<HeaderChargeViewModel> CalculationHeaderList = db.Database.SqlQuery<HeaderChargeViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".CalculationHeaderCharge @HeaderTableId, @HeaderTableName, @LineTableName", SqlParameterHeaderTableId, SqlParameterHeaderTableName, SqlParameterLineTableName).ToList();

            return CalculationHeaderList;
        }

        public SaleInvoiceReturnHeaderCharge Find(int id)
        {
            return (_unitOfWork.Repository<SaleInvoiceReturnHeaderCharge>().Find(id));
        }

        public SaleInvoiceReturnHeaderCharge Create(SaleInvoiceReturnHeaderCharge pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleInvoiceReturnHeaderCharge>().Insert(pt);

            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleInvoiceReturnHeaderCharge>().Delete(id);
        }

        public void Delete(SaleInvoiceReturnHeaderCharge pt)
        {
            _unitOfWork.Repository<SaleInvoiceReturnHeaderCharge>().Delete(pt);
        }

        public void Update(SaleInvoiceReturnHeaderCharge pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleInvoiceReturnHeaderCharge>().Update(pt);
        }

        public IEnumerable<SaleInvoiceReturnHeaderCharge> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<SaleInvoiceReturnHeaderCharge>()
                .Query()               
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<SaleInvoiceReturnHeaderCharge> GetSaleInvoiceReturnHeaderChargeList()
        {
            var pt = _unitOfWork.Repository<SaleInvoiceReturnHeaderCharge>().Query().Get().OrderBy(m=>m.HeaderTableId);

            return pt;
        }

        public SaleInvoiceReturnHeaderCharge Add(SaleInvoiceReturnHeaderCharge pt)
        {
            _unitOfWork.Repository<SaleInvoiceReturnHeaderCharge>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.SaleInvoiceReturnHeaderCharge
                        orderby p.HeaderTableId
                        select p.HeaderTableId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.SaleInvoiceReturnHeaderCharge
                        orderby p.HeaderTableId
                        select p.HeaderTableId).FirstOrDefault();
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

                temp = (from p in db.SaleInvoiceReturnHeaderCharge
                        orderby p.HeaderTableId
                        select p.HeaderTableId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.SaleInvoiceReturnHeaderCharge
                        orderby p.HeaderTableId
                        select p.HeaderTableId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<SaleInvoiceReturnHeaderCharge>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SaleInvoiceReturnHeaderCharge> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}