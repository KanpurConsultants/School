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
using Surya.India.Model.ViewModel;

namespace Surya.India.Service
{
    public interface ISaleDeliveryOrderHeaderService : IDisposable
    {
        SaleDeliveryOrderHeader Create(SaleDeliveryOrderHeader s);
        void Delete(int id);
        void Delete(SaleDeliveryOrderHeader s);
        SaleDeliveryOrderHeader GetSaleDeliveryOrderHeader(int id);
        SaleDeliveryOrderHeader Find(int id);
        IQueryable<SaleDeliveryOrderHeaderIndexViewModel> GetSaleDeliveryOrderHeaderList(int id);
        void Update(SaleDeliveryOrderHeader s);       
    }
    public class SaleDeliveryOrderHeaderService : ISaleDeliveryOrderHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public SaleDeliveryOrderHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

        public SaleDeliveryOrderHeader Create(SaleDeliveryOrderHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleDeliveryOrderHeader>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleDeliveryOrderHeader>().Delete(id);
        }
        public void Delete(SaleDeliveryOrderHeader s)
        {
            _unitOfWork.Repository<SaleDeliveryOrderHeader>().Delete(s);
        }
        public void Update(SaleDeliveryOrderHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleDeliveryOrderHeader>().Update(s);
        }

        public SaleDeliveryOrderHeader GetSaleDeliveryOrderHeader(int id)
        {
            return (from p in db.SaleDeliveryOrderHeader
                    where p.SaleDeliveryOrderHeaderId == id
                    select p).FirstOrDefault();
        }

        public SaleDeliveryOrderHeader Find(int id)
        {
            return _unitOfWork.Repository<SaleDeliveryOrderHeader>().Find(id);
        }



        public IQueryable<SaleDeliveryOrderHeaderIndexViewModel> GetSaleDeliveryOrderHeaderList(int id)
        {
            int divisionid = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int siteid = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var temp = from p in db.SaleDeliveryOrderHeader
                       orderby p.DocDate descending, p.DocNo descending
                       where p.DivisionId == divisionid && p.SiteId == siteid && p.DocTypeId == id
                       select new SaleDeliveryOrderHeaderIndexViewModel
                       {
                           DocDate = p.DocDate,
                           SaleDeliveryOrderHeaderId = p.SaleDeliveryOrderHeaderId,
                           DocNo = p.DocNo,
                           DueDate = p.DueDate,
                           BuyerName = p.Buyer.Person.Name,
                           Status = p.Status,
                           ModifiedBy=p.ModifiedBy,
                       };
            return temp;
        }

        public void Dispose()
        {
        }   
    }
}
