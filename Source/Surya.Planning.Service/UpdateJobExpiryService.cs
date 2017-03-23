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
    public interface IUpdateJobExpiryService : IDisposable
    {
        bool UpdateJobOrderExpiry(int JobOrderId, string Reason, string User, DateTime ExpiryDate);
    }

    public class UpdateJobExpiryService : IUpdateJobExpiryService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public UpdateJobExpiryService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool UpdateJobOrderExpiry(int JobOrderId, string Reason, string User, DateTime ExpiryDate)
        {
            var Rec = (from p in db.JobOrderHeader
                           where p.JobOrderHeaderId==JobOrderId
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
                    con.JobOrderHeader.Add(Rec);

                    ActivityLog log = new ActivityLog();                    
                    log.CreatedBy = User;
                    log.CreatedDate = DateTime.Now;                    
                    log.DocId = Rec.JobOrderHeaderId;
                    log.DocTypeId = Rec.DocTypeId;                    
                    log.Narration = "JobOrder Expiry changed";
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
