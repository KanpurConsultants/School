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

namespace Surya.India.Service
{
    public interface IUpdateSaleExpiryService : IDisposable
    {
        bool UpdateSaleOrderExpiry(int SaleOrderId, string Reason, string User, DateTime ExpiryDate);
    }

    public class UpdateSaleExpiryService : IUpdateSaleExpiryService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public UpdateSaleExpiryService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool UpdateSaleOrderExpiry(int SaleOrderId, string Reason, string User, DateTime ExpiryDate)
        {
            var Rec = (from p in db.SaleOrderHeader
                           where p.SaleOrderHeaderId==SaleOrderId
                           select p).FirstOrDefault();           

            if (Rec == null)
                return false;
            else
            { 
                using (ApplicationDbContext con=new ApplicationDbContext())
                {                    
                    Rec.DueDate = ExpiryDate;
                    Rec.ModifiedBy = User;
                    Rec.ModifiedDate = DateTime.Now;
                    Rec.ObjectState = Model.ObjectState.Modified;
                    con.SaleOrderHeader.Add(Rec);

                    ActivityLog log = new ActivityLog();                    
                    log.CreatedBy = User;
                    log.CreatedDate = DateTime.Now;                    
                    log.DocId = Rec.SaleOrderHeaderId;
                    log.DocTypeId = Rec.DocTypeId;                    
                    log.Narration = "SaleOrder Expiry changed";
                    log.UserRemark = Reason;
                    log.ObjectState = Model.ObjectState.Added;

                    con.ActivityLog.Add(log);

                    con.SaveChanges();
                }
                return true;
            }
        }
        public void Dispose()
        {
        }
    }
}
