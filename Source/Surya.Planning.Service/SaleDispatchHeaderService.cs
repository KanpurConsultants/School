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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;

namespace Surya.India.Service
{
    public interface ISaleDispatchHeaderService : IDisposable
    {
        SaleDispatchHeader Create(SaleDispatchHeader s);
        void Delete(int id);
        void Delete(SaleDispatchHeader s);
        SaleDispatchHeader GetSaleDispatchHeader(int id);

        SaleDispatchHeader Find(int id);
        void Update(SaleDispatchHeader s);
        string GetMaxDocNo();
        SaleDispatchHeader FindByDocNo(string Docno);
    }
    public class SaleDispatchHeaderService : ISaleDispatchHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public SaleDispatchHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

     public SaleDispatchHeader Create(SaleDispatchHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleDispatchHeader>().Insert(s);
            return s;
        }

       public void Delete(int id)
     {
         _unitOfWork.Repository<SaleDispatchHeader>().Delete(id);
     }
       public void Delete(SaleDispatchHeader s)
        {
            _unitOfWork.Repository<SaleDispatchHeader>().Delete(s);
        }
       public void Update(SaleDispatchHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleDispatchHeader>().Update(s);            
        }

       public SaleDispatchHeader GetSaleDispatchHeader(int id)
        {
            return _unitOfWork.Repository<SaleDispatchHeader>().Query().Get().Where(m => m.SaleDispatchHeaderId == id).FirstOrDefault();
        }

        public SaleDispatchHeader Find(int id)
        {
            return _unitOfWork.Repository<SaleDispatchHeader>().Find(id);
        }


        public SaleDispatchHeader FindByDocNo(string Docno)
       {
         return  _unitOfWork.Repository<SaleDispatchHeader>().Query().Get().Where(m => m.DocNo == Docno).FirstOrDefault();

       }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<SaleDispatchHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public void Dispose()
        {
        }
    }
}
