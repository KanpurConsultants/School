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
using System.Data.SqlClient;
using System.Configuration;

namespace Surya.India.Service
{
    public interface INextPrevIdService : IDisposable
    {
        int GetNextPrevId(int DocId, int DocTypeId, string UserName, string ForAction, string HeaderTableName, string HeaderTablePK, string NextPrev);
    }

    public class NextPrevIdService : INextPrevIdService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
     
        public NextPrevIdService(IUnitOfWorkForService unitofWork)
        {
            _unitOfWork = unitofWork;
        }


        public int GetNextPrevId(int DocId, int DocTypeId, string UserName, string ForAction, string HeaderTableName, string HeaderTablePK, string NextPrev)
        {

            SqlParameter SqlParameterUserName = new SqlParameter("@UserName", UserName);
            SqlParameter SqlParameterForAction = new SqlParameter("@ForAction", ForAction);
            SqlParameter SqlParameterHeaderTableName = new SqlParameter("@HeaderTableName", HeaderTableName);
            SqlParameter SqlParameterHeaderTablePkFieldName = new SqlParameter("@HeaderTablePkFieldName", HeaderTablePK);
            SqlParameter SqlParameterDocId = new SqlParameter("@DocId", DocId);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", DocTypeId);
            SqlParameter SqlParameterNextPrevious = new SqlParameter("@NextPrevious", NextPrev);

            int Id = db.Database.SqlQuery<int>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spGetNextPreviousId @UserName, @ForAction, @HeaderTableName, @HeaderTablePkFieldName, @DocId, @DocTypeId, @NextPrevious", SqlParameterUserName, SqlParameterForAction, SqlParameterHeaderTableName,SqlParameterHeaderTablePkFieldName,SqlParameterDocId,SqlParameterDocTypeId,SqlParameterNextPrevious).FirstOrDefault();

            return Id;


        }

        public void Dispose()
        {
        }
       
    }
}
