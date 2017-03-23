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

namespace Surya.India.Service
{
    public interface ICommonDocumentService : IDisposable
    {
        int NextId(int id, string TableName, string UserName, string Status);
        int PrevId(int id, string TableName, string UserName, string Status);
    }

    public class CommonDocumentService : ICommonDocumentService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public CommonDocumentService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int NextId(int id, string TableName, string UserName, string Status)
        {
            SqlParameter SqlParameterDocId = new SqlParameter("@DocId", id);
            SqlParameter SqlParameterTableName = new SqlParameter("@TableName", TableName);
            SqlParameter SqlParameterUserName = new SqlParameter("@UserName", UserName);
            SqlParameter SqlParameterStatus = new SqlParameter("@Status", Status);

            int NextId = db.Database.SqlQuery<int>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".ProcGetStockForPacking @DocId, @TableName, @UserName, @Status", SqlParameterDocId, SqlParameterTableName, SqlParameterUserName, SqlParameterStatus).FirstOrDefault();

            return NextId;
        }

        public int PrevId(int id, string TableName, string UserName, string Status)
        {

            SqlParameter SqlParameterDocId = new SqlParameter("@DocId", id);
            SqlParameter SqlParameterTableName = new SqlParameter("@TableName", TableName);
            SqlParameter SqlParameterUserName = new SqlParameter("@UserName", UserName);
            SqlParameter SqlParameterStatus = new SqlParameter("@Status", Status);

            int PrevId = db.Database.SqlQuery<int>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".ProcGetStockForPacking @DocId, @TableName, @UserName, @Status", SqlParameterDocId, SqlParameterTableName, SqlParameterUserName, SqlParameterStatus).FirstOrDefault();

            return PrevId;
        }

        public void Dispose()
        {
        }
    }
}
