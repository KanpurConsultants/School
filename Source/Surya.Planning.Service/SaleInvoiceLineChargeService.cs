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
    public interface ISaleInvoiceLineChargeService : IDisposable
    {
        SaleInvoiceLineCharge Create(SaleInvoiceLineCharge pt);
        void Delete(int id);
        void Delete(SaleInvoiceLineCharge pt);
        SaleInvoiceLineCharge Find(int id);
        IEnumerable<SaleInvoiceLineCharge> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(SaleInvoiceLineCharge pt);
        SaleInvoiceLineCharge Add(SaleInvoiceLineCharge pt);
        IEnumerable<SaleInvoiceLineCharge> GetSaleInvoiceLineChargeList();

        // IEnumerable<SaleInvoiceLineCharge> GetSaleInvoiceLineChargeList(int buyerId);
        Task<IEquatable<SaleInvoiceLineCharge>> GetAsync();
        Task<SaleInvoiceLineCharge> FindAsync(int id);
        IEnumerable<LineChargeViewModel> GetCalculationProductList(int LineTableId);
        IEnumerable<LineChargeViewModel> GetCalculationProductListSProc(int LineTableId, string LineTableName);
        int NextId(int id);
        int PrevId(int id);
        int GetMaxProductCharge(int HeaderId, string LineTableName, string HeaderFieldName, string LineTableKeyField);
    }

    public class SaleInvoiceLineChargeService : ISaleInvoiceLineChargeService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<SaleInvoiceLineCharge> _SaleInvoiceLineChargeRepository;
        RepositoryQuery<SaleInvoiceLineCharge> SaleInvoiceLineChargeRepository;
        public SaleInvoiceLineChargeService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public SaleInvoiceLineCharge Find(int id)
        {
            return (_unitOfWork.Repository<SaleInvoiceLineCharge>().Find(id));
        }

        public SaleInvoiceLineCharge Create(SaleInvoiceLineCharge pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleInvoiceLineCharge>().Insert(pt);

            return pt;
        }
        public IEnumerable<LineChargeViewModel> GetCalculationProductList(int LineTableId)
        {
            return (from p in db.SaleInvoiceLineCharge
                    where p.LineTableId== LineTableId
                    orderby p.Sr
                    select new LineChargeViewModel
                    {
                        AddDeduct = p.AddDeduct,
                        AffectCost = p.AffectCost,
                        CalculateOnId = p.CalculateOnId,
                        CalculateOnName = p.CalculateOn.ChargeName,
                        CalculateOnCode = p.CalculateOn.ChargeCode,                       
                        LineTableId=p.LineTableId,
                        Id = p.Id,
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
                        Rate = p.Rate,
                        Sr = p.Sr,
                        RateType = p.RateType,
                        IsVisible = p.IsVisible,
                        Amount = p.Amount,
                        ParentChargeId = p.ParentChargeId,                      

                    }
                      );
        }

        public int GetMaxProductCharge(int HeaderId,string LineTableName,string HeaderFieldName,string LineTableKeyField)
        {
            //var temp=from p in db.SaleInvoiceLineCharge
            //        where p.LineTableId == (from p1 in db.SaleInvoiceLine
            //                                orderby p1.SaleInvoiceLineId descending
            //                                    where p1.SaleInvoiceHeaderId==HeaderId
            //                                    select p1.SaleInvoiceLineId
            //                                    ).FirstOrDefault()
            //        orderby p.Sr
            //        select new LineChargeViewModel
            //        {
            //            AddDeduct = p.AddDeduct,
            //            AffectCost = p.AffectCost,
            //            CalculateOnId = p.CalculateOnId,
            //            CalculateOnName = p.CalculateOn.ChargeName,
            //            CalculateOnCode = p.CalculateOn.ChargeCode,
            //            LineTableId = p.LineTableId,
            //            Id = p.Id,
            //            ChargeId = p.ChargeId,
            //            ChargeName = p.Charge.ChargeName,
            //            ChargeCode = p.Charge.ChargeCode,
            //            ChargeTypeId = p.ChargeTypeId,
            //            ChargeTypeName = p.ChargeType.ChargeTypeName,
            //            CostCenterId = p.CostCenterId,
            //            CostCenterName = p.CostCenter.CostCenterName,
            //            IncludedInBase = p.IncludedInBase,
            //            LedgerAccountCrId = p.LedgerAccountCrId,
            //            LedgerAccountCrName = p.LedgerAccountCr.LedgerAccountName,
            //            LedgerAccountDrId = p.LedgerAccountDrId,
            //            LedgerAccountDrName = p.LedgerAccountDr.LedgerAccountName,
            //            Rate = p.Rate,
            //            Sr = p.Sr,
            //            RateType = p.RateType,
            //            IsVisible = p.IsVisible,
            //            Amount = 0,
            //            ParentChargeId = p.ParentChargeId,
            //        };
            //return temp;

            SqlParameter SqlParameterHeaderTableId = new SqlParameter("@HeaderTableKeyValue", HeaderId);
            SqlParameter SqlParameterLineTableName = new SqlParameter("@LineTableName", LineTableName);
            SqlParameter SqlParameterHeaderFieldName = new SqlParameter("@HeaderFieldName", HeaderFieldName);
            SqlParameter SqlParameterLineTableFieldId = new SqlParameter("@LineTableKeyField", LineTableKeyField);

            int ? CalculationLineList = Convert.ToInt32(db.Database.SqlQuery<int ?>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".procGetCalculationMaxLineId @HeaderTableKeyValue, @LineTableName,@HeaderFieldName,@LineTableKeyField", SqlParameterHeaderTableId, SqlParameterLineTableName, SqlParameterHeaderFieldName, SqlParameterLineTableFieldId).FirstOrDefault());

            return CalculationLineList??0;

        }

        public IEnumerable<LineChargeViewModel> GetCalculationProductListSProc(int LineTableId,string LineTableName)
        {
            SqlParameter SqlParameterLineTableId = new SqlParameter("@LineTableld", LineTableId);
            SqlParameter SqlParameterLineTableName = new SqlParameter("@LineTableName", LineTableName);

            IEnumerable<LineChargeViewModel> CalculationLineList = db.Database.SqlQuery<LineChargeViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".CalculationLineCharge @LineTableld, @LineTableName", SqlParameterLineTableId, SqlParameterLineTableName).ToList();

            return CalculationLineList;
        }
        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleInvoiceLineCharge>().Delete(id);
        }

        public void Delete(SaleInvoiceLineCharge pt)
        {
            _unitOfWork.Repository<SaleInvoiceLineCharge>().Delete(pt);
        }

        public void Update(SaleInvoiceLineCharge pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleInvoiceLineCharge>().Update(pt);
        }

        public IEnumerable<SaleInvoiceLineCharge> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<SaleInvoiceLineCharge>()
                .Query()               
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<SaleInvoiceLineCharge> GetSaleInvoiceLineChargeList()
        {
            var pt = _unitOfWork.Repository<SaleInvoiceLineCharge>().Query().Get().OrderBy(m=>m.LineTableId);

            return pt;
        }

        public SaleInvoiceLineCharge Add(SaleInvoiceLineCharge pt)
        {
            _unitOfWork.Repository<SaleInvoiceLineCharge>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.SaleInvoiceLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.SaleInvoiceLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).FirstOrDefault();
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

                temp = (from p in db.SaleInvoiceLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.SaleInvoiceLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<SaleInvoiceLineCharge>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SaleInvoiceLineCharge> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
